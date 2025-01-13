using Terraria.DataStructures;
using Terraria.GameContent.Drawing;

namespace SpiritReforged.Common.TileCommon.TileSway;

/// <summary> Sway drawing helper for wind and player interaction. Use <see cref="Style"/> for vanilla sway styles or <see cref="Physics"/> and <see cref="DrawSway"/> for custom drawing. </summary>
public interface ISwayTile
{
	/// <summary> The default sway physics style for this tile according to <see cref="TileDrawing.TileCounterType"/>. Defaults to -1 which enables custom drawing only. </summary>
	public int Style => -1;

	/// <summary> Add wind grid math here. Called once per multitile. </summary>
	public float Physics(Point16 topLeft)
	{
		var data = TileObjectData.GetTileData(Framing.GetTileSafely(topLeft));
		float rotation = Main.instance.TilesRenderer.GetWindCycle(topLeft.X, topLeft.Y, TileSwaySystem.Instance.SunflowerWindCounter);

		if (!WorldGen.InAPlaceWithWind(topLeft.X, topLeft.Y, data.Width, data.Height))
			rotation = 0f;

		return rotation + TileSwayHelper.GetHighestWindGridPushComplex(topLeft.X, topLeft.Y, data.Width, data.Height, 30, 2f, 1, true);
	}

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
