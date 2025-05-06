using SpiritReforged.Common.Easing;
using SpiritReforged.Common.ItemCommon;
using SpiritReforged.Common.Particle;
using SpiritReforged.Common.PrimitiveRendering.CustomTrails;
using SpiritReforged.Common.PrimitiveRendering;
using SpiritReforged.Common.ProjectileCommon.Abstract;
using SpiritReforged.Content.Particles;
using SpiritReforged.Common.ProjectileCommon;
using Terraria.Audio;

namespace SpiritReforged.Content.Underground.Items.BoulderClub;

public class Bowlder : ClubItem
{
	internal override float DamageScaling => 1.3f;

	public override void SafeSetDefaults()
	{
		Item.damage = 50;
		Item.knockBack = 8;
		ChargeTime = 45;
		SwingTime = 35;
		Item.width = 60;
		Item.height = 60;
		Item.crit = 4;
		Item.value = Item.sellPrice(0, 0, 1, 0);
		Item.rare = ItemRarityID.Blue;
		Item.shoot = ModContent.ProjectileType<BowlderProj>();
	}
}

class BowlderProj : BaseClubProj, IManualTrailProjectile
{
	public BowlderProj() : base(new Vector2(72)) { }

	public override float WindupTimeRatio => 0.6f;

	public override string Texture => (GetType().Namespace + '.' + Name).Replace('.', '/');
	public override void SafeSetStaticDefaults() => Main.projFrames[Type] = 2;

	public void DoTrailCreation(TrailManager tM)
	{
		float trailDist = 60 * MeleeSizeModifier;
		float trailWidth = 40 * MeleeSizeModifier;

		if (!FullCharge)
		{
			SwingTrailParameters parameters = new(AngleRange, -HoldAngle_Final, trailDist, trailWidth)
			{
				Color = Color.White,
				SecondaryColor = Color.LightGray,
				TrailLength = 0.33f,
				Intensity = 0.5f,
			};

			tM.CreateCustomTrail(new SwingTrail(Projectile, parameters, GetSwingProgressStatic, SwingTrail.BasicSwingShaderParams));
		}
	}

	public override void OnSwingStart() => TrailManager.ManualTrailSpawn(Projectile);
	public override void Swinging(Player owner)
	{
		base.Swinging(owner);

		if (FullCharge && GetSwingProgress >= 0.25f && Projectile.frame == 0)
		{
			if (Main.myPlayer == Projectile.owner)
			{
				var velocity = new Vector2(Projectile.direction * 8, 0);
				Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, velocity, ModContent.ProjectileType<RollingBowlder>(), (int)(Projectile.damage * DamageScaling), Projectile.knockBack, Projectile.owner);
			}

			Projectile.frame = 1;
		}
	}

	internal override bool CanCollide(float progress) => !FullCharge;
	public override void OnSmash(Vector2 position)
	{
		TrailManager.TryTrailKill(Projectile);
		Collision.HitTiles(Projectile.position, Vector2.UnitY, Projectile.width, Projectile.height);

		DustClouds(8);

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
		var basePosition = Vector2.Lerp(Projectile.Center, target.Center, 0.6f);
		Vector2 directionUnit = basePosition.DirectionFrom(Owner.MountedCenter) * TotalScale;

		int numParticles = FullCharge ? 12 : 8;
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

			if (!Main.rand.NextBool(3))
				Dust.NewDustPerfect(position, DustID.t_LivingWood, particleVel / 3, Scale: 0.5f);
		}

		ParticleHandler.SpawnParticle(new SmokeCloud(basePosition, directionUnit * 3, Color.LightGray, 0.06f * TotalScale, EaseFunction.EaseCubicOut, 30));
		ParticleHandler.SpawnParticle(new SmokeCloud(basePosition, directionUnit * 6, Color.LightGray, 0.08f * TotalScale, EaseFunction.EaseCubicOut, 30));
	}
}

class RollingBowlder : ModProjectile
{
	public static readonly SoundStyle Break = new("SpiritReforged/Assets/SFX/Tile/StoneCrack2")
	{
		PitchRange = (0.2f, 0.8f),
		Volume = 0.3f
	};

	public static readonly SoundStyle Hit = SoundID.Item70 with
	{
		MaxInstances = 3
	};

	public override void SetStaticDefaults()
	{
		ProjectileID.Sets.TrailCacheLength[Type] = 5;
		ProjectileID.Sets.TrailingMode[Type] = 2;
	}

	public override void SetDefaults()
	{
		Projectile.DamageType = DamageClass.Melee;
		Projectile.Size = new Vector2(32);
		Projectile.friendly = true;
		Projectile.penetrate = -1;

		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = -1;
	}

	public override void AI()
	{
		if (Projectile.velocity.Y == 0 && Main.rand.NextBool(10))
		{
			SoundEngine.PlaySound(Hit.WithVolumeScale(0.2f), Projectile.Center);
			Projectile.velocity.Y -= 3;
		}

		Projectile.velocity.Y += 0.5f;
		Projectile.rotation += Projectile.velocity.X * 0.06f;
	}

	public override void OnKill(int timeLeft)
	{
		var velocity = Projectile.oldVelocity;

		for (int i = 1; i < 6; i++)
		{
			int type = Mod.Find<ModGore>("Bowlder" + i).Type;
			Gore.NewGore(Projectile.GetSource_Death(), Projectile.position + Main.rand.NextVector2Unit() * Main.rand.NextFloat(10f), velocity * 0.1f, type);
		}

		for (int i = 0; i < 15; i++)
		{
			var random = (velocity * Main.rand.NextFloat()).RotatedByRandom(1);
			Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Stone, random.X, random.Y);
		}

		ParticleHandler.SpawnParticle(new SmokeCloud(Projectile.Center, velocity * 0.3f, Color.DarkGray, Projectile.scale * 0.15f, EaseFunction.EaseCircularOut, 40));

		SoundEngine.PlaySound(Hit, Projectile.Center);
		SoundEngine.PlaySound(Break, Projectile.Center);
	}

	//Reduce damage with hits
	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) => modifiers.FinalDamage *= Math.Max(0.2f, 1f - Projectile.numHits / 5f);

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		if (Projectile.velocity.X == 0)
			Projectile.Kill();

		if (Projectile.velocity.Y > 1f)
		{
			SoundEngine.PlaySound(Hit.WithVolumeScale(0.5f), Projectile.Center);

			ParticleHandler.SpawnParticle(new SmokeCloud(Projectile.Center, oldVelocity * 0.3f, Color.Gray, Projectile.scale * 0.15f, EaseFunction.EaseCircularOut, 40));
			Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);

			Projectile.velocity.Y = oldVelocity.Y * -0.5f;
		}

		return false;
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Projectile.QuickDrawTrail();
		Projectile.QuickDraw();
		
		return false;
	}
}