using static Terraria.Main;
using static SpiritReforged.Common.Visuals.Skies.SunMoonILEdit;

namespace SpiritReforged.Common.Visuals.Skies;

public static class CustomDrawSunHelpers
{
	/// <summary>
	/// Redraws the vanilla sun, using the draw data retrieved directly from <see cref="IL_Main.DrawSunAndMoon"/>
	/// </summary>
	/// <param name="opacity">The transparency applied to the sun.</param>
	/// <param name="resetSpritebatch">Whether this should automatically restart the spritebatch to get the correct parameters, before and after drawing the sun.<para/>
	/// Uses <see cref="ResetToSunMoonParams(SpriteBatch)"/> and <see cref="ResetWithBGMatrixFix(SpriteBatch)"/>.<para/>
	/// Assumes this is being drawn by a custom sky or background.</param>
	public static void DrawSunFromSky(float opacity = 1, bool resetSpritebatch = false)
	{
		if (resetSpritebatch)
			spriteBatch.ResetToSunMoonParams();

		Texture2D sunTex = TextureAssets.Sun.Value;
		if (eclipse)
			sunTex = TextureAssets.Sun3.Value;
		else if (!gameMenu && player[myPlayer].head == 12)
			sunTex = TextureAssets.Sun2.Value;

		spriteBatch.Draw(sunTex, SunDrawData.Position, null, SunDrawData.Color, SunMoonData.Rotation, sunTex.Size() / 2, SunDrawData.Scale, 0, 0);
		spriteBatch.Draw(sunTex, SunDrawData.Position, null, SunDrawData.SecondaryColor, SunMoonData.Rotation, sunTex.Size() / 2, SunDrawData.Scale, 0, 0);

		if (resetSpritebatch)
			spriteBatch.ResetWithBGMatrixFix();
	}

	/// <summary>
	/// Resets the spritebatch, and applies the vanilla parameters used to draw the sun/moon.
	/// </summary>
	/// <param name="spriteBatch"></param>
	public static void ResetToSunMoonParams(this SpriteBatch spriteBatch)
	{
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, DefaultSamplerState, DepthStencilState.None, Rasterizer, null, BackgroundViewMatrix.EffectMatrix);
	}

	/// <summary>
	/// Resets the spritebatch, and applies the vanilla parameters to draw backgrounds and custom skies.
	/// </summary>
	/// <param name="spriteBatch"></param>
	public static void ResetWithBGMatrixFix(this SpriteBatch spriteBatch)
	{
		spriteBatch.End();
		Matrix transformationMatrix = BackgroundViewMatrix.TransformationMatrix;
		transformationMatrix.Translation -= BackgroundViewMatrix.ZoomMatrix.Translation * new Vector3(1f, BackgroundViewMatrix.Effects.HasFlag(SpriteEffects.FlipVertically) ? -1f : 1f, 1f);
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, transformationMatrix);
	}
}