using SpiritReforged.Common.Misc;
using SpiritReforged.Common.Particle;
using SpiritReforged.Common.PrimitiveRendering;
using SpiritReforged.Common.PrimitiveRendering.CustomTrails;
using SpiritReforged.Common.ProjectileCommon.Abstract;
using SpiritReforged.Content.Particles;
using Terraria.Audio;
using static Microsoft.Xna.Framework.MathHelper;
using static SpiritReforged.Common.Easing.EaseFunction;

namespace SpiritReforged.Content.Underground.Items.OreClubs;

class GoldClubProj : BaseClubProj, IManualTrailProjectile
{
	private bool _inputHeld = false;

	private static Color LightGold => new(255, 249, 181);
	private static Color DarkGold => new(227, 197, 105);
	private static Color Ruby => new(216, 13, 13);

	public GoldClubProj() : base(new Vector2(82)) { }

	public int Direction { get; set; } = 1;

	public override float WindupTimeRatio => 0.8f;

	public override float HoldAngle_Intial => base.HoldAngle_Intial * 1.5f;
	public override float HoldAngle_Final => (Direction * base.HoldAngle_Final) - (Direction < 0 ? PiOver4 / 2 : 0);
	public override float SwingAngle_Max => (Direction * base.SwingAngle_Max * 1.1f) - (Direction < 0 ? PiOver2 : 0);
	public override float LingerTimeRatio => 1.5f;

	public void DoTrailCreation(TrailManager tM)
	{
		float trailDist = 78;
		float trailWidth = 25;
		float intensity = 3;
		float trailLengthMod = 1f;
		float rotation = HoldAngle_Final - PiOver4 / 2;
		if (Direction < 0)
			rotation += PiOver4;

		if (FullCharge)
		{
			trailWidth *= 1.6f;
			intensity *= 1.4f;
			trailLengthMod *= 1.6f;
		}

		tM.CreateCustomTrail(new SwingTrail(Projectile, LightGold, DarkGold, intensity, AngleRange, 0.3f * trailLengthMod, rotation, trailDist, trailWidth, GetSwingProgressStatic, s => SwingTrail.NoiseSwingShaderParams(s, "swirlNoise", new Vector2(2, 1)), TrailLayer.UnderProjectile, 0.85f));

		tM.CreateCustomTrail(new SwingTrail(Projectile, Color.Pink, Ruby, intensity, AngleRange, 0.35f * trailLengthMod, rotation, trailDist, trailWidth / 3, GetSwingProgressStatic, s => SwingTrail.NoiseSwingShaderParams(s, "noiseCrystal", new Vector2(3f, 0.5f)), TrailLayer.UnderProjectile, 0.85f));
	}

	public override void OnSwingStart() => TrailManager.ManualTrailSpawn(Projectile);

	internal override float ChargedScaleInterpolate(float progress) => (Direction == 1) ? base.ChargedScaleInterpolate(progress) : 1;

	internal override float ChargedRotationInterpolate(float progress) => (Direction == 1) ? base.ChargedRotationInterpolate(progress) : Lerp(WrapAngle(BaseRotation), WrapAngle(HoldAngle_Final), 0.1f);

	public override void Swinging(Player owner)
	{
		base.Swinging(owner);

		if (owner.controlUseItem && GetSwingProgress < SwingShrinkThreshold)
			_inputHeld = true;

		if(GetSwingProgress > SwingShrinkThreshold && Direction == 1 && _inputHeld)
		{
			SetAiState(AiStates.CHARGING);
			Direction = -1;
			ResetData();
			Projectile.ResetLocalNPCHitImmunity();
			TrailManager.TryTrailKill(Projectile);

			return;
		}
	}

	public override void OnSmash(Vector2 position)
	{
		TrailManager.TryTrailKill(Projectile);
		Collision.HitTiles(Projectile.position, Vector2.UnitY, Projectile.width, Projectile.height);

		DustClouds(12);

		if (FullCharge)
		{
			float angle = PiOver4 * 1.5f;
			if (Projectile.direction > 0)
				angle = -angle + Pi;

			DoShockwaveCircle(Vector2.Lerp(Projectile.Center, Owner.Center, 0.5f), 380, angle, 0.4f);
		}

		DoShockwaveCircle(Projectile.Bottom - Vector2.UnitY * 8, 240, PiOver2, 0.4f);
	}

	public override void AfterCollision()
	{
		const float shrinkThreshold = 0.7f;

		_lingerTimer--;
		float lingerProgress = _lingerTimer / (float)LingerTime;
		lingerProgress = 1 - lingerProgress;

		float shrinkProgress = (lingerProgress - shrinkThreshold) / (1 - shrinkThreshold);
		shrinkProgress = Clamp(shrinkProgress, 0, 1);

		if (Owner.controlUseItem && lingerProgress < shrinkThreshold)
			_inputHeld = true;

		if (_inputHeld && lingerProgress >= shrinkThreshold && Direction == 1)
		{
			SetAiState(AiStates.CHARGING);
			Direction = -1;
			ResetData();
			Projectile.ResetLocalNPCHitImmunity();

			return;
		}
		else
		{
			Projectile.scale = Lerp(1, 0, EaseCircularOut.Ease(shrinkProgress));

			if (_lingerTimer <= 0)
				Projectile.Kill();
		}

		BaseRotation = Lerp(BaseRotation, SwingAngle_Max, EaseQuadIn.Ease(lingerProgress) / 5f);
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		if (Direction < 0 && target.knockBackResist > 0)
		{
			if (target.gravity != 0)
				target.velocity.Y = -Projectile.knockBack * 0.75f;

			target.velocity.Y -= Projectile.knockBack * target.knockBackResist * 0.25f;
		}

		if (!Main.dedServ)
		{
			var direction = -Vector2.UnitY.RotatedByRandom(Pi / 8);

			var position = Vector2.Lerp(Projectile.Center, target.Center, 0.75f);
			SoundEngine.PlaySound(SoundID.Item70.WithVolumeScale(0.5f), position);

			float width = 260;

			var pos = position + direction;
			float rotation = direction.ToRotation();

			var p = new TexturedPulseCircle(pos, Ruby, Color.LightPink, 0.6f, width, Main.rand.Next(20, 25), "Star2", new Vector2(2, 1), EaseCircularOut, false, 0.2f).WithSkew(.5f, rotation);
			ParticleHandler.SpawnParticle(p);

			float shineRotation = Main.rand.NextFloatDirection();
			for(int i = 0; i < 3; i++)
				ParticleHandler.SpawnParticle(new DissipatingImage(pos, DarkGold, shineRotation, 0.12f, 0, "GodrayCircle", new Vector2(0), new Vector2(3, 1.5f), 18));

			ParticleHandler.SpawnParticle(new Shatter(position, DarkGold, 40));

			float numLines = 16;
			for (int i = 0; i < numLines; i++)
			{
				Vector2 velocity = Vector2.UnitX.RotatedBy(TwoPi * i / numLines);
				velocity = velocity.RotatedByRandom(PiOver4);
				velocity *= Main.rand.NextFloat(4, 7);

				var line = new ImpactLine(position, velocity, DarkGold.Additive() * 0.5f, new Vector2(0.2f, 0.6f), Main.rand.Next(15, 20), 0.9f);
				line.UseLightColor = true;
				ParticleHandler.SpawnParticle(line);
			}
		}
	}
}
