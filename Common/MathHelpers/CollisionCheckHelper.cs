namespace SpiritReforged.Common.MathHelpers;

public static class CollisionCheckHelper
{
	public static bool CheckSolidTilesAndPlatforms(Rectangle range)
	{
		int startX = range.X;
		int endX = range.X + range.Width;

		int startY = range.Y;
		int endY = range.Y + range.Height;

		if (Collision.SolidTiles(startX, endX, startY, endY))
			return true;

		for (int x = startX; x <= endX; x++)
		{
			for (int y = startY; y <= endY; y++)
			{
				if (TileID.Sets.Platforms[Framing.GetTileSafely(new Point(x, y)).TileType])
					return true;
			}
		}

		return false;
	}
}