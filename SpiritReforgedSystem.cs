using SpiritReforged.Common.Particle;

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