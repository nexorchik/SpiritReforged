using Terraria.Audio;

namespace SpiritReforged.Content.Ocean.Items.BaskingShark;

public class BaskingSharkProj : ModProjectile
{
	public override void SetDefaults()
	{
		Projectile.width = 16;
		Projectile.height = 16;
		Projectile.aiStyle = 1;
		Projectile.hide = false;
		Projectile.DamageType = DamageClass.Ranged;
		Projectile.friendly = true;
		Projectile.tileCollide = true;
		Projectile.scale = 1f;
		AIType = ProjectileID.Bullet;
	}

	public override void AI()
	{
		Projectile.velocity.Y += 0.2f;

		if (Projectile.ai[0] == 0)
		{
			Projectile.ai[0] = 1f;
			SoundEngine.PlaySound(SoundID.NPCHit1 with { Volume = 0.4f }, Projectile.Center);
			for (int index = 0; index < 8; ++index)
			{
				var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, 0.0f, 0.0f, 0, Color.Pink, 1f);
				dust.velocity *= 1f;
				dust.velocity += Projectile.velocity * 0.65f;
				dust.scale = 0.8f;
				dust.fadeIn = 1.1f;
				dust.noGravity = true;
				dust.noLight = false;
				dust.position += dust.velocity * 3f;
			}
		}

		for (int index1 = 0; index1 < 4; ++index1)
		{
			int index2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), 16, 16, DustID.Blood, Projectile.velocity.X, Projectile.velocity.Y, 50, Color.Pink, 0.7f);
			Main.dust[index2].position = (Main.dust[index2].position + Projectile.Center) / 2f;
			Main.dust[index2].noGravity = true;
			Main.dust[index2].velocity *= 0.3f;
			Main.dust[index2].scale *= 1f;
		}

		for (int index1 = 0; index1 < 2; ++index1)
		{
			int index2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), 16, 16, DustID.Blood, Projectile.velocity.X, Projectile.velocity.Y, 50, Color.Pink, 0.4f);
			Main.dust[index2].position = (Main.dust[index2].position + Projectile.Center * 5f) / 6f;
			Main.dust[index2].velocity *= 0.1f;
			Main.dust[index2].noGravity = true;
			Main.dust[index2].fadeIn = 0.9f;
			Main.dust[index2].scale *= 1f;
		}
	}
}