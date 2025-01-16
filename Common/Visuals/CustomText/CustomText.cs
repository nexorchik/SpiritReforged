namespace SpiritReforged.Common.Visuals.CustomText;

/// <summary> Used to override sign text drawing using the associated <see cref="Key"/> value. <br/>
/// Loaded by <see cref="CustomTextHandler"/>. </summary>
internal abstract class CustomText
{
	/// <summary> The beginning text key required for <see cref="Draw"/> to be called. </summary>
	public abstract string Key { get; }

	/// <summary> Enables optional customization based on the given string. </summary>
	/// <param name="parameters"> The text provided after <see cref="Key"/> and separated by ':'. Can be parsed in any way to change how the text behaves. </param>
	/// <returns> Whether the parameters were successfully parsed. </returns>
	public virtual bool ParseParams(string parameters) => false;

	public virtual void Draw(Rectangle panel, string[] text, int numLines)
	{
		var color = Main.MouseTextColorReal;

		if (Main.SettingsEnabled_OpaqueBoxBehindTooltips)
		{
			color = Color.Lerp(color, Color.White, 1f);
			Utils.DrawInvBG(Main.spriteBatch, panel, new Color(23, 25, 81, 255) * 0.925f * 0.85f);
		}

		var textPosition = new Vector2(panel.X + 10, panel.Y + 5);

		for (int line = 0; line < numLines; line++)
			Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value, text[line], textPosition.X, textPosition.Y + line * 30, color, Color.Black, Vector2.Zero);
	}
}
