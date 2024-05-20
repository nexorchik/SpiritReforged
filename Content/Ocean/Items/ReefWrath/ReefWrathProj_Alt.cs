using Terraria.Audio;

namespace SpiritReforged.Content.Ocean.Items.ReefWrath;

public class ReefWrathProj_Alt : ModProjectile
{
	public override LocalizedText DisplayName => Language.GetText("Mods.SpiritReforged.Projectiles.ReefWrathProjectile_1.DisplayName");

	public override string Texture => "Terraria/Images/Projectile_1"; //Use a basic texture because this projectile is hidden

	public override void SetDefaults()
	{
		Projectile.width = 4;
		Projectile.height = 4;
		Projectile.aiStyle = 1;
		AIType = ProjectileID.WoodenArrowFriendly;
		Projectile.hide = true;
		Projectile.scale = 1f;
		Projectile.timeLeft = 2;
	}

	public override bool ShouldUpdatePosition() => false;

	public override bool? CanDamage() => false;

	public override void OnKill(int timeLeft)
	{
		for (int i = 0; i < 3; i++)
		{
			Vector2 position = Projectile.position;
			position += new Vector2(0, - (18 * (i - 1))).RotatedBy(Projectile.velocity.ToRotation());
			Projectile.NewProjectile(Projectile.GetSource_Death(), position, Projectile.velocity, ModContent.ProjectileType<ReefWrathProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner, i);
		}

		SoundEngine.PlaySound(SoundID.LiquidsWaterLava);
	}
}
