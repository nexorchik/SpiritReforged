public class SpiritReforgedSystem : ModSystem
{
	public override void PreUpdateItems()
	{
		if (Main.netMode != NetmodeID.Server)
			SpiritReforgedLoadables.VertexTrailManager.UpdateTrails();
	}
}