using Terraria.DataStructures;

namespace SpiritReforged.Common.TileCommon.Corruption;

internal class TileCorruptionProjectile : GlobalProjectile
{
	public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => entity.type is ProjectileID.PurificationPowder or ProjectileID.ViciousPowder 
		or ProjectileID.VilePowder or ProjectileID.CorruptSpray or ProjectileID.CrimsonSpray or ProjectileID.HallowSpray or ProjectileID.PureSpray or ProjectileID.HolyWater
		or ProjectileID.UnholyWater or ProjectileID.BloodWater;

	public override void AI(Projectile projectile)
	{
		if (projectile.type is ProjectileID.HolyWater or ProjectileID.UnholyWater or ProjectileID.BloodWater)
			return;

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

	public override void OnKill(Projectile projectile, int timeLeft)
	{
		if (projectile.type is not (ProjectileID.HolyWater or ProjectileID.UnholyWater or ProjectileID.BloodWater))
			return;

		ConversionType type = projectile.type switch
		{
			ProjectileID.UnholyWater => ConversionType.Corrupt,
			ProjectileID.BloodWater => ConversionType.Crimson,
			ProjectileID.HolyWater => ConversionType.Hallow,
			_ => ConversionType.Purify
		};

		Point16 pos = projectile.Center.ToTileCoordinates16();

		for (int i = pos.X - 20; i < pos.X + 20; ++i)
			for (int j = pos.Y - 20; j < pos.Y + 20; ++j)
				if (Vector2.Distance(new Vector2(i, j), pos.ToVector2()) < 5 && WorldGen.InWorld(i, j, WorldGen.InfectionAndGrassSpreadOuterWorldBuffer))
					TileCorruptor.Convert(projectile.GetSource_Death(), type, i, j);
	}
}
