using Terraria.DataStructures;

namespace SpiritReforged.Common.WorldGeneration;

public struct TileCondition(bool active, ushort tileID, short frameX, short frameY, Point16 pos)
{
	internal bool Active = active;
	internal ushort TileId = tileID;
	internal short FrameX = frameX;
	internal short FrameY = frameY;
	internal Point16 Position = pos;

	public readonly bool HasChanged()
	{
		Tile tile = Main.tile[Position.ToPoint()];
		bool isDifferent = tile.HasTile != Active || tile.TileType != TileId || tile.TileFrameX != FrameX || tile.TileFrameY != FrameY;
		return isDifferent;
	}

	public static List<TileCondition> GetTileSquare(int i, int j)
	{
		List<TileCondition> conditions = [];

		for (int x = i - 1; x < i + 2; ++x)
			for (int y = j - 1; y < j + 2; ++y)
				conditions.Add(FromTile(x, y));

		return conditions;
	}

	public static List<TileCondition> GetArea(int i, int j, int width, int height)
	{
		List<TileCondition> conditions = [];

		for (int x = i; x < i + width; ++x)
			for (int y = j; y < j + height; ++y)
				conditions.Add(FromTile(x, y));

		return conditions;
	}

	public static TileCondition FromTile(int i, int j)
	{
		Tile tile = Main.tile[i, j];
		TileCondition condition = new(tile.HasTile, tile.TileType, tile.TileFrameX, tile.TileFrameY, new Point16(i, j));
		return condition;
	}
}
