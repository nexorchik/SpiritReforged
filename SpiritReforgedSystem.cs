using SpiritReforged.Common.Particle;

namespace SpiritReforged;

public class SpiritReforgedSystem : ModSystem
{
	internal static int StaticWorldSeed { get; private set; }

	public override void OnWorldLoad()
	{
		if (!int.TryParse(WorldGen.currentWorldSeed, out int seed))
			seed = ReLogic.Utilities.Crc32.Calculate(WorldGen.currentWorldSeed);

		StaticWorldSeed = (seed == int.MinValue) ? int.MaxValue : Math.Abs(seed); //Parse the world seed and store it on load
	}

	public override void PreUpdateItems()
	{
		if (Main.netMode != NetmodeID.Server)
		{
			AssetLoader.VertexTrailManager.UpdateTrails();
			ParticleHandler.UpdateAllParticles();
		}
	}
}