namespace SpiritReforged.Common.TileCommon;

internal static class TreeExtensions
{
	public static Vector2 GetRandomTreePosition(this ModTree tree, Tile tile)
	{
		var size = GetTreeSize(tree, tile);
		var halfSize = size / 2f;
		var offset = new Vector2(Main.rand.NextFloat(-halfSize.X, halfSize.X), -Main.rand.NextFloat(size.Y * 0.1f, size.Y * 0.8f));
		return offset;
	}

	public static Vector2 GetTreeSize(this ModTree tree, Tile tile)
	{
		int _ = 0;
		int width = 0;
		int height = 0;
		tree.SetTreeFoliageSettings(tile, ref _, ref _, ref _, ref width, ref height);
		return new Vector2(width, height);
	}
}
