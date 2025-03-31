namespace SpiritReforged.Content.Underground.Items;

public class SandBomb : ModItem
{
	public override void SetDefaults()
	{
		Item.CloneDefaults(ItemID.DirtBomb);
		Item.shoot = ModContent.ProjectileType<SandBombProjectile>();
	}
}

public class SandBombProjectile : ModProjectile
{
	public override string Texture => base.Texture.Replace("Projectile", string.Empty);
	public override void SetDefaults() => Projectile.CloneDefaults(ProjectileID.DirtBomb);
}