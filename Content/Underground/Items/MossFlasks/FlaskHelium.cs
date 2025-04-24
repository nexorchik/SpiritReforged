using SpiritReforged.Common.Visuals.Glowmasks;

namespace SpiritReforged.Content.Underground.Items.MossFlasks;

[AutoloadGlowmask("Method:Content.Underground.Items.MossFlasks.FlaskHelium Glow")]
public class FlaskHelium : MossFlask
{
	public static Color Glow(object obj) => Main.DiscoColor;

	public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
	{
		var glow = GlowmaskItem.ItemIdToGlowmask[Type].Glowmask.Value;
		spriteBatch.Draw(glow, position, frame, Item.GetAlpha(Glow(null)), 0, origin, scale, default, 0);
	}
}

public class FlaskHeliumProjectile : MossFlaskProjectile
{
	public override (ushort, ushort) Types => (TileID.RainbowMoss, TileID.RainbowMossBrick);
}