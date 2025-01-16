using SpiritReforged.Common.Misc;

namespace SpiritReforged.Common.Visuals.CustomText;

internal class ColorfulText : CustomText
{
	private Color _color;

	public override string Key => "colorful";

	public override bool ParseParams(string parameters)
	{
		_color = default;

		if (parameters is null)
			return false;

		string[] values = parameters.Split(',');
		Color color = new();

		for (int i = 0; i < values.Length; i++)
		{
			if (!int.TryParse(values[i], out int type))
				return false;

			if (i == 0)
				color.R = (byte)type;
			else if (i == 1)
				color.G = (byte)type;
			else if (i == 2)
				color.B = (byte)type; //Doesn't modify Color.A
			else
				break;
		}

		_color = color;
		return true;
	}

	public override void Draw(Rectangle panel, string[] text, int numLines)
	{
		var color = ((_color == default) ? Main.DiscoColor : _color).Additive(Main.mouseTextColor);

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
