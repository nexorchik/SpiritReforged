using SpiritReforged.Common.Easing;
using SpiritReforged.Common.Particle;
using SpiritReforged.Common.PrimitiveRendering;
using SpiritReforged.Common.PrimitiveRendering.CustomTrails;
using SpiritReforged.Common.ProjectileCommon.Abstract;
using SpiritReforged.Content.Ocean.Items.Reefhunter.Particles;
using SpiritReforged.Content.Particles;
using System.IO;

namespace SpiritReforged.Content.Forest.BassSlapper;

class BassSlapperProj : BaseClubProj, IManualTrailProjectile
{
	private const int MAX_SLAMS = 3;

	private int _numSlams = 0;

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
			angleRangeMod = 1.1f;
			rotOffset = 0;
		}

		SwingTrailParameters parameters = new(AngleRange * angleRangeMod, -HoldAngle_Final + rotOffset, trailDist, trailWidth)
		{
			Color = Color.White,
			SecondaryColor = new(139, 140, 70),
			TrailLength = 0.5f,
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

	public override void AfterCollision()
	{
		if(_numSlams < MAX_SLAMS && FullCharge)
		{
			_lingerTimer--;
			float lingerProgress = _lingerTimer / (float)LingerTime;
			lingerProgress = 1 - lingerProgress;
			BaseRotation = MathHelper.Lerp(BaseRotation, HoldAngle_Final, EaseFunction.EaseCubicOut.Ease(lingerProgress) / 6f);

			if(lingerProgress >= 0.66f)
			{
				SetAIState(AIStates.SWINGING);
				ResetData();
				Charge = 1;
				Projectile.ResetLocalNPCHitImmunity();
				OnSwingStart();
			}

			return;
		}

		base.AfterCollision();
	}

	public override void OnSmash(Vector2 position)
	{
		++_numSlams;
		TrailManager.TryTrailKill(Projectile);
		Collision.HitTiles(Projectile.position, Vector2.UnitY, Projectile.width, Projectile.height);

		DustClouds(6);

		//placeholder water dust
		for(int i = 0; i < 10; i++)
		{
			float maxOffset = 35 * TotalScale;
			float offset = Main.rand.NextFloat(-maxOffset, maxOffset);
			Vector2 dustPos = Projectile.Bottom + Vector2.UnitX * offset;
			float velocity = MathHelper.Lerp(4, 0, EaseFunction.EaseCircularIn.Ease(Math.Abs(offset) / maxOffset));
			if (FullCharge)
				velocity *= 1.33f;

			ParticleHandler.SpawnParticle(new BubbleParticle(dustPos, velocity * -Vector2.UnitY, Main.rand.NextFloat(0.2f, 0.4f), Main.rand.Next(20, 40)));

			for(int j = 0; j < 2; j++)
				Dust.NewDustPerfect(dustPos + Main.rand.NextVector2Circular(4, 4), DustID.Water, velocity * -Vector2.UnitY * Main.rand.NextFloat(), Scale : Main.rand.NextFloat(2));
		}

		DoShockwaveCircle(Projectile.Bottom - Vector2.UnitY * 8, 220, MathHelper.PiOver2, 0.4f);
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

	internal override void SendExtraDataSafe(BinaryWriter writer) => writer.Write((short)_numSlams);
	internal override void ReceiveExtraDataSafe(BinaryReader reader) => _numSlams = reader.ReadInt16();
}
