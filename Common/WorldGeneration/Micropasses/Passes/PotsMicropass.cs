using Terraria.WorldBuilding;
using SpiritReforged.Common.WorldGeneration.Micropasses;
using SpiritReforged.Common.WorldGeneration;
using SpiritReforged.Content.Underground.Tiles;

namespace SpiritMod.World.Micropasses;

internal class PotsMicropass : Micropass
{
	public override string WorldGenName => "Pots";

	public override int GetWorldGenIndexInsert(List<GenPass> passes, ref bool afterIndex)
	{
		afterIndex = true;
		return passes.FindIndex(genpass => genpass.Name.Equals("Pots"));
	}

	public override void Run(GenerationProgress progress, Terraria.IO.GameConfiguration config)
	{
		const int maxTries = 5000; //Failsafe

		progress.Message = Language.GetTextValue("Mods.SpiritReforged.Generation.Pots");

		int maxPots = (int)(Main.maxTilesX * Main.maxTilesY * 0.0005); //Normal weight is 0.0008
		int pots = 0;

		for (int t = 0; t < maxTries; t++)
		{
			int x = Main.rand.Next(20, Main.maxTilesX - 20);
			int y = Main.rand.Next((int)GenVars.worldSurfaceHigh, Main.maxTilesY - 20);

			WorldMethods.FindGround(x, ref y);

			if (CreateStack(x, y - 1) && ++pots >= maxPots)
				break;
		}
	}

	private static bool CreateStack(int x, int y)
	{
		int t = Main.tile[x, y + 1].TileType;
		int w = Main.tile[x, y].WallType;

		if (w is WallID.Dirt or WallID.GrassUnsafe || y > Main.worldSurface && y < Main.UnderworldLayer && t is TileID.Dirt or TileID.Stone or TileID.ClayBlock or TileID.WoodBlock or TileID.Granite)
		{
			if (Main.rand.NextBool()) //Generate a stack of 3 in a pyramid
			{
				if (!WorldMethods.AreaClear(x - 1, y - 3, 4, 4))
					return false;

				WorldGen.PlaceTile(x - 1, y, ModContent.TileType<StackablePots>(), true, style: GetRandomStyle());
				WorldGen.PlaceTile(x + 1, y, ModContent.TileType<StackablePots>(), true, style: GetRandomStyle());
				WorldGen.PlaceTile(x, y - 2, ModContent.TileType<StackablePots>(), true, style: GetRandomStyle());
			}
			else //Generate a stack of 2 in a tower
			{
				if (!WorldMethods.AreaClear(x, y - 5, 2, 4))
					return false;

				for (int s = 0; s < 2; s++)
					WorldGen.PlaceTile(x, y - s * 2, ModContent.TileType<StackablePots>(), true, style: GetRandomStyle());
			}

			return true;
		}

		return false;

		static int GetRandomStyle() => WorldGen.genRand.Next(12);
	}
}
