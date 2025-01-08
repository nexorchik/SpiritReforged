namespace SpiritReforged.Common.TileCommon.Corruption;

internal class TileCorruptionProjectile : GlobalProjectile
{
	public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => entity.type is ProjectileID.PurificationPowder or ProjectileID.ViciousPowder 
		or ProjectileID.VilePowder or ProjectileID.CorruptSpray or ProjectileID.CrimsonSpray or ProjectileID.HallowSpray or ProjectileID.PureSpray;

	public override void AI(Projectile projectile)
	{
		ConversionType type = projectile.type switch
		{
			ProjectileID.VilePowder or ProjectileID.CorruptSpray => ConversionType.Corrupt,
			ProjectileID.ViciousPowder or ProjectileID.CrimsonSpray => ConversionType.Crimson,
			ProjectileID.HallowSpray => ConversionType.Hallow,
			_ => ConversionType.Purify
		};

		int width = (int)(projectile.width / 16f);
		int height = (int)(projectile.height / 16f);
		var pos = projectile.position.ToTileCoordinates16();

		if (width == 0)
			width = 4;

		if (height == 0)
			height = 4;

		for (int i = pos.X - 1; i < pos.X + width + 1; ++i)
		{
			for (int j = pos.Y - 1; j < pos.Y + height + 1; ++j)
			{
				if (!WorldGen.InWorld(i, j, 5))
					continue;

				TileCorruptor.Convert(projectile.GetSource_FromThis(), type, i, j);
			}
		}
	}
}
