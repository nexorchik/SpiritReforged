using SpiritReforged.Common.Particle;
using SpiritReforged.Content.Particles;
using Terraria.Audio;
using Terraria.Graphics.Shaders;

namespace SpiritReforged.Content.Ocean.Items.Reefhunter.JellyfishStaff;

public class JellyfishBolt : ModProjectile
{
	public bool IsPink
	{
		get => (int)Projectile.ai[0] != 0;
		set => Projectile.ai[0] = value ? 1 : 0;
	}

	public bool SetSpawnPos
	{
		get => (int)Projectile.ai[1] != 0;
		set => Projectile.ai[1] = value ? 1 : 0;
	}

	public Vector2 SpawnPosition = new(0, 0);

	public override string Texture => "Terraria/Images/Projectile_1"; //Use a basic texture because this projectile is hidden

	// public override void SetStaticDefaults() => DisplayName.SetDefault("Electric Bolt");

	public override void SetStaticDefaults() => ProjectileID.Sets.MinionShot[Type] = true;

	public override void SetDefaults()
	{
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.penetrate = 1;
		Projectile.timeLeft = 200;
		Projectile.height = 4;
		Projectile.width = 4;
		Projectile.hide = true;
		Projectile.extraUpdates = 40;
	}

	public override void AI()
	{
		if(!SetSpawnPos)
		{
			SpawnPosition = Projectile.Center;
			SetSpawnPos = true;
			Projectile.netUpdate = true;
		}
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		SoundEngine.PlaySound(SoundID.Item93, Projectile.Center);
		Color particleColor = IsPink ? new Color(255, 161, 225) : new Color(156, 255, 245);
		ParticleHandler.SpawnParticle(new LightningParticle(SpawnPosition, target.Center, particleColor, 30, 30f));

		for(int i = 0; i < 15; i++)
		{
			Vector2 particleVelBase = -Projectile.oldVelocity;
			ParticleHandler.SpawnParticle(new GlowParticle(target.Center, particleVelBase.RotatedByRandom(MathHelper.Pi / 3) * Main.rand.NextFloat(2f), particleColor, Main.rand.NextFloat(0.3f, 0.5f), 20, 10));
		}
	}
}
