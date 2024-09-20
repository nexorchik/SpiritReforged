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
		var midDayColor = new Color(254, 194, 130);

		var finalColor = Color.Lerp(sunRiseSetColor, midDayColor, midDayFactor);

		//Make it slightly dimmer during the sunrise
		float sunRiseFactor = EaseFunction.EaseCircularOut.Ease((float)(Main.time / Main.dayLength));
		if (!Main.dayTime)
			sunRiseFactor = 1 - EaseFunction.EaseCircularOut.Ease((float)(Main.time / Main.nightLength));

		sunRiseFactor = MathHelper.Lerp(sunRiseFactor, 1, 0.7f);

		return finalColor * Math.Min(midDayFactor + sunRiseSetFactor, 1) * sunRiseFactor;
	}

	public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
	{
		if (maxDepth < float.MaxValue || minDepth > float.MaxValue)
			return;

		float dayProgress = Main.dayTime ? TimeProgress() : 0;
		dayProgress = EaseFunction.EaseQuadOut.Ease(dayProgress);
		Color skyColor = SavannaColor() * GetFadeOpacity();
		spriteBatch.Draw(TextureAssets.MagicPixel.Value,
			new Rectangle(0, 0, Main.screenWidth, Main.screenHeight),
			null,
			skyColor * dayProgress * 0.25f);

		int verticalOffset = (int)MathHelper.Lerp(-100, -240, dayProgress);
		spriteBatch.Draw(AssetLoader.LoadedTextures["SkyGradient"],
			new Rectangle(0, verticalOffset, Main.screenWidth, Main.screenHeight),
			null,
			Color.Lerp(skyColor, Color.White * GetFadeOpacity() * dayProgress, 0.3f), 0, Vector2.Zero, SpriteEffects.FlipVertically, 1f);

		if(Main.dayTime)
			SunMoonDraw.DrawSunFromSky();
	}

	public override Color OnTileColor(Color inColor) => Color.Lerp(inColor, SavannaColor(), 0.2f * GetFadeOpacity());

	public override float GetCloudAlpha() => 1f;

	internal override bool ActivationCondition(Player p) => p.InModBiome<SavannaBiome>();
}