namespace SpiritReforged.Content.Ocean.Items.ReefWrath;

public class ReefWrathProj : ModProjectile
{
	public int Style { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }

	private readonly int timeLeftMax = 50;

	public override void SetStaticDefaults() => Main.projFrames[Type] = 3;

	public override void SetDefaults()
	{
		Projectile.width = 18;
		Projectile.height = 24;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.friendly = true;
		Projectile.scale = 1f;
		Projectile.penetrate = -1;
		Projectile.alpha = 250;
		Projectile.tileCollide = false;
		Projectile.timeLeft = timeLeftMax;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		int num2 = Main.rand.Next(20, 40);
		for (int index1 = 0; index1 < num2; ++index1)
		{
			int index2 = Dust.NewDust(Projectile.Center, 0, 0, DustID.Blood, 0.0f, 0.0f, 100, new Color(), 1.5f);
			Main.dust[index2].velocity *= 1.2f;
			--Main.dust[index2].velocity.Y;
			Main.dust[index2].velocity += Projectile.velocity;
			Main.dust[index2].noGravity = true;
		}
	}

	public override bool ShouldUpdatePosition() => false;

	public override void AI()
	{
		Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
		if (Projectile.alpha > 0 && Projectile.timeLeft > 20)
			Projectile.alpha -= 25;
		
		if (Projectile.timeLeft <= 10)
			Projectile.alpha += 25;
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D tex = TextureAssets.Projectile[Type].Value;
		float sinewave = Math.Min(1.3f * (float)Math.Sin(Math.PI * (timeLeftMax - Projectile.timeLeft) / timeLeftMax), 1);
		var frame = new Rectangle(0, tex.Height / Main.projFrames[Type] * Style, (int)(sinewave * tex.Width), tex.Height / Main.projFrames[Type] - 2);

		Vector2 position = Projectile.Center - Main.screenPosition;
		position += new Vector2(18 * (Style + 1), 0).RotatedBy(Projectile.rotation) * (1 - sinewave);
		position -= new Vector2(18, 0).RotatedBy(Projectile.rotation) * sinewave;
		position += new Vector2(18, 0).RotatedBy(Projectile.rotation);

		Main.EntitySpriteDraw(tex, position, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, frame.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
		return false;
	}
}