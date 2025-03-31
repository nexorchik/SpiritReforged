using SpiritReforged.Common.Misc;
using SpiritReforged.Common.Particle;
using SpiritReforged.Common.ProjectileCommon;
using SpiritReforged.Content.Particles;
using Terraria.Audio;

namespace SpiritReforged.Content.Underground.Items.BigBombs;

public abstract class BigBombProjectile : ModProjectile
{
	public virtual int OriginalType { get; }

	public int _radius = 10;
	public int _timeLeftMax = 60 * 3;

	public override void SetStaticDefaults() => BoomShroomPlayer.OriginalTypes.Add(OriginalType, Type);

	protected void SetTimeLeft(int value) => Projectile.timeLeft = _timeLeftMax = value;

	public sealed override void SetDefaults()
	{
		//Projectile.CloneDefaults(OriginalType);

		Projectile.friendly = Projectile.hostile = true;
		Projectile.timeLeft = _timeLeftMax;
		Projectile.Size = new Vector2(32);
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = -1;

		PostSetDefaults();
	}

	public virtual void PostSetDefaults() { }

	public override void AI()
	{
		Projectile.rotation += Projectile.velocity.X / 10;
		Projectile.velocity.X *= 0.99f;
		Projectile.velocity.Y += 0.15f;

		FuseVisuals();
	}

	public virtual void FuseVisuals()
	{
		var position = Projectile.Center - (new Vector2(0, Projectile.height / 2 + 10) * Projectile.scale).RotatedBy(Projectile.rotation);

		for (int i = 0; i < 2; i++)
		{
			var d = Dust.NewDustPerfect(position, DustID.Torch);
			d.noGravity = true;

			var d2 = Dust.NewDustPerfect(position, DustID.Smoke, Alpha: 180);
			d2.noGravity = true;
			d2.fadeIn = 1.5f;
		}
	}

	public override void OnKill(int timeLeft)
	{
		Explode();

		if (Main.dedServ)
			return;

		SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.Center);
		ParticleHandler.SpawnParticle(new TexturedPulseCircle(Projectile.Center, Color.Goldenrod, Color.Orange * .5f, .25f, 30 * _radius, 20, "SmokeSimple", Vector2.One, Common.Easing.EaseFunction.EaseCircularOut));
		ParticleHandler.SpawnParticle(new SmokeCloud(Projectile.Center, Vector2.Zero, Color.Gray, .05f * _radius, Common.Easing.EaseFunction.EaseCubicOut, 40));

		const int time = 5;
		ParticleHandler.SpawnParticle(new ImpactLine(Projectile.Center, Vector2.Zero, Color.Orange * .5f, new Vector2(0.8f, 1.6f) * _radius, time));
		ParticleHandler.SpawnParticle(new ImpactLine(Projectile.Center, Vector2.Zero, Color.White, new Vector2(0.5f, 1.2f) * _radius, time));

		for (int i = 0; i < _radius * 2; i++)
		{
			ParticleHandler.SpawnParticle(new GlowParticle(Projectile.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(1f, 4f), 
				Color.Lerp(Color.Orange, Color.Red, Main.rand.NextFloat()), Main.rand.NextFloat(.05f, .1f) * _radius, Main.rand.Next(10, 20), 4));

			var d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(16f * _radius), DustID.Torch, Scale: Main.rand.NextFloat() + .5f);
			d.noGravity = true;
		}
	}

	private void Explode()
	{
		//Deal damage
		int value = _radius * 16;
		Rectangle oldHitbox = Projectile.Hitbox;

		Projectile.Hitbox.Inflate(value, value);

		Projectile.Damage();
		Projectile.Hitbox = oldHitbox;

		//Destroy walls and tiles
		var area = new Rectangle((int)(Projectile.Center.X / 16) - _radius / 2, (int)(Projectile.Center.Y / 16) - _radius / 2, _radius, _radius);
		bool doWalls = Projectile.ShouldWallExplode(Projectile.Center, _radius, area.X, area.X + _radius, area.Y, area.Y + _radius);

		Projectile.ExplodeTiles(Projectile.Center, _radius / 2, area.X, area.X + _radius, area.Y, area.Y + _radius, doWalls);
	}

	public override bool? CanCutTiles() => false;

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		Projectile.Bounce(oldVelocity, 0.3f);
		return false;
	}

	public override bool PreDraw(ref Color lightColor)
	{
		var texture = TextureAssets.Projectile[Type].Value;
		var origin = new Vector2(texture.Width / 2, texture.Height / 2 + 4);

		float progress = 1f - (float)Projectile.timeLeft / _timeLeftMax;
		float lerp = (float)Math.Sin(Main.timeForVisualEffects / (60f - progress * 20)) * progress;

		float scale = Projectile.scale + lerp * .1f;
		Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, scale, SpriteEffects.None);

		var color = Projectile.GetAlpha(Color.Red.Additive()) * lerp * 2;
		Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, origin, scale, SpriteEffects.None);
		return false;
	}

	#region delegate helper
	internal static int SpreadType = 0;
	/// <summary> Adapted from <see cref="DelegateMethods.SpreadDirt"/>, allows spreading of any tile by assigning <see cref="SpreadType"/>. </summary>
	public static bool SpreadTileType(int x, int y)
	{
		if (Vector2.Distance(DelegateMethods.v2_1, new Vector2(x, y)) > DelegateMethods.f_1)
			return false;

		WorldGen.TryKillingReplaceableTile(x, y, 0);

		if (WorldGen.PlaceTile(x, y, SpreadType))
		{
			if (Main.netMode != NetmodeID.SinglePlayer)
				NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 1, x, y);

			var position = new Vector2(x * 16, y * 16);
			int num = 0;

			for (int i = 0; i < 3; i++)
			{
				var dust = Dust.NewDustDirect(position, 16, 16, num, 0f, 0f, 100, Color.Transparent, 2.2f);
				dust.noGravity = true;
				dust.velocity.Y -= 1.2f;
				dust.velocity *= 4f;

				var dust2 = Dust.NewDustDirect(position, 16, 16, num, 0f, 0f, 100, Color.Transparent, 1.3f);
				dust2.velocity.Y -= 1.2f;
				dust2.velocity *= 2f;
			}

			int num2 = y + 1;
			if (Main.tile[x, num2] != null && !TileID.Sets.Platforms[Main.tile[x, num2].TileType] && (Main.tile[x, num2].TopSlope || Main.tile[x, num2].IsHalfBlock))
			{
				WorldGen.SlopeTile(x, num2);

				if (Main.netMode != NetmodeID.SinglePlayer)
					NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 14, x, num2);
			}

			num2 = y - 1;
			if (Main.tile[x, num2] != null && !TileID.Sets.Platforms[Main.tile[x, num2].TileType] && Main.tile[x, num2].BottomSlope)
			{
				WorldGen.SlopeTile(x, num2);

				if (Main.netMode != NetmodeID.SinglePlayer)
					NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 14, x, num2);
			}

			for (int j = x - 1; j <= x + 1; j++)
			{
				for (int k = y - 1; k <= y + 1; k++)
				{
					var tile = Main.tile[j, k];
					if (!tile.HasTile || num == tile.TileType || tile.TileType != 2 && tile.TileType != 23 && tile.TileType != 60 && tile.TileType != 70 && tile.TileType != 109 && tile.TileType != 199 && tile.TileType != 477 && tile.TileType != 492)
						continue;

					bool flag = true;
					for (int l = j - 1; l <= j + 1; l++)
					{
						for (int m = k - 1; m <= k + 1; m++)
						{
							if (!WorldGen.SolidTile(l, m))
								flag = false;
						}
					}

					if (flag)
					{
						WorldGen.KillTile(j, k, fail: true);

						if (Main.netMode != NetmodeID.SinglePlayer)
							NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, j, k, 1f);
					}
				}
			}

			return true;
		}

		Tile tile2 = Main.tile[x, y];
		if (tile2 == null)
			return false;

		if (tile2.TileType < 0)
			return false;

		if (Main.tileSolid[tile2.TileType] && !TileID.Sets.Platforms[tile2.TileType])
			return tile2.TileType == 380;

		return true;
	}
	#endregion
}