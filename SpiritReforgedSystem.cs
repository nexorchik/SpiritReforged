using SpiritReforged.Common.Particle;
using System.IO;

namespace SpiritReforged;

public class SpiritReforgedSystem : ModSystem
{
	private static int StaticWorldSeed = -1;

	internal static int GetWorldSeed()
	{
		if (StaticWorldSeed > -1)
			return StaticWorldSeed;

		int seed = -1;
		if (WorldGen.currentWorldSeed is not null && !int.TryParse(WorldGen.currentWorldSeed, out seed))
			seed = ReLogic.Utilities.Crc32.Calculate(WorldGen.currentWorldSeed);

		return StaticWorldSeed = (seed == int.MinValue) ? int.MaxValue : Math.Abs(seed); //Parse the world seed and store it
	}

	public override void OnWorldUnload() => StaticWorldSeed = -1;
	public override void NetSend(BinaryWriter writer) => writer.Write(StaticWorldSeed);
	public override void NetReceive(BinaryReader reader) => StaticWorldSeed = reader.ReadInt32();

	public override void PreUpdateItems()
	{
		if (Main.netMode != NetmodeID.Server)
		{
			AssetLoader.VertexTrailManager.UpdateTrails();
			ParticleHandler.UpdateAllParticles();
		}
	}
}