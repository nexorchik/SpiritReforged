using SpiritReforged.Common.Particle;
using SpiritReforged.Content.Particles;
using System.IO;
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

	public override void SetStaticDefaults() => ProjectileID.Sets.MinionShot[Type] = true;

	public override void SetDefaults()
	{
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.penetrate = 1;
		Projectile.timeLeft = 300;
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

	public override void OnKill(int timeLeft)
	{
		//If the projectile times out and doesn't hit something
		if(timeLeft == 0 && Projectile.penetrate > 0 && !Main.dedServ)
		{
			ParticleHandler.SpawnParticle(new LightningParticle(SpawnPosition, Projectile.Center, ParticleColor, 30, 30f));
			SoundEngine.PlaySound(SoundID.Item93, Projectile.Center);
		}
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		if (Main.dedServ)
			return;

		SoundEngine.PlaySound(SoundID.Item93, Projectile.Center);
		ParticleHandler.SpawnParticle(new LightningParticle(SpawnPosition, target.Center, ParticleColor, 30, 30f));

		for(int i = 0; i < 15; i++)
		{
			Vector2 particleVelBase = -Projectile.oldVelocity.RotatedByRandom(MathHelper.Pi / 2);
			ParticleHandler.SpawnParticle(new GlowParticle(target.Center, particleVelBase * Main.rand.NextFloat(2f), ParticleColor, Main.rand.NextFloat(0.5f, 1f), Main.rand.Next(10, 30), 10));
		}
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		if(!Main.dedServ)
		{
			SoundEngine.PlaySound(SoundID.Item93, Projectile.Center);
			ParticleHandler.SpawnParticle(new LightningParticle(SpawnPosition, Projectile.Center, ParticleColor, 30, 30f));

			for (int i = 0; i < 15; i++)
			{
				Vector2 particleVelBase = -Projectile.oldVelocity.RotatedByRandom(MathHelper.Pi / 2);
				ParticleHandler.SpawnParticle(new GlowParticle(Projectile.Center, particleVelBase * Main.rand.NextFloat(2f), ParticleColor, Main.rand.NextFloat(0.5f, 1f), Main.rand.Next(10, 30), 10));
			}
		}

		return base.OnTileCollide(oldVelocity);
	}

	private Color ParticleColor => IsPink ? new Color(255, 161, 225) : new Color(156, 255, 245);

	public override void SendExtraAI(BinaryWriter writer) => writer.WriteVector2(SpawnPosition);

	public override void ReceiveExtraAI(BinaryReader reader) => SpawnPosition = reader.ReadVector2();
}
