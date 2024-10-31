using SpiritReforged.Common.Particle;

namespace SpiritReforged;

public class SpiritReforgedSystem : ModSystem
{
	private static int staticWorldSeed = -1;

	internal static int GetWorldSeed()
	{
		if (staticWorldSeed > -1)
			return staticWorldSeed;

		if (!int.TryParse(WorldGen.currentWorldSeed, out int seed))
			seed = ReLogic.Utilities.Crc32.Calculate(WorldGen.currentWorldSeed);

		return staticWorldSeed = (seed == int.MinValue) ? int.MaxValue : Math.Abs(seed); //Parse the world seed and store it
	}

	public override void OnWorldUnload() => staticWorldSeed = -1;

	public override void PreUpdateItems()
	{
		if (Main.netMode != NetmodeID.Server)
		{
			AssetLoader.VertexTrailManager.UpdateTrails();
			ParticleHandler.UpdateAllParticles();
		}
	}
}