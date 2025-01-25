namespace SpiritReforged.Common.TileCommon.Corruption;

/// <summary> <see cref="ProjectileID.PurificationPowder"/> is excluded from <see cref="ConversionHandler"/>'s patch, so handle it here. </summary>
internal class TilePurityProjectile : GlobalProjectile
{
	public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => entity.type is ProjectileID.PurificationPowder;

	public override void AI(Projectile projectile)
	{
		int width = (int)(projectile.width / 16f);
		int height = (int)(projectile.height / 16f);
		var pos = projectile.Center.ToTileCoordinates16();

		if (width == 0)
			width = 4;

		if (height == 0)
			height = 4;

		ConversionHandler.ConvertArea(pos, Math.Max(width, height), ConversionType.Purify, projectile.GetSource_FromThis());
	}
}
