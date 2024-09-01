namespace SpiritReforged.Common.WorldGeneration;

internal class NoiseSystem : ModSystem
{
	private FastNoiseLite _noise;

	public override void Load() => _noise = new FastNoiseLite(Main.rand.Next());

	public static float Perlin(float x, float y) => ModContent.GetInstance<NoiseSystem>()._noise.GetNoise(x, y);
}
