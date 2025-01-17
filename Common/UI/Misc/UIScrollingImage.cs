using Terraria.UI;

namespace SpiritReforged.Common.UI.Misc;

public class UIScrollingImage : UIElement
{
	private readonly Asset<Texture2D> Border;
	private readonly Asset<Texture2D> Scrolling;
	private readonly float ScrollSpeed;

	private float _timer = 0;

	public UIScrollingImage(Asset<Texture2D> border, Asset<Texture2D> scrolling, float scrollSpeed)
	{
		Border = border;
		Scrolling = scrolling;
		ScrollSpeed = scrollSpeed;

		OverrideSamplerState = SamplerState.PointWrap;
	}

	protected override void DrawSelf(SpriteBatch spriteBatch)
	{
		_timer += ScrollSpeed;

		Vector2 pos = GetDimensions().Position();
		spriteBatch.Draw(Scrolling.Value, pos, new Rectangle((int)_timer, 0, 80, 80), Color.White);
		spriteBatch.Draw(Border.Value, pos, Color.White);
	}
}