using SpiritReforged.Common.MathHelpers;
using SpiritReforged.Common.WorldGeneration;
using SpiritReforged.Content.Savanna.Tiles;
using System.Linq;

namespace SpiritReforged.Content.Savanna;

internal static class BaobabGen
{
	public static void GenerateBaobab(int x, int y)
	{
		GenerateBase(x, y, out int minX, out int maxX, out int height, out int topMinX, out int topMaxX);
		MineInterior(x - 1, y - WorldGen.genRand.Next(2, 5));
		CreateRoots(minX, maxX, y);
		CreateBranches(topMinX, topMaxX, y, height);
	}

	private static void CreateBranches(int minX, int maxX, int y, int height)
	{
		float repeats = WorldGen.genRand.Next(3, 6);
		int center = (int)((minX + maxX) / 2f);

		y -= height - 2;

		for (int i = 0; i < repeats; ++i)
		{
			float factor = i / (repeats - 1f);
			int x = (int)MathHelper.Lerp(minX, maxX, factor);
			float xOff = MathHelper.Lerp(-3, 3, factor);

			var points = GetBranchPositions(x, y, x > center ? -1 : 1, WorldGen.genRand.Next(1, 3), false);
			PointToPointRunner.SingleTile(new(points), PointToPointRunner.PlaceTileClearSlope(ModContent.TileType<LivingBaobab>(), true), false);

			Vector2 position = points.Last();
			WorldGen.TileRunner((int)position.X, (int)position.Y, 4, 3, ModContent.TileType<LivingBaobabLeaf>(), true, 0, 0, true, false);
			WorldGen.SquareTileFrame((int)position.X, (int)position.Y);
		}
	}

	private static Vector2[] GetBranchPositions(int x, int y, int dir, int size, bool down)
	{
		Point current = new Point(x, y);
		List<Vector2> points = [current.ToVector2()];
		bool hori = WorldGen.genRand.NextBool();
		int vDir = down ? 1 : -1;

		for (int i = 0; i < size; ++i)
		{
			if (hori)
				current.X += WorldGen.genRand.Next(1, 3) * dir;
			else
				current.Y += WorldGen.genRand.Next(2, 5) * vDir;

			hori = !hori;
			points.Add(current.ToVector2());
		}

		return [.. points];
	}

	private static void CreateRoots(int minX, int maxX, int y)
	{
		const float Repeats = 6f;

		int center = (int)((minX + maxX) / 2f);

		for (int i = 0; i < Repeats; ++i)
		{
			int x = (int)MathHelper.Lerp(minX, maxX, i / (Repeats - 1));
			int size = 4 - (int)Math.Abs(i - Repeats / 2);

			var points = Spline.CreateSpline(GetBranchPositions(x, y, x < center ? -1 : 1, size, true), 8);
			PointToPointRunner.SingleTile(new(points), PointToPointRunner.PlaceTileClearSlope(ModContent.TileType<LivingBaobab>(), true));
		}
	}

	private static void MineInterior(int x, int y)
	{
		for (int i = x; i < x + 2; ++i)
		{
			for (int j = y; j > y - 5; --j)
			{
				WorldGen.KillTile(i, j);
				WorldGen.PlaceLiquid(i, j, (byte)LiquidID.Water, 255);
			}
		}
	}

	private static void GenerateBase(int x, int y, out int minX, out int maxX, out int height, out int topMinX, out int topMinY)
	{
		height = WorldGen.genRand.Next(15, 21);
		(int leftCount, int rightCount) = (1, 1);
		(int leftAdd, int rightAdd) = (WorldGen.genRand.Next(2) - 1, WorldGen.genRand.Next(2) - 1);
		(int leftWidth, int rightWidth) = (WorldGen.genRand.Next(1, 4), WorldGen.genRand.Next(1, 4));

		topMinX = x - leftWidth;
		topMinY = x + rightWidth;
		minX = x;
		maxX = x;
		y -= height;

		for (int j = y; j < y + height; ++j)
		{
			for (int i = x - leftWidth; i < x + rightWidth; ++i)
			{
				WorldGen.PlaceTile(i, j, ModContent.TileType<LivingBaobab>(), true, true);

				if (i < minX)
					minX = i;

				if (i > maxX) 
					maxX = i;
			}

			leftCount--;
			rightCount--;

			if (leftCount <= 0)
				leftCount = ModifyWidth(ref leftAdd, ref leftWidth);

			if (rightCount <= 0)
				rightCount = ModifyWidth(ref rightAdd, ref rightWidth);
		}
	}

	private static int ModifyWidth(ref int widthAdditions, ref int width)
	{
		width++;
		int widthDenom = widthAdditions switch
		{
			<= 0 => ++widthAdditions,
			1 => widthAdditions + (!WorldGen.genRand.NextBool(6) ? 1 : 0),
			_ => WorldGen.genRand.NextBool(10) ? widthAdditions++ : 200
		};
		widthAdditions = widthDenom;

		if (widthDenom > 2)
			widthAdditions = 200;
		return widthDenom;
	}
}
