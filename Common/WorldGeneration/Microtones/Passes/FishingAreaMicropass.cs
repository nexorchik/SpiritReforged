using Terraria.DataStructures;
using Terraria.WorldBuilding;

namespace SpiritReforged.Common.WorldGeneration.Microtones.Passes;

internal class FishingAreaMicropass : Micropass
{
	public override string WorldGenName => "Adding fishing spots";

	public override int GetWorldGenIndexInsert(List<GenPass> passes, ref bool afterIndex) => passes.FindIndex(genpass => genpass.Name.Equals("Sunflowers"));

	public override void Run(GenerationProgress progress, Terraria.IO.GameConfiguration config)
	{
		int repeats = (int)(Main.maxTilesX / 4200f * 8);

		for (int i = 0; i < repeats; ++i)
		{
			int subId = WorldGen.genRand.Next(5);
			string subChar = WorldGen.genRand.NextBool() ? "a" : "b";
			string structureName = "Assets/Structures/Coves/FishCove" + subId + subChar;
			Point16 size = new();
			StructureHelper.Generator.GetDimensions(structureName, ModContent.GetInstance<SpiritReforgedMod>(), ref size);
			Point16 position;

			do
			{
				position = new Point16(WorldGen.genRand.Next(60, Main.maxTilesX - 60), WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY - 400));
			} while (CanPlaceStructureAt(size, position));

			StructureTools.PlaceByOrigin(structureName, position, new(0));
			GenVars.structures.AddProtectedStructure(new Rectangle(position.X, position.Y, size.X, size.Y), 6);
		}
	}

	private static bool CanPlaceStructureAt(Point16 size, Point16 position)
	{
		bool open = GenVars.structures.CanPlace(new Rectangle(position.X, position.Y, size.X, size.Y), 10);
		//bool hasAttachment = RecursivelyFindOpening(position, size);
		return open;
	}
}
