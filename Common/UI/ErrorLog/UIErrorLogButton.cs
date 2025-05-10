using SpiritReforged.Common.Misc;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace SpiritReforged.Common.UI.ErrorLog;

public class UIErrorLogButton : UIElement
{
	public UIErrorLogButton()
	{
		Width.Set(30, 0);
		Height.Set(30, 0);
	}

	protected override void DrawSelf(SpriteBatch spriteBatch)
	{
		var pos = GetDimensions().Center();

		if (IsMouseHovering)
		{
			int count = LogUtils.Logs.Count;
			UICommon.TooltipMouseText("[c/ffff00:" + $"{count} {(count == 1 ? "system" : "systems")} failed to initialize]\n" + "Click to open log");

			if (Main.mouseLeft && Main.mouseLeftRelease)
				UIInfoMessage.Instance.ShowMessage();
		}

		var texture = UICommon.ButtonErrorTexture.Value;
		spriteBatch.Draw(texture, pos, null, Color.White, 0, texture.Size() / 2, 1, default, 0);
	}
}