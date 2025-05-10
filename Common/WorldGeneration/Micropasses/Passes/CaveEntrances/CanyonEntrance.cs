using SpiritReforged.Common.WorldGeneration.Noise;
using Terraria.DataStructures;

namespace SpiritReforged.Common.WorldGeneration.Micropasses.Passes.CaveEntrances;

internal class CanyonEntrance : CaveEntrance
{
	public override CaveEntranceType Type => CaveEntranceType.Canyon;

	public override void Generate(int x, int y)
	{
		int dif = (int)Main.worldSurface - y;
		int missing = Math.Max(0, 80 - dif);
		int depth = Math.Max(80, dif);

		y -= missing;

		DigCavern(x, y, depth);
	}

	public void DigCavern(int x, int y, int depth)
	{
		FastNoiseLite diggingNoise = new(WorldGen._genRandSeed);
		diggingNoise.SetFrequency(0.01f);

		FastNoiseLite wallNoise = new(WorldGen._genRandSeed);
		wallNoise.SetFrequency(0.06f);

		for (int j = y; j < y + depth; j++) 
		{
			int useX = x - (int)(diggingNoise.GetNoise(x, j) * 24);
			int minDistance = (int)MathHelper.Lerp(8, 1, (j - y) / (float)depth);
			int leftEdge = Math.Max(1, (int)(diggingNoise.GetNoise(x + 1200, j) * 4) + minDistance);
			int rightEdge = Math.Max(1, (int)(diggingNoise.GetNoise(x + 2400, j) * 4) + minDistance);
			
			int wallLeftEdge = (int)(wallNoise.GetNoise(x + 1200, j + 400) * 4) + minDistance;
			int wallRightEdge = (int)(wallNoise.GetNoise(x + 2400, j + 400) * 4) + minDistance;

			for (int i = useX - leftEdge; i < useX + rightEdge; ++i)
			{
				Tile tile = Main.tile[i, j];
				tile.Clear(TileDataType.Tile);

				if (i > useX - wallLeftEdge && i < useX + wallRightEdge)
					tile.Clear(TileDataType.Wall);
				else if (tile.WallType > 0)
				{
					float noise = wallNoise.GetNoise(i, j);

					tile.WallType = noise switch
					{
						< -0.12f => WallID.Stone,
						< 0.32f => WallID.Dirt,
						_ => WallID.Grass
					};
				}
			}
		}
	}

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
		return true;
	}
}
