using SpiritReforged.Common.Particle;
using SpiritReforged.Common.WorldGeneration;
using Terraria.Audio;
using Terraria.DataStructures;

namespace SpiritReforged.Content.Underground.Tiles;

public class OreCarts : ModTile
{
	public const int FrameWidth = 54;

	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
		TileObjectData.newTile.Origin = new(1, 2);
		TileObjectData.newTile.CoordinateHeights = [16, 16, 16];
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.RandomStyleRange = 8;
		TileObjectData.newTile.DrawYOffset = 8;
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.AlternateTile, TileObjectData.newTile.Width, 0);
		TileObjectData.newTile.AnchorAlternateTiles = [TileID.MinecartTrack];
		TileObjectData.addTile(Type);

		DustType = DustID.WoodFurniture;

		AddMapEntry(new Color(152, 107, 73));
	}

	public override void NearbyEffects(int i, int j, bool closer)
	{
		if (closer || Main.dedServ)
			return;

		if (TileObjectData.IsTopLeft(i, j))
		{
			var world = new Vector2(i, j).ToWorldCoordinates(0, 0);
			var area = new Rectangle((int)world.X, (int)world.Y, 64, 48);
			var p = Main.LocalPlayer;

			if (p.getRect().Intersects(area) && p.velocity.LengthSquared() > 4)
			{
				int style = Main.tile[i, j].TileFrameX / FrameWidth;

				Deactivate(i, j);
				NetMessage.SendTileSquare(-1, i, j, 3, 2);

				Projectile.NewProjectile(new EntitySource_TileBreak(i, j), new Vector2(i, j).ToWorldCoordinates(32, 32), 
					p.velocity, ModContent.ProjectileType<RollingCart>(), 0, 0, Main.myPlayer, style);
			}
		}

		static void Deactivate(int i, int j)
		{
			for (int x = i; x < i + 3; x++)
			{
				for (int y = j; y < j + 3; y++)
				{
					var t = Framing.GetTileSafely(x, y);

					if (t.TileType == ModContent.TileType<OreCarts>())
						t.HasTile = false;
				}
			}
		}
	}

	public override IEnumerable<Item> GetItemDrops(int i, int j)
	{
		int frame = Main.tile[i, j].TileFrameX / FrameWidth;
		return GetItemsByStyle(frame);
	}

	public static IEnumerable<Item> GetItemsByStyle(int style)
	{
		int itemID = style switch
		{
			0 => ItemID.CopperOre,
			1 => ItemID.IronOre,
			2 => ItemID.SilverOre,
			3 => ItemID.GoldOre,
			4 => ItemID.TinOre,
			5 => ItemID.LeadOre,
			6 => ItemID.TungstenOre,
			7 => ItemID.PlatinumOre,
			_ => ItemID.Amber
		};

		yield return new Item(itemID) { stack = Main.rand.Next(22, 31) };
		yield return new Item(ItemID.Wood) { stack = Main.rand.Next(4, 12) };
	}
}

internal class RollingCart : ModProjectile
{
	public ref float Style => ref Projectile.ai[0];
	public ref float Counter => ref Projectile.ai[1];

	/// <summary> Whether this cart is going to be killed next frame. </summary>
	private bool _hide = false;

	public override void SetStaticDefaults() => Main.projFrames[Type] = 9; //Frame 9 is not reserved for Style
	public override void SetDefaults() => Projectile.Size = new Vector2(48, 32);

	public override void AI()
	{
		if (Surface())
		{
			CheckPlayerCollision();
			TryPlace();

			Projectile.velocity.Y = 0;
			Projectile.velocity.X *= .985f;

			Projectile.tileCollide = false;
		}
		else
		{
			Projectile.velocity.Y += .5f;
			Projectile.rotation = Utils.AngleLerp(Projectile.rotation, -(Projectile.velocity.ToRotation() - MathHelper.PiOver2), .1f);

			Projectile.tileCollide = true;
		}
	}

	private void CheckPlayerCollision()
	{
		if (++Counter < 10)
			return;

		foreach (var p in Main.ActivePlayers)
		{
			if (p.getRect().Intersects(Projectile.getRect()))
			{
				int push = Math.Sign(Projectile.Center.X - (p.Center - p.velocity).X);

				Projectile.velocity.X = p.velocity.X;
				Projectile.Center = new Vector2(p.Center.X + (Projectile.width / 2 + p.width / 2) * push, Projectile.Center.Y);

				break;
			}
		}
	}

	private void TryPlace()
	{
		if (_hide)
			Projectile.Kill();

		//Revert to a tile if velocity is low enough and squarely in the tile grid
		if (Projectile.velocity.LengthSquared() > .25f * .25f || (int)Projectile.Center.X % 16 != 8)
			return;

		var coords = Projectile.Center.ToTileCoordinates();

		if (WorldMethods.AreaClear(coords.X, coords.Y, 3, 2))
		{
			int type = ModContent.TileType<OreCarts>();
			WorldGen.PlaceTile(coords.X, coords.Y, type, true, true, style: (int)Style);

			if (Framing.GetTileSafely(coords).TileType == type)
				_hide = true;
		}
	}

	private bool Surface()
	{
		//Sample the two farthest contact points on the cart
		var a = Sample(Projectile.Bottom - new Vector2(Projectile.width / 2, 0).RotatedBy(Projectile.rotation), out bool f1);
		var b = Sample(Projectile.Bottom + new Vector2(Projectile.width / 2, 0).RotatedBy(Projectile.rotation), out bool f2);

		float amount = Projectile.Bottom.X / 16f % 1;
		Projectile.Bottom = Vector2.Lerp(a, b, amount);
		Projectile.rotation = a.AngleTo(b);

		return !f1 || !f2;
	}

	private static Vector2 Sample(Vector2 origin, out bool failed)
	{
		const int type = TileID.MinecartTrack;
		failed = true;

		for (int i = -2; i < 3; i++)
		{
			var p = origin + new Vector2(0, 16 * i);

			if (Framing.GetTileSafely(p).TileType == type)
			{
				origin.Y = (int)(p.Y / 16) * 16;
				failed = false;

				break;
			}
		}

		return origin;
	}

	public override void OnKill(int timeLeft)
	{
		if (_hide)
			return;

		if (Main.netMode != NetmodeID.MultiplayerClient)
		{
			foreach (var item in OreCarts.GetItemsByStyle((int)Style))
				Item.NewItem(Entity.GetSource_Death(), Projectile.Center, item);
		} //Item drops on the server/singleplayer

		if (!Main.dedServ)
		{
			SoundEngine.PlaySound(SoundID.Item5 with { PitchVariance = .5f }, Projectile.Center);
			ParticleHandler.SpawnParticle(new Particles.SmokeCloud(Projectile.Bottom, Vector2.UnitY * -.5f, Color.SandyBrown * .5f, .15f, Common.Easing.EaseFunction.EaseQuarticInOut, 150));

			for (int i = 0; i < 10; i++)
			{
				var velocity = new Vector2(0, -Main.rand.NextFloat(2f)).RotatedByRandom(.25f);
				Dust.NewDustDirect(Projectile.BottomLeft, Projectile.width, 4, DustID.WoodFurniture, velocity.X, velocity.Y, Scale: Main.rand.NextFloat() + .25f);
			}
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		var texture = TextureAssets.Projectile[Type].Value;
		var source = GetSource((int)Style);
		var gfx = new Vector2(0, Projectile.gfxOffY);

		Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + gfx, source, Projectile.GetAlpha(lightColor), Projectile.rotation, source.Size() / 2, Projectile.scale, default);

		source = GetSource(8);
		float rotation = Projectile.velocity.X * -10;

		for (int i = 0; i < 2; i++) //Draw wheels
		{
			var position = Projectile.Center + new Vector2((i == 0) ? -10 : 10, 16).RotatedBy(Projectile.rotation);
			Main.EntitySpriteDraw(texture, position - Main.screenPosition + gfx, source, Projectile.GetAlpha(lightColor), rotation, source.Size() / 2, Projectile.scale, default);
		}

		return false;

		Rectangle GetSource(int frame) => texture.Frame(Main.projFrames[Type], 1, frame, 0, -2, 0);
	}
}