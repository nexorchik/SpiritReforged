using SpiritReforged.Common.ProjectileCommon;

namespace SpiritReforged.Content.Underground.Items.BigBombs;

public class Bomb : BigBombProjectile
{
	public override int OriginalType => ProjectileID.Bomb;
}

public class BombSticky : BigBombProjectile
{
	public override int OriginalType => ProjectileID.StickyBomb;
}

public class BombBouncy : BigBombProjectile
{
	public override int OriginalType => ProjectileID.BouncyBomb;

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		Projectile.Bounce(oldVelocity);
		return false;
	}
}

public class BombDirt : BigBombProjectile
{
	public override int OriginalType => ProjectileID.DirtBomb;

	public override void OnKill(int timeLeft)
	{
		if (Main.netMode != NetmodeID.MultiplayerClient)
		{
			var pt = Projectile.Center.ToTileCoordinates();
			Projectile.Kill_DirtAndFluidProjectiles_RunDelegateMethodPushUpForHalfBricks(pt, _radius, DelegateMethods.SpreadDirt);
		}
	}
}

public class BombClay : BigBombProjectile
{
	public override int OriginalType => ModContent.ProjectileType<ClayBombProjectile>();

	public override void OnKill(int timeLeft)
	{
		if (Main.netMode != NetmodeID.MultiplayerClient)
		{
			var pt = Projectile.Center.ToTileCoordinates();

			SpreadType = TileID.ClayBlock;
			Projectile.Kill_DirtAndFluidProjectiles_RunDelegateMethodPushUpForHalfBricks(pt, _radius, SpreadTileType);
		}
	}
}

public class BombSand : BigBombProjectile
{
	public override int OriginalType => ModContent.ProjectileType<SandBombProjectile>();

	public override void OnKill(int timeLeft)
	{
		if (Main.netMode != NetmodeID.MultiplayerClient)
		{
			var pt = Projectile.Center.ToTileCoordinates();

			SpreadType = TileID.Sandstone;
			Projectile.Kill_DirtAndFluidProjectiles_RunDelegateMethodPushUpForHalfBricks(pt, _radius, SpreadTileType);
		}
	}
}

public class BombDirtSticky : BigBombProjectile
{
	public override int OriginalType => ProjectileID.DirtStickyBomb;
}

public class BombFish : BigBombProjectile
{
	public override int OriginalType => ProjectileID.BombFish;
}

public class BombScarab : BigBombProjectile
{
	public override int OriginalType => ProjectileID.ScarabBomb;
}

public class BombDry : BigBombProjectile
{
	public override int OriginalType => ProjectileID.DryBomb;

	public override void PostSetDefaults() => _radius = 8;

	public override void OnKill(int timeLeft)
	{
		if (Main.netMode != NetmodeID.MultiplayerClient)
		{
			var pt = Projectile.Center.ToTileCoordinates();
			Projectile.Kill_DirtAndFluidProjectiles_RunDelegateMethodPushUpForHalfBricks(pt, _radius, DelegateMethods.SpreadWater);
		}
	}
}

public class BombLava : BigBombProjectile
{
	public override int OriginalType => ProjectileID.LavaBomb;

	public override void PostSetDefaults() => _radius = 6;

	public override void OnKill(int timeLeft)
	{
		if (Main.netMode != NetmodeID.MultiplayerClient)
		{
			var pt = Projectile.Center.ToTileCoordinates();
			Projectile.Kill_DirtAndFluidProjectiles_RunDelegateMethodPushUpForHalfBricks(pt, _radius, DelegateMethods.SpreadLava);
		}
	}
}

public class BombHoney : BigBombProjectile
{
	public override int OriginalType => ProjectileID.HoneyBomb;

	public override void PostSetDefaults() => _radius = 6;

	public override void OnKill(int timeLeft)
	{
		if (Main.netMode != NetmodeID.MultiplayerClient)
		{
			var pt = Projectile.Center.ToTileCoordinates();
			Projectile.Kill_DirtAndFluidProjectiles_RunDelegateMethodPushUpForHalfBricks(pt, _radius, DelegateMethods.SpreadHoney);
		}
	}
}

public class BombWet : BigBombProjectile
{
	public override int OriginalType => ProjectileID.WetBomb;

	public override void PostSetDefaults() => _radius = 6;

	public override void OnKill(int timeLeft)
	{
		if (Main.netMode != NetmodeID.MultiplayerClient)
		{
			var pt = Projectile.Center.ToTileCoordinates();
			Projectile.Kill_DirtAndFluidProjectiles_RunDelegateMethodPushUpForHalfBricks(pt, _radius, DelegateMethods.SpreadWater);
		}
	}
}

public class Dynamite : BigBombProjectile
{
	public override int OriginalType => ProjectileID.Dynamite;

	public override void PostSetDefaults()
	{
		SetTimeLeft(60 * 5);
		_radius = 20;
	}
}

public class DynamiteSticky : BigBombProjectile
{
	public override int OriginalType => ProjectileID.StickyDynamite;

	public override void PostSetDefaults()
	{
		SetTimeLeft(60 * 5);
		_radius = 20;
	}
}

public class DynamiteBouncy : BigBombProjectile
{
	public override int OriginalType => ProjectileID.BouncyDynamite;

	public override void PostSetDefaults()
	{
		SetTimeLeft(60 * 5);
		_radius = 20;
	}
}