using Terraria.Audio;

namespace SpiritReforged.Content.Ocean.Items.BubbleMine;

public class BubbleMineProj : ModProjectile
{
	public override LocalizedText DisplayName => Language.GetText("Mods.SpiritReforged.Items.BubbleMine.DisplayName");

	public override string Texture => "SpiritReforged/Content/Ocean/Items/BubbleMine/BubbleMine";

	public override void SetDefaults()
	{
		Projectile.CloneDefaults(ProjectileID.StickyGrenade);
		AIType = ProjectileID.StickyGrenade;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.DamageType = DamageClass.Ranged;
		Projectile.timeLeft = 600;
		Projectile.width = 20;
		Projectile.height = 30;
	}

	public override void OnKill(int timeLeft)
	{
		SoundEngine.PlaySound(SoundID.Item14, Projectile.position);

		for (float i = 0; i <= 6.28f; i += Main.rand.NextFloat(.5f, 2))
			Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, i.ToRotationVector2() * Main.rand.NextFloat(), ModContent.ProjectileType<BubbleMineBubble>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);

		for (int i = 0; i < 8; i++)
			Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Wraith, Scale: Main.rand.NextFloat(1f, 1.5f)).noGravity = true;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.immune[Projectile.owner] = 20;

	public override void AI()
	{
		if (++Projectile.ai[1] >= 7200f)
			Projectile.Kill();

		Lighting.AddLight((int)(Projectile.position.X / 16f), (int)(Projectile.position.Y / 16f), .196f, .871f, .965f);

		if (Main.myPlayer == Projectile.owner && Main.mouseRight)
		{
			Projectile.ai[1] = 7201;
			Projectile.netUpdate = true;
		}
	}
}
