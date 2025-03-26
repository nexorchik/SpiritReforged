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
		progress.Message = Language.GetTextValue("Mods.SpiritReforged.Generation.Pots");

		int maxPots = (int)(Main.maxTilesX * Main.maxTilesY * 0.00025); //Normal weight is 0.0008
		int pots = 0;

		while (true)
		{
			int x = Main.rand.Next(20, Main.maxTilesX - 20);
			int y = Main.rand.Next((int)GenVars.worldSurfaceHigh, Main.maxTilesY - 20);

			WorldMethods.FindGround(x, ref y);

			if (!PickStyle(x, y, out int style))
			{
				if (!PickGeneric(x, y, out int caveStyle))
					continue;

				WorldGen.PlaceTile(x, y - 1, ModContent.TileType<CavePots>(), true, style: caveStyle);
			}
			else
				WorldGen.PlaceTile(x, y - 1, ModContent.TileType<BiomePots>(), true, style: style);

			if (++pots >= maxPots)
				break;
		}
	}

	/// <summary> Picks a relevant style for cave pots (<see cref="CavePots"/>). </summary>
	private static bool PickGeneric(int x, int y, out int style)
	{
		int tile = Main.tile[x, y].TileType;
		int wall = Main.tile[x, y - 1].WallType;

		style = -1;

		if (tile is TileID.Dirt or TileID.Stone || wall is WallID.Dirt)
		{
			bool fancyPot = WorldGen.genRand.NextBool(50);
			style = fancyPot ? WorldGen.genRand.Next(4, 7) : WorldGen.genRand.Next(4);
		}

		return style != -1;
	}

	/// <summary> Picks a relevant style for biome pots (<see cref="BiomePots"/>). </summary>
	private static bool PickStyle(int x, int y, out int style)
	{
		int tile = Main.tile[x, y].TileType;
		int wall = Main.tile[x, y - 1].WallType;

		style = -1;

		if (tile is TileID.SnowBlock or TileID.IceBlock or TileID.BreakableIce)
			style = GetRange(BiomePots.STYLE.ICE);
		else if (wall is WallID.Sandstone or WallID.HardenedSand)
			style = GetRange(BiomePots.STYLE.DESERT);
		else if (tile is TileID.JungleGrass)
			style = GetRange(BiomePots.STYLE.JUNGLE);
		else if (tile is TileID.CorruptGrass or TileID.Ebonstone or TileID.Demonite)
			style = GetRange(BiomePots.STYLE.CORRUPTION);
		else if (tile is TileID.CrimsonGrass or TileID.Crimstone or TileID.Crimtane)
			style = GetRange(BiomePots.STYLE.CRIMSON);
		else if (tile is TileID.Marble)
			style = GetRange(BiomePots.STYLE.MARBLE);

		if (y > Main.UnderworldLayer)
			style = GetRange(BiomePots.STYLE.HELL);
		else if (tile is TileID.BlueDungeonBrick or TileID.GreenDungeonBrick or TileID.PinkDungeonBrick || Main.wallDungeon[wall])
			style = GetRange(BiomePots.STYLE.DUNGEON);

		return style != -1;

		static int GetRange(BiomePots.STYLE value)
		{
			int v = (int)value * 3;
			return WorldGen.genRand.Next(v, v + 3);
		}
	}
}
