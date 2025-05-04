using SpiritReforged.Common.Easing;
using SpiritReforged.Common.Particle;
using SpiritReforged.Common.PrimitiveRendering;
using SpiritReforged.Common.PrimitiveRendering.CustomTrails;
using SpiritReforged.Common.ProjectileCommon.Abstract;
using SpiritReforged.Content.Particles;

namespace SpiritReforged.Content.Forest.BassSlapper;

class BassSlapperProj : BaseClubProj, IManualTrailProjectile
{
	public BassSlapperProj() : base(new Vector2(76, 84)) { }

	public override float WindupTimeRatio => 0.8f;

	public void DoTrailCreation(TrailManager tM)
	{
		float trailDist = 70 * MeleeSizeModifier;
		float trailWidth = 50 * MeleeSizeModifier;
		float angleRangeMod = 1f;
		float rotOffset = 0;

		if (FullCharge)
		{
			trailDist *= 1.1f;
			trailWidth *= 1.1f;
			angleRangeMod = 1.2f;
			rotOffset = -MathHelper.PiOver4 / 2;
		}

		SwingTrailParameters parameters = new(AngleRange * angleRangeMod, -HoldAngle_Final + rotOffset, trailDist, trailWidth)
		{
			Color = Color.White,
			SecondaryColor = new(139, 140, 70),
			TrailLength = 0.6f,
			Intensity = 0.75f,
		};

		tM.CreateCustomTrail(new SwingTrail(Projectile, parameters, GetSwingProgressStatic, SwingTrail.BasicSwingShaderParams));

		parameters = new(AngleRange * angleRangeMod, -HoldAngle_Final + rotOffset, trailDist * 0.8f, trailWidth)
		{
			Color = Color.White,
			SecondaryColor = Color.Red,
			TrailLength = 0.3f,
			Intensity = 1f,
		};

		tM.CreateCustomTrail(new SwingTrail(Projectile, parameters, GetSwingProgressStatic, SwingTrail.BasicSwingShaderParams));
	}

	public override void OnSwingStart() => TrailManager.ManualTrailSpawn(Projectile);

	public override void OnSmash(Vector2 position)
	{
		TrailManager.TryTrailKill(Projectile);
		Collision.HitTiles(Projectile.position, Vector2.UnitY, Projectile.width, Projectile.height);

		DustClouds(6);

		//placeholder water dust
		for(int i = 0; i < 25; i++)
		{
			float maxOffset = 35 * TotalScale;
			float offset = Main.rand.NextFloat(-maxOffset, maxOffset);
			Vector2 dustPos = Projectile.Bottom + Vector2.UnitX * Main.rand.NextFloat(-30, 30);
			float velocity = MathHelper.Lerp(6, 2, EaseFunction.EaseCircularOut.Ease(Math.Abs(offset) / maxOffset)) * Main.rand.NextFloat(0.9f, 1.1f);
			if (FullCharge)
				velocity *= 1.33f;

			Dust.NewDustPerfect(dustPos, DustID.Water, -Vector2.UnitY * velocity, 0, Color.White, Scale: Main.rand.NextFloat(0.5f, 2f));
		}

		if (FullCharge)
		{
			float angle = MathHelper.PiOver4 * 1.5f;
			if (Projectile.direction > 0)
				angle = -angle + MathHelper.Pi;

			DoShockwaveCircle(Vector2.Lerp(Projectile.Center, Owner.Center, 0.5f), 280, angle, 0.4f);
		}

		DoShockwaveCircle(Projectile.Bottom - Vector2.UnitY * 8, 180, MathHelper.PiOver2, 0.4f);
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		if (Main.dedServ)
			return;

		var basePosition = Vector2.Lerp(Projectile.Center, target.Center, 0.6f);
		Vector2 directionUnit = basePosition.DirectionFrom(Owner.MountedCenter) * TotalScale;

		int numParticles = FullCharge ? 24 : 16;
		for (int i = 0; i < numParticles; i++)
		{
			float maxOffset = 15;
			float offset = Main.rand.NextFloat(-maxOffset, maxOffset);
			Vector2 position = basePosition + directionUnit.RotatedBy(MathHelper.PiOver2) * offset;
			float velocity = MathHelper.Lerp(12, 2, Math.Abs(offset) / maxOffset) * Main.rand.NextFloat(0.9f, 1.1f);
			if (FullCharge)
				velocity *= 1.5f;

			float rotationOffset = MathHelper.PiOver4 * offset / maxOffset;
			rotationOffset *= Main.rand.NextFloat(0.9f, 1.1f);

			Vector2 particleVel = directionUnit.RotatedBy(rotationOffset) * velocity;
			var p = new ImpactLine(position, particleVel, Color.White * 0.5f, new Vector2(0.15f, 0.6f) * TotalScale, Main.rand.Next(15, 20), 0.8f);
			p.UseLightColor = true;
			ParticleHandler.SpawnParticle(p);
		}
	}
}
