using SpiritReforged.Common.Easing;
using SpiritReforged.Common.Visuals.Skies;

namespace SpiritReforged.Content.Savanna.Biome;

public class SavannaSky : AutoloadedSky
{
	private Texture2D _bgTexture;

	public override void OnLoad() => _bgTexture = TextureAssets.MagicPixel.Value;

	private static float TimeProgress()
	{
		if (Main.dayTime)
			return (float)Math.Sin(Math.PI * Main.time / Main.dayLength);

		else
			return (float)Math.Sin(Math.PI * Main.time / Main.nightLength);
	}

	private static Color SavannaColor()
	{
		float sunRiseSetFactor = 1 - TimeProgress();
		float midDayFactor = Main.dayTime ? TimeProgress() : 0;

		sunRiseSetFactor = Main.dayTime ? 
			EaseFunction.EaseQuadOut.Ease(sunRiseSetFactor) 
			: EaseFunction.EaseCircularIn.Ease(sunRiseSetFactor);

		var sunRiseSetColor = new Color(242, 89, 58);
		var midDayColor = new Color(227, 191, 130);

		var finalColor = Color.Lerp(sunRiseSetColor, midDayColor, midDayFactor);

		//Make it slightly dimmer during the sunrise
		float sunRiseFactor = EaseFunction.EaseCircularOut.Ease((float)(Main.time / Main.dayLength));
		if (!Main.dayTime)
			sunRiseFactor = 1 - EaseFunction.EaseCircularOut.Ease((float)(Main.time / Main.nightLength));

		sunRiseFactor = MathHelper.Lerp(sunRiseFactor, 1, 0.7f);

		return finalColor * (midDayFactor * 0.66f + sunRiseSetFactor * 0.33f) * sunRiseFactor;
	}

	public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
	{
		if (maxDepth >= float.MaxValue && minDepth < float.MaxValue)
		{
			spriteBatch.Draw(_bgTexture, 
				new Rectangle(0, Math.Max(0, (int)((Main.worldSurface * 16 - Main.screenPosition.Y - 900) * 0.1f) - Main.screenHeight / 3), Main.screenWidth, Main.screenHeight), 
				null,
				SavannaColor() * Math.Min(1f, (Main.screenPosition.Y - 800) / 1000 * GetFadeOpacity()),
				0, Vector2.Zero, SpriteEffects.FlipVertically, 1f);
		}
	}

	public override Color OnTileColor(Color inColor) => Color.Lerp(inColor, SavannaColor(), 0.2f * GetFadeOpacity());

	public override float GetCloudAlpha() => 1f;

	internal override bool ActivationCondition(Player p) => p.InModBiome<SavannaBiome>();
}