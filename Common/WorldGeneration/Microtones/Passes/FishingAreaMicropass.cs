using Terraria.DataStructures;
using Terraria.WorldBuilding;

namespace SpiritReforged.Common.WorldGeneration.Microtones.Passes;

internal class FishingAreaMicropass : Micropass
{
	public static Dictionary<int, Func<Point16[]>> OffsetFuncsBySubId = new Dictionary<int, Func<Point16[]>>()
	{
		{ 0, Offsets0 }
	};

	/// <summary>
	/// Offsets for the 0th cove.
	/// </summary>
	/// <returns></returns>
	private static Point16[] Offsets0()
	{
		List<Point16> points = [];

		points.Add(Vector2.Lerp(new Vector2(25, 4), new Vector2(36, 11), WorldGen.genRand.NextFloat()).ToPoint16());
		points.Add(new Point16(1, 12));

		return [.. points];
	}

	public override string WorldGenName => "Adding fishing spots";

	public override int GetWorldGenIndexInsert(List<GenPass> passes, ref bool afterIndex) => passes.FindIndex(genpass => genpass.Name.Equals("Sunflowers"));

	public override void Run(GenerationProgress progress, Terraria.IO.GameConfiguration config)
	{
		int repeats = (int)(Main.maxTilesX / 4200f * 16);

		for (int i = 0; i < repeats; ++i)
		{
			int subId = WorldGen.genRand.Next(1);
			string subChar = WorldGen.genRand.NextBool() ? "a" : "b";
			string structureName = "Assets/Structures/Coves/FishCove" + subId + subChar;
			Point16 size = new();
			StructureHelper.Generator.GetDimensions(structureName, ModContent.GetInstance<SpiritReforgedMod>(), ref size);
			Point16 position;

			do
			{
				position = new Point16(WorldGen.genRand.Next(60, Main.maxTilesX - 60), WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY - 400));
			} while (!Collision.SolidCollision(position.ToWorldCoordinates(), 32, 32));

			position -= WorldGen.genRand.Next(OffsetFuncsBySubId[subId]());

			if (GenVars.structures.CanPlace(new Rectangle(position.X, position.Y, size.X, size.Y), 10))
			{
				var conditions = TileCondition.GetArea(position.X, position.Y, size.X, size.Y);
				var biome = QuickConversion.FindConversionBiome(position, size);
				StructureTools.PlaceByOrigin(structureName, position, new(0));
				QuickConversion.SimpleConvert(conditions, biome);
				GenVars.structures.AddProtectedStructure(new Rectangle(position.X, position.Y, size.X, size.Y), 6);
			}
		}
	}
}
