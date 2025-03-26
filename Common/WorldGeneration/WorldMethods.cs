using System.Linq;
using Terraria.DataStructures;

namespace SpiritReforged.Common.WorldGeneration;

public class WorldMethods
{
	internal static int FindNearestBelow(int x, int y)
	{
		while (!WorldGen.SolidTile(x, y))
			y++;

		return y - 1;
	}

	internal static int FindNearestAbove(int x, int y)
	{
		while (!WorldGen.SolidTile(x, y))
		{
			y--;

			if (y < 10)
				return 10;
		}

		return y;
	}

	/// <summary> Scans up, then down for the nearest surface tile. </summary>
	public static int FindGround(int i, ref int j)
	{
		while (WorldGen.SolidOrSlopedTile(i, j - 1) && i > 20)
			j--; //Up

		while (!WorldGen.SolidOrSlopedTile(i, j) && j < Main.maxTilesY - 20)
			j++; //Down

		return j;
	}

	public static void CragSpike(int X, int Y, int length, int height, ushort type2, float slope, float sloperight)
	{
		float trueslope = 1 / slope;
		float truesloperight = 1 / sloperight;

		for (int level = 0; level <= height; level++)
		{
			Tile tile = Main.tile[X, (int)(Y + level - slope / 2)];
			tile.HasTile = true;
			Main.tile[X, (int)(Y + level - slope / 2)].TileType = type2;
			for (int I = X - (int)(length + level * trueslope); I < X + (int)(length + level * truesloperight); I++)
			{
				Tile tile2 = Main.tile[I, Y + level];
				tile2.HasTile = true;
				Main.tile[I, (Y + level)].TileType = type2;
			}
		}
	}

	public static void RoundHole(int X, int Y, int Xmult, int Ymult, int strength, bool initialdig)
	{
		if (initialdig)
			WorldGen.digTunnel(X, Y, 0, 0, strength, strength, false);

		for (int rotation2 = 0; rotation2 < 350; rotation2++)
		{
			int DistX = (int)(0 - Math.Sin(rotation2) * Xmult);
			int DistY = (int)(0 - Math.Cos(rotation2) * Ymult);

			WorldGen.digTunnel(X + DistX, Y + DistY, 0, 0, strength, strength, false);
		}
	}

	public static int AreaCount(int i, int j, int width, int height)
	{
		int count = 0; 

		for (int x = i; x < i + width; ++x)
		{
			for (int y = j; y < j + height; ++y)
			{
				Tile tile = Framing.GetTileSafely(x, y);
				if (tile.HasTile && Main.tileSolid[tile.TileType])
					count++;
			}
		}

		return count;
	}

	public static bool AreaClear(int i, int j, int width, int height) => AreaCount(i, j, width, height) == 0;

	/// <summary> Checks whether this tile area is completely submerged in water. </summary>
	public static bool Submerged(int i, int j, int width, int height)
	{
		for (int x = i; x < i + width; x++)
		{
			for (int y = j; y < j + height; y++)
			{
				var tile = Framing.GetTileSafely(x, y);
				if (tile.LiquidType != LiquidID.Water || tile.LiquidAmount < 255)
					return false;
			}
		}

		return true;
	}

	public static bool AdjacentOpening(int x, int y)
	{
		for (int i = x - 1; i < x + 2; ++i)
			for (int j = y - 1; j < y + 2; ++j)
				if (!WorldGen.SolidTile(i, j) && (i != x || j != y))
					return true;
		return false;
	}

	public static bool CloudsBelow(int x, int y, out int addY)
	{
		const int scanDistance = 30;
		HashSet<int> types = [TileID.Cloud, TileID.RainCloud, TileID.SnowCloud];

		for (int i = 0; i < scanDistance; i++)
			if (Main.tile[x, y + i].HasTile && types.Contains(Main.tile[x, y + i].TileType))
			{
				addY = scanDistance;
				return true;
			}

		addY = 0;
		return false;
	}

	public static bool IsFlat(Point16 position, int width, out int startY, out int endY, int maxDeviance = 2)
	{
		int maxSamples = (width * 2 + 1) / 4;
		List<int> samples = [];

		for (int i = 0; i < maxSamples; i++)
		{
			int x = (int)MathHelper.Lerp(position.X - width, position.X + width, (float)i / maxSamples);
			int y = position.Y;

			FindGround(x, ref y);
			samples.Add(y);
		}

		startY = samples[0];
		endY = samples[^1];

		int average = (int)samples.Average();
		int surfaceAverage = (int)Math.Abs(MathHelper.Lerp(startY, endY, .5f));

		return Math.Abs(startY - endY) <= maxDeviance && Math.Abs(average - surfaceAverage) <= maxDeviance;
	}
}