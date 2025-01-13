using Terraria.DataStructures;
using Terraria.GameContent.Drawing;

namespace SpiritReforged.Common.TileCommon.TileSway;

/// <summary> Assign <see cref="Style"/> for vanilla sway styles or use <see cref="DrawSway"/> for custom drawing.<br/>
/// <see cref="Physics"/> changes how the tile responds to wind and player interaction. </summary>
public interface ISwayTile
{
	/// <summary> The default sway physics style for this tile according to <see cref="TileDrawing.TileCounterType"/>. Defaults to -1 which enables custom drawing only. </summary>
	public int Style => -1;

	/// <summary> Add wind grid math here. Called once per multitile. </summary>
	public float Physics(Point16 topLeft) => 0;

	/// <summary> Draw this tile transformed by <see cref="Physics"/>. </summary>
	public void DrawSway(int i, int j, SpriteBatch spriteBatch, Vector2 offset, float rotation, Vector2 origin)
	{
		var tile = Framing.GetTileSafely(i, j);
		var data = TileObjectData.GetTileData(tile);

		var drawPos = new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y);
		var source = new Rectangle(tile.TileFrameX, tile.TileFrameY, data.CoordinateWidth, data.CoordinateHeights[tile.TileFrameY / 18]);

		spriteBatch.Draw(TextureAssets.Tile[tile.TileType].Value, drawPos + offset - new Vector2(0, 2),
			source, Lighting.GetColor(i, j), rotation, origin, 1, SpriteEffects.None, 0);
	}
}
