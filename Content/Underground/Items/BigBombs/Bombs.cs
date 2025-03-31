using SpiritReforged.Common.ProjectileCommon;
using Terraria.Audio;
using Terraria.ID;

namespace SpiritReforged.Content.Underground.Items.BigBombs;

public class Bomb : BigBombProjectile
{
	public override int OriginalType => ProjectileID.Bomb;
}

public class BombSticky : BigBombProjectile
{
	public override int OriginalType => ProjectileID.StickyBomb;

	public override void AI()
	{
		const int padding = 2;

		if (Collision.SolidCollision(Projectile.position - new Vector2(padding), Projectile.width + padding * 2, Projectile.height + padding * 2))
		{
			Projectile.velocity = Vector2.Zero;
			FuseVisuals();
		}
		else
		{
			base.AI();
		}
	}

	public override bool OnTileCollide(Vector2 oldVelocity) => false;
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

			SpreadType = ModContent.TileType<PackedSand>();
			Projectile.Kill_DirtAndFluidProjectiles_RunDelegateMethodPushUpForHalfBricks(pt, _radius, SpreadTileType);
		}
	}
}

public class BombDirtSticky : BombSticky
{
	public override int OriginalType => ProjectileID.DirtStickyBomb;

	public override void OnKill(int timeLeft)
	{
		if (Main.netMode != NetmodeID.MultiplayerClient)
		{
			var pt = Projectile.Center.ToTileCoordinates();
			Projectile.Kill_DirtAndFluidProjectiles_RunDelegateMethodPushUpForHalfBricks(pt, _radius, DelegateMethods.SpreadDirt);
		}
	}
}

public class BombFish : BigBombProjectile
{
	public override int OriginalType => ProjectileID.BombFish;
}

public class BombScarab : BigBombProjectile
{
	public override int OriginalType => ProjectileID.ScarabBomb;

	public override void PostSetDefaults() => _radius = 5;

	public override void AI()
	{
		const int padding = 2;

		if (Collision.SolidCollision(Projectile.position - new Vector2(padding), Projectile.width + padding * 2, Projectile.height + padding * 2))
		{
			Projectile.velocity = Vector2.Zero;
			Projectile.rotation = Projectile.AngleFrom(Main.player[Projectile.owner].Center) - MathHelper.PiOver2;

			FuseVisuals();
		}
		else
		{
			base.AI();
		}
	}

	public override void FuseVisuals()
	{
		var position = Projectile.Center - (new Vector2(0, Projectile.height / 2 + 10) * Projectile.scale).RotatedBy(Projectile.rotation);

		for (int i = 0; i < 2; i++)
		{
			var d = Dust.NewDustPerfect(position, DustID.BlueTorch, position.DirectionFrom(Projectile.Center).RotatedByRandom(0.5f), Scale: Main.rand.NextFloat() + 1);
			d.noGravity = true;
		}
	}

	public override bool OnTileCollide(Vector2 oldVelocity) => false;

	public override void OnKill(int timeLeft)
	{
		var scarabBombDigDirectionSnap = ScarabBombSnap();
		float angle = scarabBombDigDirectionSnap.ToVector2().ToRotation() + (float)Math.PI / 2f;

		#region vfx
		SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
		Projectile.Resize(80, 80);

		for (int i = 0; i < 60; i++)
		{
			var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 2f);
			dust.velocity.Y -= 0.5f;

			var dust2 = dust;
			dust2.velocity *= 1.2f;
			dust.color = Color.Black * 0.9f;

			if (Main.rand.NextBool(2))
			{
				dust.scale = 0.5f;
				dust.fadeIn = 1f + Main.rand.Next(10) * 0.1f;
				dust.color = Color.Black * 0.8f;
			}
		}

		for (int num940 = 0; num940 < 30; num940++)
		{
			var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 59, 0f, 0f, 100);
			dust.noGravity = true;
			if (Math.Abs(Projectile.velocity.X) > 0.25f)
			{
				Projectile.velocity.X *= 0.25f / Math.Abs(Projectile.velocity.X);
			}

			dust.velocity.Y -= 0.5f;
			dust.fadeIn = 1.2f;
			var dust2 = dust;
			dust2.velocity *= 8f;
			dust.velocity = dust.velocity.RotatedBy(angle);
			dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 59, 0f, 0f, 100, default, 1.5f);
			dust2 = dust;
			dust2.velocity *= 2f;
			dust.velocity = dust.velocity.RotatedBy(angle);
			dust.velocity.Y -= 1.5f;
			dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 59, 0f, 0f, 100, default, 1.5f);
			dust.noGravity = true;
			dust.velocity.Y -= 1f;
			dust.fadeIn = 2f;
			dust2 = dust;
			dust2.velocity *= 4f;
			dust.velocity = dust.velocity.RotatedBy(angle);
		}

		bool flag4 = Math.Abs(scarabBombDigDirectionSnap.X) + Math.Abs(scarabBombDigDirectionSnap.Y) == 1;
		for (int num941 = 1; num941 <= 3; num941++)
		{
			float num942 = (float)Math.PI * 2f * Main.rand.NextFloat();
			for (float num943 = 0f; num943 < 1f; num943 += 1f / 12f)
			{
				float f6 = (float)Math.PI * 2f * num943 + num942;
				Vector2 value13 = f6.ToRotationVector2();
				value13 *= new Vector2(0.7f, 0.3f);
				var dust = Dust.NewDustPerfect(Projectile.Center, 59, value13);
				dust.fadeIn = 2f;
				dust.noGravity = true;
				Dust dust2 = dust;
				dust2.velocity *= (float)num941 + Main.rand.NextFloat() * 0.6f;
				dust.velocity.Y -= (float)num941 * 0.8f;
				dust.velocity = dust.velocity.RotatedBy(angle);
				if (flag4)
				{
					dust2 = dust;
					dust2.velocity += scarabBombDigDirectionSnap.ToVector2() * (Main.rand.NextFloat() * 11f + 6f);
				}
				else
				{
					dust2 = dust;
					dust2.velocity += scarabBombDigDirectionSnap.ToVector2() * (Main.rand.NextFloat() * 7f + 5f);
				}
			}
		}

		Vector2 vector74 = (angle - (float)Math.PI / 2f).ToRotationVector2();
		for (int num944 = 1; num944 <= 1; num944++)
		{
			for (int num945 = -1; num945 <= 1; num945 += 2)
			{
				for (int num946 = -1; num946 <= 1; num946 += 2)
				{
					var gore14 = Gore.NewGoreDirect(Entity.GetSource_Death(), Projectile.Center - Vector2.One * 20f, Vector2.Zero, Main.rand.Next(61, 64));
					gore14.velocity = gore14.velocity * 0.5f + vector74 * 3f;
					Gore gore2 = gore14;
					gore2.velocity += new Vector2(num945, num946) * 0.2f;
				}
			}
		}

		Projectile.Resize(10, 10);
		#endregion

		if (Projectile.owner == Main.myPlayer)
		{
			var tile = Projectile.Center.ToTileCoordinates();
			var world = tile.ToWorldCoordinates();

			bool wallSplode = false;
			int limit = _radius * 5;

			if (scarabBombDigDirectionSnap.X == 0 || scarabBombDigDirectionSnap.Y == 0)
				limit = (int)(limit * Math.Sqrt(2.0));

			for (int i = 0; i < limit; i++)
			{
				var vec = world + scarabBombDigDirectionSnap.ToVector2() * 16f * i * 1f;
				var p2 = vec.ToTileCoordinates();
				
				if (Projectile.ShouldWallExplode(p2.ToWorldCoordinates(), 9999, p2.X - 1, p2.X + 1, p2.Y - 1, p2.Y + 1))
				{
					wallSplode = true;
					break;
				}
			}

			for (int i = 0; i < limit; i++)
			{
				var vec2 = world + scarabBombDigDirectionSnap.ToVector2() * 16f * i * 1f;
				var p3 = vec2.ToTileCoordinates();
				int half = _radius / 2;

				Projectile.ExplodeTiles(p3.ToWorldCoordinates(), 9999, p3.X - half, p3.X + half, p3.Y - half, p3.Y + half, wallSplode);
			}
		}
	}

	private Point ScarabBombSnap()
	{
		var owner = Main.player[Projectile.owner];

		Vector2 vector = Projectile.DirectionTo(owner.Center);
		var result = new Point((!(vector.X > 0f)) ? 1 : (-1), (!(vector.Y > 0f)) ? 1 : (-1));

		if (Math.Abs(vector.X) > Math.Abs(vector.Y) * 2f)
			result.Y = 0;
		else if (Math.Abs(vector.Y) > Math.Abs(vector.X) * 2f)
			result.X = 0;

		return result;
	}
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
			Projectile.Kill_DirtAndFluidProjectiles_RunDelegateMethodPushUpForHalfBricks(pt, _radius, DelegateMethods.SpreadDry);
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