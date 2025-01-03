using SpiritReforged.Common.Particle;

namespace SpiritReforged;

public class SpiritReforgedSystem : ModSystem
{
	public override void PreUpdateItems()
	{
		if (Main.netMode != NetmodeID.Server)
		{
			AssetLoader.VertexTrailManager.UpdateTrails();
			ParticleHandler.UpdateAllParticles();
		}
	}
}