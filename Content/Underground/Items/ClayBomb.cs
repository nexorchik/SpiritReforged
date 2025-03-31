using SpiritReforged.Content.Underground.Items.BigBombs;

namespace SpiritReforged.Content.Underground.Items;

public class ClayBomb : ModItem
{
	public override void SetDefaults()
	{
		Item.CloneDefaults(ItemID.DirtBomb);
		Item.shoot = ModContent.ProjectileType<ClayBombProjectile>();
	}
}

public class ClayBombProjectile : ModProjectile
{
	public override string Texture => base.Texture.Replace("Projectile", string.Empty);
	public override void SetDefaults() => Projectile.CloneDefaults(ProjectileID.DirtBomb);

	public override void OnKill(int timeLeft)
	{
		if (Main.netMode != NetmodeID.MultiplayerClient)
		{
			var pt = Projectile.Center.ToTileCoordinates();

			BigBombProjectile.SpreadType = TileID.ClayBlock;
			Projectile.Kill_DirtAndFluidProjectiles_RunDelegateMethodPushUpForHalfBricks(pt, 5, BigBombProjectile.SpreadTileType);
		}
	}
}