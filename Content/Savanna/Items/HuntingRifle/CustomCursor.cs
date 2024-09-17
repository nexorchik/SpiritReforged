namespace SpiritReforged.Content.Savanna.Items.HuntingRifle;

public class CustomCursor : ILoadable
{
	private static Asset<Texture2D> CursorTexture;
	private static float opacity;

	public void Load(Mod mod)
	{
		if (!Main.dedServ)
			CursorTexture = mod.Assets.Request<Texture2D>("Content/Savanna/Items/HuntingRifle/Cursor_Reticle");

		On_Main.DrawCursor += DrawCursor;
		On_Main.DrawThickCursor += DrawThickCursor;
	}

	public void Unload() { }

	private Vector2 DrawThickCursor(On_Main.orig_DrawThickCursor orig, bool smart)
	{
		DrawCustomCursor(true);
		return orig(smart);
	}

	private void DrawCursor(On_Main.orig_DrawCursor orig, Vector2 bonus, bool smart)
	{
		DrawCustomCursor(false);
		orig(bonus, smart);
	}

	private static bool DrawCustomCursor(bool thick)
	{
		if (Main.LocalPlayer.mouseInterface || Main.LocalPlayer.HeldItem == null || Main.LocalPlayer.HeldItem.type != ModContent.ItemType<HuntingRifle>())
		{
			opacity = 0;
			return false;
		}

		float scale = Main.cursorScale * .6f * (thick ? 1.1f : 1f);
		float distance = MathHelper.Clamp(Main.LocalPlayer.Distance(Main.MouseWorld) / (16 * 50), 0, 1);
		float offsetLength = 5f + (1f - distance) * 5f 
			+ (float)Main.LocalPlayer.itemAnimation / Main.LocalPlayer.itemAnimationMax * 3f
			+ MathHelper.Min(Main.LocalPlayer.velocity.Length(), 2);
		//Distance, item animation, and player velocity adjustments

		opacity = MathHelper.Min(opacity + .05f, 1f);
		Color color = Main.cursorColor;

		if (thick) //Border cursor color
			color = Main.MouseBorderColor;
		else if (!Main.gameMenu && Main.LocalPlayer.hasRainbowCursor) //Rainbow cursor color
			color = Main.hslToRgb(Main.GlobalTimeWrappedHourly * 0.25f % 1f, 1f, 0.5f);

		for (int c = 0; c < 8; c++)
		{
			var frame = CursorTexture.Frame(2, 2, 0, thick ? 1 : 0, -2, -2);
			var origin = frame.Size() / 2;
			float rotation = MathHelper.PiOver2 * c;
			var position = Main.MouseScreen + new Vector2(1, -1).RotatedBy(rotation) * offsetLength;

			if (c > 3)
			{
				frame = CursorTexture.Frame(2, 2, 1, thick ? 1 : 0, -2, -2);
				position = Main.MouseScreen + new Vector2(1, 0).RotatedBy(rotation) * (offsetLength + 4);
			}

			if (!thick)
			{
				var shadowColor = color.MultiplyRGB(new Color(100, 100, 100)) * .25f;
				Main.spriteBatch.Draw(CursorTexture.Value, position + new Vector2(2), frame, shadowColor, rotation, origin, scale, SpriteEffects.None, 0f);
			}

			Main.spriteBatch.Draw(CursorTexture.Value, position, frame, color * opacity, rotation, origin, scale, SpriteEffects.None, 0f);
		}

		return true;
	}
}
