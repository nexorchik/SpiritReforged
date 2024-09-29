using Terraria.DataStructures;
using Terraria.WorldBuilding;

namespace SpiritReforged.Common.WorldGeneration.Microtones.Passes;

internal class FishingAreaMicropass : Micropass
{
	public static Dictionary<int, Point16[]> OffsetFuncsBySubId = new Dictionary<int, Point16[]>()
	{
		{ 0, Offsets0() }, { 1, Offsets1() }, { 2, Offsets2() }
	};

	private static Point16[] Offsets2()
	{
		List<Point16> points = [];
		points.Add(new Point16(28, 3));
		points.Add(new Point16(2, 18));
		points.Add(new Point16(42, 20));

		return [.. points];
	}

	private static Point16[] Offsets1()
	{
		List<Point16> points = [];
		points.Add(new Point16(30, 7));
		points.Add(new Point16(1, 12));

		return [.. points];
	}
	
	private static Point16[] Offsets0()
	{
		List<Point16> points = [];
		points.Add(new Point16(9, 5));
		points.Add(new Point16(54, 19));
		points.Add(new Point16(6, 15));

		return [.. points];
	}

	public override string WorldGenName => "Adding fishing spots";

	public override int GetWorldGenIndexInsert(List<GenPass> passes, ref bool afterIndex) => passes.FindIndex(genpass => genpass.Name.Equals("Sunflowers"));

	public override void Run(GenerationProgress progress, Terraria.IO.GameConfiguration config)
	{
		int repeats = (int)(Main.maxTilesX / 4200f * 10);

		for (int i = 0; i < repeats; ++i)
		{
			int subId = WorldGen.genRand.Next(2);
			string subChar = WorldGen.genRand.NextBool() ? "a" : "b";
			string structureName = "Assets/Structures/Coves/FishCove" + subId + subChar;
			Point16 size = new();
			StructureHelper.Generator.GetDimensions(structureName, ModContent.GetInstance<SpiritReforgedMod>(), ref size);
			Point16 position;

			do
			{
				position = new Point16(WorldGen.genRand.Next(60, Main.maxTilesX - 60), WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY - 400));
			} while (!Collision.SolidCollision(position.ToWorldCoordinates(), 32, 32));

			position -= WorldGen.genRand.Next(OffsetFuncsBySubId[subId]);

			if (GenVars.structures.CanPlace(new Rectangle(position.X, position.Y, size.X, size.Y), 10))
			{
				var biome = QuickConversion.FindConversionBiome(position, size);

				if (biome == QuickConversion.BiomeType.Desert) // Don't spawn in deserts
				{
					i--;
					continue;
				}

				var conditions = TileCondition.GetArea(position.X, position.Y, size.X, size.Y);
				StructureTools.PlaceByOrigin(structureName, position, new(0));
				QuickConversion.SimpleConvert(conditions, biome, biome != QuickConversion.BiomeType.Purity);
				GenVars.structures.AddProtectedStructure(new Rectangle(position.X, position.Y, size.X, size.Y), 6);
			}
		}
	}
}
