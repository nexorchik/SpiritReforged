using PathOfTerraria.Common.World.Generation;
using SpiritReforged.Common.WorldGeneration;
using SpiritReforged.Content.Savanna.Tiles;

namespace SpiritReforged.Content.Savanna;

internal static class BaobabGen
{
	public static void GenerateBaobab(int x, int y)
	{
		GenerateBase(x, y, out int minX, out int maxX, out int height);
		MineInterior(x - 1, y - WorldGen.genRand.Next(2, 5));
		CreateRoots(minX, maxX, y);
		CreateBranches(minX, maxX, y, height);
	}

	private static void CreateBranches(int minX, int maxX, int y, int height)
	{
		const float Repeats = 3f;

		y -= height - 2;

		for (int i = 0; i < Repeats; ++i)
		{
			int x = (int)MathHelper.Lerp(minX, maxX, i / (Repeats - 1));
			float xOff = WorldGen.genRand.NextFloat(0.5f, 3f) * (WorldGen.genRand.NextBool() ? -1 : 1);

			var points = Spline.CreateSpline([new Vector2(x, y),
				new Vector2(x + xOff, y - WorldGen.genRand.NextFloat(1, 3)),
				new Vector2(x + xOff * 1.5f, y - WorldGen.genRand.NextFloat(3, 6))], 4);
			PointToPointRunner.SingleTile(new(points), PointToPointRunner.PlaceTileClearSlope(ModContent.TileType<LivingBaobab>(), true));
			PointToPointRunner.SingleTile(new(points), (ref Vector2 position, ref Vector2 direction) =>
			{
				WorldGen.TileRunner((int)position.X, (int)position.Y, 8, 3, ModContent.TileType<LivingBaobabLeaf>(), true, 0, 0, true, false);
				WorldGen.SquareTileFrame((int)position.X, (int)position.Y);
			});
		}
	}

	private static void CreateRoots(int minX, int maxX, int y)
	{
		const float Repeats = 8f;

		for (int i = 0; i < Repeats; ++i)
		{
			int x = (int)MathHelper.Lerp(minX, maxX, i / (Repeats - 1));

			var points = Spline.CreateSpline([new Vector2(x, y),
				new Vector2(x + WorldGen.genRand.NextFloat(-2, 2), y + WorldGen.genRand.NextFloat(1, 4)),
				new Vector2(x + WorldGen.genRand.NextFloat(-6, 6), y + WorldGen.genRand.NextFloat(4, 9))], 8);
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

	private static void GenerateBase(int x, int y, out int minX, out int maxX, out int height)
	{
		height = WorldGen.genRand.Next(15, 21);
		(int leftCount, int rightCount) = (1, 1);
		(int leftAdd, int rightAdd) = (WorldGen.genRand.Next(2) - 1, WorldGen.genRand.Next(2) - 1);
		(int leftWidth, int rightWidth) = (WorldGen.genRand.Next(1, 4), WorldGen.genRand.Next(1, 4));

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
