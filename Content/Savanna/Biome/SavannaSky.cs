using SpiritReforged.Common.Easing;
using SpiritReforged.Common.Visuals.Skies;

namespace SpiritReforged.Content.Savanna.Biome;

public class SavannaSky : AutoloadedSky
{
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
		if (maxDepth < float.MaxValue || minDepth > float.MaxValue)
			return;

		spriteBatch.Draw(TextureAssets.MagicPixel.Value,
			new Rectangle(0, 0, Main.screenWidth, Main.screenHeight),
			null,
			SavannaColor() * Math.Min(1f, (Main.screenPosition.Y - 800) / 1000 * GetFadeOpacity()));
	}

	public override Color OnTileColor(Color inColor) => Color.Lerp(inColor, SavannaColor(), 0.2f * GetFadeOpacity());

	public override float GetCloudAlpha() => 1f;

	internal override bool ActivationCondition(Player p) => p.InModBiome<SavannaBiome>();
}