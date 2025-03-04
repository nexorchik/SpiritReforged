using SpiritReforged.Common.Misc;
using SpiritReforged.Common.Particle;

namespace SpiritReforged;

public class SpiritReforgedSystem : ModSystem
{
	public override void Load() => SpiritClassic.AddSystem(Mod);

	public override void PreUpdateItems()
	{
		if (Main.netMode != NetmodeID.Server)
		{
			AssetLoader.VertexTrailManager.UpdateTrails();
			ParticleHandler.UpdateAllParticles();
		}
	}
}