using SpiritReforged.Common.Easing;
using SpiritReforged.Common.Misc;
using SpiritReforged.Common.Visuals.Skies;

namespace SpiritReforged.Content.Savanna.Biome;

public class SavannaSky : AutoloadedSky
{
	internal override bool DrawUnderSun => true;
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
		var midDayColor = new Color(254, 194, 130);

		var finalColor = Color.Lerp(sunRiseSetColor, midDayColor, midDayFactor);

		//Make it slightly dimmer during the sunrise
		float sunRiseFactor = EaseFunction.EaseCircularOut.Ease((float)(Main.time / Main.dayLength));
		if (!Main.dayTime)
			sunRiseFactor = 1 - EaseFunction.EaseCircularOut.Ease((float)(Main.time / Main.nightLength));

		sunRiseFactor = MathHelper.Lerp(sunRiseFactor, 1, 0.7f);

		return finalColor * Math.Min(midDayFactor + sunRiseSetFactor, 1) * sunRiseFactor;
	}

	public override void DoDraw(SpriteBatch spriteBatch)
	{
		float dayProgress = Main.dayTime ? TimeProgress() : 0;
		dayProgress = EaseFunction.EaseQuadOut.Ease(dayProgress);
		Color skyColor = SavannaColor() * GetFadeOpacity();
		spriteBatch.Draw(TextureAssets.MagicPixel.Value,
			new Rectangle(0, 0, Main.screenWidth, Main.screenHeight),
			null,
			skyColor * dayProgress * GetFadeOpacity() * 0.4f);

		int verticalOffset = (int)MathHelper.Lerp(0, -100, Math.Min(dayProgress, 0.5f));
		spriteBatch.Draw(AssetLoader.LoadedTextures["SkyGradient"],
			new Rectangle(0, verticalOffset, Main.screenWidth, Main.screenHeight - verticalOffset),
			null,
			Color.Lerp(skyColor.Additive(150), Color.White * GetFadeOpacity() * dayProgress, 0.1f), 0, Vector2.Zero, SpriteEffects.FlipVertically, 1f);
	}

	public override Color OnTileColor(Color inColor) => Color.Lerp(inColor, SavannaColor(), 0.2f * GetFadeOpacity());

	public override float GetCloudAlpha() => 1f;

	internal override bool ActivationCondition(Player p) => p.InModBiome<SavannaBiome>();
}