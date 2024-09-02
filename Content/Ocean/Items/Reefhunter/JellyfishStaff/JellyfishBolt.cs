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

	private int ShaderID => IsPink ? 93 : 96;

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

	public override void OnKill(int timeLeft)
	{
		SoundEngine.PlaySound(SoundID.NPCHit3, Projectile.Center);
		ParticleHandler.SpawnParticle(new LightningParticle(SpawnPosition, Projectile.Center, IsPink ? new Color(255, 59, 206, 0) : new Color(51, 214, 255, 0), 20, 30f));
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		for (int i = 0; i < 10; i++)
		{
			int num = Dust.NewDust(target.position, target.width, target.height, DustID.Electric, 0f, -2f, 0, default, 2f);
			Main.dust[num].noGravity = true;
			Main.dust[num].shader = GameShaders.Armor.GetSecondaryShader(ShaderID, Main.LocalPlayer);
			Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
			Main.dust[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
			Main.dust[num].scale *= .25f;
			if (Main.dust[num].position != target.Center)
				Main.dust[num].velocity = target.DirectionTo(Main.dust[num].position) * 3f;
		}
	}
}
