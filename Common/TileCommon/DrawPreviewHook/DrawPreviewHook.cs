namespace SpiritReforged.Common.TileCommon.DrawPreviewHook;

public class DrawPreviewHook : ILoadable
{
	public static void ForceDrawPreview()
	{
		var offscreen = new Vector2(Main.offScreenRange);
		var obj = TileObject.objectPreview;

		TileObject.DrawPreview(Main.spriteBatch, obj, Main.Camera.UnscaledPosition - offscreen);
	}

	public void Load(Mod mod) => On_TileObject.DrawPreview += On_TileObject_DrawPreview;

	private void On_TileObject_DrawPreview(On_TileObject.orig_DrawPreview orig, SpriteBatch sb, Terraria.DataStructures.TileObjectPreviewData op, Vector2 position)
	{
		if (TileLoader.GetTile(op.Type) is IDrawPreview idp)
			idp.DrawPreview(sb, op, position);
		else
			orig(sb, op, position);
	}

	public void Unload() { }
}
