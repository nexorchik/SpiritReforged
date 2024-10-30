namespace SpiritReforged.Content.Ocean;

public class OceanWaterStyle : ModWaterStyle
{
	public override int ChooseWaterfallStyle() => 0;
	public override int GetSplashDust() => DustID.Water;
	public override int GetDropletGore() => GoreID.WaterDrip;

	public override void LightColorMultiplier(ref float r, ref float g, ref float b)
	{
		//r = 1.08f;
		//g = 1.08f;
		//b = 1.08f;
	}

	public override Color BiomeHairColor() => Color.DeepSkyBlue;
	public override byte GetRainVariant() => (byte)Main.rand.Next(3);
	public override Asset<Texture2D> GetRainTexture() => ModContent.Request<Texture2D>("SpiritReforged/Content/Ocean/OceanRain");
}