using Terraria.Audio;

namespace SpiritReforged.Content.Ocean.Items.BubbleMine;

public class BubbleMineBubble : ModProjectile
{
	public override void SetDefaults()
	{
		Projectile.aiStyle = -1;
		Projectile.width = 16;
		Projectile.height = 16;
		Projectile.friendly = true;
		Projectile.tileCollide = false;
		Projectile.hostile = false;
		Projectile.DamageType = DamageClass.Ranged;
		Projectile.timeLeft = 200;
		Projectile.alpha = 110;
		Projectile.extraUpdates = 1;
		Projectile.scale = Main.rand.NextFloat(.7f, 1.3f);
	}

	public override void AI()
	{
		Projectile.velocity.X *= .9925f;
		Projectile.velocity.Y -= .012f;
	}

	public override void OnKill(int timeLeft)
	{
		SoundEngine.PlaySound(SoundID.Item54, Projectile.Center);
		for (int i = 0; i < 20; i++)
		{
			int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.FungiHit, 0f, -2f, 0, default, 2f);
			Main.dust[num].noGravity = true;
			Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
			Main.dust[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
			Main.dust[num].scale *= .3f;
			if (Main.dust[num].position != Projectile.Center)
				Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 7f;
		}
	}
}
