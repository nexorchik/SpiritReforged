using SpiritReforged.Common.Misc;

namespace SpiritReforged.Common.Visuals.CustomText;

internal class RainbowText : CustomText
{
	public override string Key => "<rainbow>";

	public override void Draw(Rectangle panel, string[] text, int numLines)
	{
		var color = Main.DiscoColor.Additive(Main.mouseTextColor);

		if (Main.SettingsEnabled_OpaqueBoxBehindTooltips)
		{
			color = Color.Lerp(color, Color.White, .2f);
			Utils.DrawInvBG(Main.spriteBatch, panel, new Color(23, 25, 81, 255) * 0.925f * 0.85f);
		}

		var textPosition = new Vector2(panel.X + 10, panel.Y + 5);

		for (int line = 0; line < numLines; line++)
			Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value, text[line], textPosition.X, textPosition.Y + line * 30, color, Color.Black, Vector2.Zero);
	}
}
