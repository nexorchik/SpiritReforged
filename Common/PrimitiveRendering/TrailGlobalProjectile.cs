namespace SpiritReforged.Common.PrimitiveRendering;

public class TrailGlobalProjectile : GlobalProjectile
{
	public override void OnKill(Projectile projectile, int timeLeft)
	{
		if (Main.netMode != NetmodeID.Server)
			TrailManager.TryTrailKill(projectile);
	}
}