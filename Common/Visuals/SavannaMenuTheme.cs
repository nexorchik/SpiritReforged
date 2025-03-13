using Newtonsoft.Json.Linq;
using SpiritReforged.Common.PrimitiveRendering.PrimitiveShape;
using SpiritReforged.Common.PrimitiveRendering;
using SpiritReforged.Content.Savanna.Biome;
using System.Runtime.InteropServices;

namespace SpiritReforged.Common.Visuals;

internal class SavannaMenuTheme : ModMenu
{
	public override ModSurfaceBackgroundStyle MenuBackgroundStyle => ModContent.GetInstance<SavannaBGStyle>();
	public override string DisplayName => "Savanna";

	//Shader assets

	public static Asset<Texture2D> sTexture;
	public static Asset<Texture2D> sOutlineTexture; //The colored inner outline
	public static Asset<Texture2D> piritTexture;
	public static Asset<Texture2D> piritOutlineTexture;
	public static Asset<Texture2D> reforgedTexture;
	public static Asset<Texture2D> noiseTrail; //Triple trail texture that gets twisted into a vortex for the noise overlay on the letters

	public static Asset<Texture2D> pirtTexture;
	public static Asset<Texture2D> pirtOutlineTexture;

	public static float logoTime = 0f;
	public static float clickWobbleTimer = 0f;
	public static bool SpirtMod = false;

	public override void SetStaticDefaults() 
	{
		if (Main.dedServ)
			return;

		sTexture = AssetLoader.LoadedTextures["ModLogo/LogoS"];
		sOutlineTexture = AssetLoader.LoadedTextures["ModLogo/LogoSOutline"];
		piritTexture = AssetLoader.LoadedTextures["ModLogo/LogoPirit"];
		piritOutlineTexture = AssetLoader.LoadedTextures["ModLogo/LogoPiritOutline"];
		reforgedTexture = AssetLoader.LoadedTextures["ModLogo/LogoReforged"];
		noiseTrail = AssetLoader.LoadedTextures["ModLogo/TrailTri"];

		pirtTexture = AssetLoader.LoadedTextures["ModLogo/LogoPirt"];
		pirtOutlineTexture = AssetLoader.LoadedTextures["ModLogo/LogoPirtOutline"];
	}

	public override void OnSelected()
	{
		clickWobbleTimer = 0f;
		SpirtMod = Main.rand.NextBool(500); // Spirt mod
	}

	public override void Update(bool isOnTitleScreen)
	{
		float logoTimeMultiplier = 1f;
		if (clickWobbleTimer > 0f)
		{
			clickWobbleTimer -= (float)Main.gameTimeCache.ElapsedGameTime.TotalSeconds;
			if (clickWobbleTimer < 0f)
				clickWobbleTimer = 0f;
			logoTimeMultiplier += clickWobbleTimer * 8f;
		}

		if (!isOnTitleScreen)
			logoTime = 0;
		else
			logoTime += (float)Main.gameTimeCache.ElapsedGameTime.TotalSeconds * logoTimeMultiplier;
	}

	public static void StyleSpiritLogoPearlescentBlue(Effect effect)
	{
		//Lots of colors, so much customizability... woah
		effect.Parameters["indigoOutline"].SetValue(new Color(45, 87, 255).ToVector4());           //Indigo outline for all letters
		effect.Parameters["darkUnderlineColor"].SetValue(new Vector4(0, 0, 0, 1f));                //Color of the 1 px offset underline of the letters
		effect.Parameters["shadowColor"].SetValue(new Vector4(0, 0, 0, 0.2f));                     //Color of the shadows below the letters

		effect.Parameters["sFillColor"].SetValue(new Color(255, 255, 255).ToVector4());            //Inside white fill for the S
		effect.Parameters["sGradientTopColor"].SetValue(new Color(98, 186, 255).ToVector4());      //Top-down gradient for the internal outline of the S
		effect.Parameters["sGradientBottomColor"].SetValue(new Color(255, 186, 230).ToVector4());
		effect.Parameters["indigoOutlineGlowing"].SetValue(new Color(62, 164, 255).ToVector4());   //Glowing color for the S outline as it grows

		effect.Parameters["fillColorBase"].SetValue(new Color(139, 230, 255).ToVector4());         // Base fill color for the pirit letters
		effect.Parameters["fillColorSecondary"].SetValue(new Color(221, 190, 208).ToVector4());    // Secondary fill color for the pirit letters, tinting the right side
		effect.Parameters["piritFillGlowColor"].SetValue(new Color(76, 128, 76).ToVector4());      // Additive color for the pirit letters, applied with the dithery noise
		effect.Parameters["piritOutlineBaseColor"].SetValue(new Color(192, 253, 255).ToVector4()); // Base color for the inner outline of the letters
		effect.Parameters["innerOutlineGlowColor"].SetValue(new Color(98, 38, 0).ToVector4());     // Additive color layered ontop of the inner outline. Affected by the dithery noise

		effect.Parameters["reforgedColorLeft"].SetValue(new Color(40, 110, 255).ToVector4());       // Colors for the L/R gradient on the reforged subtext
		effect.Parameters["reforgedColorRight"].SetValue(new Color(108, 255, 111).ToVector4());
	}

	public static void StyleSpiritLogoSavannaOrange(Effect effect)
	{
		//Lots of colors, so much customizability... woah
		effect.Parameters["indigoOutline"].SetValue(new Color(153, 46, 0).ToVector4());            //Orange letter outline (not so indigo now are we)
		effect.Parameters["darkUnderlineColor"].SetValue(new Vector4(0, 0, 0, 1f));                //Black underline and shadows as usual
		effect.Parameters["shadowColor"].SetValue(new Vector4(0, 0, 0, 0.2f));                     

		effect.Parameters["sFillColor"].SetValue(new Color(255, 255, 255).ToVector4());            //S is still white inside
		effect.Parameters["sGradientTopColor"].SetValue(new Color(255, 98, 212).ToVector4());      //Sunset colored gradient in pink and yellow
		effect.Parameters["sGradientBottomColor"].SetValue(new Color(242, 255, 186).ToVector4());
		effect.Parameters["indigoOutlineGlowing"].SetValue(new Color(215, 94, 0).ToVector4());     //Lighter orange outline

		effect.Parameters["fillColorBase"].SetValue(new Color(255, 173, 62).ToVector4());          // Summery orange for the left of the pirit
		effect.Parameters["fillColorSecondary"].SetValue(new Color(174, 255, 227).ToVector4());    // Cool pale blue for the right of the letters
		effect.Parameters["piritFillGlowColor"].SetValue(new Color(40, 51, 0).ToVector4());        // Dark green for the glow, but it layers well with the chosen colors
		effect.Parameters["piritOutlineBaseColor"].SetValue(new Color(255, 224, 159).ToVector4()); // Bright orange-yellow letter inner outline
		effect.Parameters["innerOutlineGlowColor"].SetValue(new Color(98, 38, 0).ToVector4());     // No change from the usual

		effect.Parameters["reforgedColorLeft"].SetValue(new Color(193, 149, 0).ToVector4());       // Yellowish color on the left
		effect.Parameters["reforgedColorRight"].SetValue(new Color(255, 68, 68).ToVector4());    // Pinkish red on the right
	}

	public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor)
	{
		Effect effect = AssetLoader.LoadedShaders["SpiritLogo"];
		effect.Parameters["time"].SetValue(logoTime);
		effect.Parameters["textureResolution"].SetValue(sTexture.Size());
		effect.Parameters["sCenterCoordinates"].SetValue(new Vector2(120, 120));
		effect.Parameters["wobbleMult"].SetValue(1 + MathF.Pow(clickWobbleTimer, 1.6f) * 3.5f);

		effect.Parameters["sTexture"].SetValue(sTexture.Value);
		effect.Parameters["sOutlineTexture"].SetValue(sOutlineTexture.Value);

		if (!SpirtMod)
		{
			effect.Parameters["piritTexture"].SetValue(piritTexture.Value);
			effect.Parameters["piritOutlineTexture"].SetValue(piritOutlineTexture.Value);
		}
		else
		{
			effect.Parameters["piritTexture"].SetValue(pirtTexture.Value);
			effect.Parameters["piritOutlineTexture"].SetValue(pirtOutlineTexture.Value);
		}

		effect.Parameters["reforgedTexture"].SetValue(reforgedTexture.Value);
		effect.Parameters["noiseTexture"].SetValue(noiseTrail.Value);

		StyleSpiritLogoPearlescentBlue(effect);
		//StyleSpiritLogoSavannaOrange(effect); //Pick whichever

		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);

		//logoScale *= .84f; //Scale is slightly too large by default

		logoDrawCenter.Y += 16f;

		Rectangle approximateLogoRectangle = new Rectangle((int)(logoDrawCenter.X - sTexture.Width() * logoScale * 0.5f), (int)(logoDrawCenter.Y - sTexture.Height() * logoScale * 0.5f), sTexture.Width(), sTexture.Height());
		if (approximateLogoRectangle.Contains(Main.MouseScreen.ToPoint()) && Main.mouseLeft && Main.mouseLeftRelease)
		{
			clickWobbleTimer = 1f;
		}

		var square = new SquarePrimitive
		{
			Color = Color.White,
			Height = sTexture.Height() * logoScale,
			Length = sTexture.Width() * logoScale,
			Position = logoDrawCenter,
			Rotation = 0f,
		};
		PrimitiveRenderer.DrawPrimitiveShape(square, effect);

		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);
		return false;
	}
}
