using SpiritReforged.Common.WorldGeneration.Noise;
using Terraria.DataStructures;
using Terraria.WorldBuilding;

namespace SpiritReforged.Common.WorldGeneration.Micropasses.Passes.CaveEntrances;

internal class CanyonEntrance : CaveEntrance
{
	public override CaveEntranceType Type => CaveEntranceType.Canyon;

	public override void Generate(int x, int y)
	{
		int dif = (int)Main.worldSurface - y;
		int missing = Math.Max(0, 120 - dif);
		int depth = Math.Max(80, dif);

		y -= missing;

		if (missing > 0)
		{
			int tileY = WorldMethods.FindGround(x, y);
			Tile checkType = Main.tile[x, tileY];

			CreateMound(x, y, missing + 10, checkType);
		}

		DigCavern(x, y, depth);
	}

	private static void CreateMound(int x, int y, int depth, Tile referenceTile)
	{
		ushort type = referenceTile.TileType switch
		{
			TileID.Mud or TileID.JungleGrass => TileID.Mud,
			TileID.SnowBlock or TileID.IceBlock => TileID.SnowBlock,
			TileID.Sand or TileID.Sandstone or TileID.HardenedSand => TileID.Sand,
			_ => TileID.Dirt,
		};

		var mound = new Shapes.Mound(WorldGen.genRand.Next(20, 30), depth);
		WorldUtils.Gen(new Point(x, y + depth), mound, Actions.Chain(new Modifiers.Blotches(), new Actions.PlaceTile(type)));
	}

	public static void DigCavern(int x, int y, int depth)
	{
		int tileY = WorldMethods.FindGround(x, y);
		Tile checkType = Main.tile[x, tileY];

		(ushort wallDirt, ushort wallStone) = checkType.TileType switch
		{
			TileID.Mud or TileID.JungleGrass => (WallID.MudUnsafe, GetJungleWallVariant()),
			TileID.SnowBlock or TileID.IceBlock => (WallID.SnowWallUnsafe, WallID.IceUnsafe),
			TileID.Sand or TileID.Sandstone or TileID.HardenedSand => (WallID.HardenedSand, WallID.HardenedSand),
			_ => (WallID.DirtUnsafe, WallID.GrassUnsafe),
		};

		FastNoiseLite diggingNoise = new(WorldGen._genRandSeed);
		diggingNoise.SetFrequency(0.01f);

		FastNoiseLite windingNoise = new(WorldGen._genRandSeed);
		windingNoise.SetFrequency(0.007f);

		FastNoiseLite wallNoise = new(WorldGen._genRandSeed);
		wallNoise.SetFrequency(0.04f);

		for (int j = y; j < y + depth; j++) 
		{
			int useX = x - (int)(windingNoise.GetNoise(x, j) * 8);
			int minDistance = (int)MathHelper.Lerp(8, 1, (j - y) / (float)depth);
			int leftEdge = Math.Max(2, (int)(diggingNoise.GetNoise(x + 1200, j) * 4) + minDistance);
			int rightEdge = Math.Max(1, (int)(diggingNoise.GetNoise(x + 2400, j) * 4) + minDistance);
			
			int wallLeftEdge = (int)(wallNoise.GetNoise(x + 1200, j + 400) * 6) + minDistance * 2;
			int wallRightEdge = (int)(wallNoise.GetNoise(x + 2400, j + 400) * 6) + minDistance * 2;

			for (int i = useX - leftEdge * 2; i < useX + rightEdge * 2; ++i)
			{
				Tile tile = Main.tile[i, j];

				// Ice and snow are in the can't be cleared set because they're dumb, ignore that
				bool canClear = TileID.Sets.CanBeClearedDuringGeneration[tile.TileType] || tile.TileType is TileID.SnowBlock or TileID.IceBlock;

				if (canClear && i >= useX + leftEdge && i <= useX - rightEdge)
					tile.Clear(TileDataType.Tile);

				if (i > useX - wallLeftEdge && i < useX + wallRightEdge)
					tile.Clear(TileDataType.Wall);
				else
				{
					float noise = wallNoise.GetNoise(i, j);

					tile.WallType = noise switch
					{
						< 0.12f => wallStone,
						_ => wallDirt
					};
				}
			}
		}
	}

	private static ushort GetJungleWallVariant() => WorldGen.genRand.Next(4) switch
	{
		0 => WallID.JungleUnsafe1,
		1 => WallID.JungleUnsafe2,
		2 => WallID.JungleUnsafe3,
		_ => WallID.JungleUnsafe4
	};

	/// <summary>
	/// This makes for a nice shape so I'm keeping it for future reference.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="depth"></param>
	public void WindingCavern(int x, int y, int depth)
	{
		FastNoiseLite diggingNoise = new(WorldGen._genRandSeed);
		diggingNoise.SetFrequency(0.1f);

		FastNoiseLite windingNoise = new(WorldGen._genRandSeed);
		windingNoise.SetFrequency(0.03f);

		for (int j = y; j < y + depth; j++)
		{
			int useX = x - (int)(windingNoise.GetNoise(x, j) * 24);
			int minDistance = (int)MathHelper.Lerp(8, 1, (j - y) / (float)depth);
			int leftEdge = (int)(diggingNoise.GetNoise(x + 1200, j) * 4) + minDistance;
			int rightEdge = (int)(diggingNoise.GetNoise(x + 2400, j) * 4) + minDistance;

			for (int i = useX - leftEdge; i < useX + rightEdge; ++i)
			{
				Tile tile = Main.tile[i, j];

				tile.ClearEverything();
			}
		}
	}

	public override bool ModifyOpening(ref int x, ref int y, bool isCavinator)
	{
		if (isCavinator)
			return false;

		Generate(x, y); // Replace to KILL the ug desert (and all other structures that may impede)
		y = (int)Main.worldSurface - WorldGen.genRand.Next(5, 30);
		return true;
	}
}
