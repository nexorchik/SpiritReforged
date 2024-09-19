namespace SpiritReforged.Common.TileCommon.DrawPreviewHook;

public interface IDrawPreview
{
	public void DrawPreview(SpriteBatch spriteBatch, Terraria.DataStructures.TileObjectPreviewData data, Vector2 position);
}
