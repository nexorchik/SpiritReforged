using SpiritReforged.Content.Underground.Items.BigBombs;

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

	public override void OnKill(int timeLeft)
	{
		if (Main.netMode != NetmodeID.MultiplayerClient)
		{
			var pt = Projectile.Center.ToTileCoordinates();

			BigBombProjectile.SpreadType = ModContent.TileType<PackedSand>();
			Projectile.Kill_DirtAndFluidProjectiles_RunDelegateMethodPushUpForHalfBricks(pt, 5, BigBombProjectile.SpreadTileType);
		}
	}
}

/// <summary> Mimics <see cref="TileID.Sand"/> but without gravity-affectedness. </summary>
public class PackedSand : ModTile
{
	public override string Texture => "Terraria/Images/Tiles_" + TileID.Sand;

	public override void SetStaticDefaults()
	{
		Main.tileBlockLight[Type] = true;
		Main.tileSolid[Type] = true;

		TileID.Sets.CanBeDugByShovel[Type] = true;
		TileID.Sets.Conversion.Sand[Type] = true;

		RegisterItemDrop(ItemID.SandBlock);
		MineResist = .5f;
	}
}