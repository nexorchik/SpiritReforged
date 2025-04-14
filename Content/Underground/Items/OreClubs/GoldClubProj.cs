using SpiritReforged.Common.Misc;
using SpiritReforged.Common.PrimitiveRendering;
using SpiritReforged.Common.PrimitiveRendering.CustomTrails;
using SpiritReforged.Common.ProjectileCommon.Abstract;

namespace SpiritReforged.Content.Underground.Items.OreClubs;

class GoldClubProj : BaseClubProj, IManualTrailProjectile
{
	public GoldClubProj() : base(new Vector2(82)) { }

	public override float WindupTimeRatio => 0.8f;

	public void DoTrailCreation(TrailManager tM)
	{
		float trailDist = 72;
		float trailWidth = 60;
		tM.CreateCustomTrail(new SwingTrail(Projectile, new Color(227, 197, 105), 2, AngleRange, -HoldAngle_Final, trailDist, trailWidth, GetSwingProgressStatic, SwingTrail.BasicSwingShaderParams));
		if (FullCharge)
			tM.CreateCustomTrail(new SwingTrail(Projectile, Color.LightGoldenrodYellow, 2, AngleRange * 1.3f, -HoldAngle_Final, 1.1f * trailDist, trailWidth, GetSwingProgressStatic, SwingTrail.BasicSwingShaderParams));

		tM.CreateCustomTrail(new SwingTrail(Projectile, new Color(216, 13, 13, 160), 3, AngleRange, -HoldAngle_Final, trailDist, trailWidth / 2f, GetSwingProgressStatic, SwingTrail.BasicSwingShaderParams));
	}

	public override void OnSwingStart() => TrailManager.ManualTrailSpawn(Projectile);

	public override void OnSmash(Vector2 position)
	{
		TrailManager.TryTrailKill(Projectile);
		Collision.HitTiles(Projectile.position, Vector2.UnitY, Projectile.width, Projectile.height);

		DustClouds(12);

		if (FullCharge)
		{
			float angle = MathHelper.PiOver4 * 1.5f;
			if (Projectile.direction > 0)
				angle = -angle + MathHelper.Pi;

			DoShockwaveCircle(Vector2.Lerp(Projectile.Center, Main.player[Projectile.owner].Center, 0.5f), 380, angle, 0.4f);
		}

		DoShockwaveCircle(Projectile.Bottom - Vector2.UnitY * 8, 240, MathHelper.PiOver2, 0.4f);
	}
}
