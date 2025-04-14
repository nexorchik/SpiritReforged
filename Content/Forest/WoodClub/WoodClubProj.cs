using SpiritReforged.Common.Misc;
using SpiritReforged.Common.PrimitiveRendering;
using SpiritReforged.Common.PrimitiveRendering.CustomTrails;
using SpiritReforged.Common.ProjectileCommon.Abstract;

namespace SpiritReforged.Content.Forest.WoodClub;

class WoodClubProj : BaseClubProj, IManualTrailProjectile
{
	public WoodClubProj() : base(new Vector2(58)) { }

	public override float WindupTimeRatio => 0.8f;
	 
	public void DoTrailCreation(TrailManager tM)
	{
		float trailDist = 52;
		float trailWidth = 40;
		tM.CreateCustomTrail(new SwingTrail(Projectile, Color.Beige, 3, AngleRange, -HoldAngle_Final, trailDist, trailWidth, GetSwingProgressStatic, SwingTrail.BasicSwingShaderParams));

		if (FullCharge)
			tM.CreateCustomTrail(new SwingTrail(Projectile, Color.Beige, 2, AngleRange * 1.3f, -HoldAngle_Final, 1.2f * trailDist, trailWidth * 1.2f, GetSwingProgressStatic, SwingTrail.BasicSwingShaderParams));
	}

	public override void OnSwingStart() => TrailManager.ManualTrailSpawn(Projectile);

	public override void OnSmash(Vector2 position)
	{
		TrailManager.TryTrailKill(Projectile);
		Collision.HitTiles(Projectile.position, Vector2.UnitY, Projectile.width, Projectile.height);

		DustClouds(8);

		if(FullCharge)
		{
			float angle = MathHelper.PiOver4 * 1.5f;
			if (Projectile.direction > 0)
				angle = -angle + MathHelper.Pi;

			DoShockwaveCircle(Vector2.Lerp(Projectile.Center, Main.player[Projectile.owner].Center, 0.5f), 280, angle, 0.4f);
		}

		DoShockwaveCircle(Projectile.Bottom - Vector2.UnitY * 8, 180, MathHelper.PiOver2, 0.4f);
	}
}
