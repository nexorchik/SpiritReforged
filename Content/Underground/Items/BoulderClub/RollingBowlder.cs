using SpiritReforged.Common.Easing;
using SpiritReforged.Common.Particle;
using SpiritReforged.Content.Particles;
using SpiritReforged.Common.ProjectileCommon;
using Terraria.Audio;

namespace SpiritReforged.Content.Underground.Items.BoulderClub;

class RollingBowlder : ModProjectile
{
	public const int MaxPenetrate = 5;

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
		Projectile.penetrate = MaxPenetrate;
		Projectile.timeLeft = 200;

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

		Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref Projectile.stepSpeed, ref Projectile.gfxOffY);
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
	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) => modifiers.FinalDamage *= Math.Max(0.2f, 1f - Projectile.numHits / (float)MaxPenetrate);

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