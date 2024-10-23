using SpiritReforged.Content.Forest.Botanist.Tiles;

namespace SpiritReforged.Content.Forest.Botanist.Items;

internal class WheatgrassSeedProjectile : ModProjectile
{
	public override void SetDefaults()
	{
		Projectile.Size = new(6);
		Projectile.aiStyle = -1;
		Projectile.timeLeft = 2000;
	}

	public override void AI()
	{
		Projectile.velocity.Y += 0.2f;
		Projectile.rotation += Projectile.velocity.X * 0.05f;
	}

	public override void OnKill(int timeLeft)
	{
		Point pos = (Projectile.Center + new Vector2(0, 8)).ToTileCoordinates();

		if (Main.tile[pos].HasTile && TileID.Sets.Grass[Main.tile[pos].TileType] && (!Main.tile[pos.X, pos.Y - 1].HasTile || Main.tileCut[Main.tile[pos.X, pos.Y - 1].TileType]))
		{
			WorldGen.PlaceTile(pos.X, pos.Y - 1, ModContent.TileType<Wheatgrass>(), true, style: Main.rand.Next(6));

			for (int i = 0; i < 4; ++i)
				Dust.NewDust(pos.ToWorldCoordinates(), 2, 2, DustID.Hay, WorldGen.genRand.NextFloat(-1, 1) + Projectile.velocity.X, -Main.rand.NextFloat(1, 4), Scale: 0.6f);
		}
	}
}