namespace SpiritReforged.Common.TileCommon;

public static class TileExtensions
{
	public static Vector2 TileOffset => Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
	public static Vector2 DrawPosition(this ModTile tile, int i, int j, Vector2 off = default) => new Vector2(i, j) * 16 - Main.screenPosition - off + TileOffset;
}
