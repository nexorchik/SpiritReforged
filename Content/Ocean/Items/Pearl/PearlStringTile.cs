using SpiritReforged.Common.Particle;
using SpiritReforged.Common.TileCommon;
using SpiritReforged.Common.Visuals.Glowmasks;
using Terraria.DataStructures;

namespace SpiritReforged.Content.Ocean.Items.Pearl;

[AutoloadGlowmask("255,255,255", false)]
public class PearlStringTile : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = false;
		Main.tileFrameImportant[Type] = true;
		Main.tileNoFail[Type] = true;

		TileID.Sets.CanDropFromRightClick[Type] = true;
		
		TileObjectData.newTile.CopyFrom(TileObjectData.Style2x1);
		TileObjectData.newTile.CoordinateHeights = [16];
		TileObjectData.newTile.Origin = new(1, 0);
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, 2, 0);
		TileObjectData.newTile.AnchorValidTiles = [TileID.Sand, TileID.Ebonsand, TileID.Crimsand, TileID.Pearlsand];
		TileObjectData.addTile(Type);

		AddMapEntry(new Color(100, 100, 120));
		RegisterItemDrop(ModContent.ItemType<PearlString>());
		SolidBottomGlobalTile.solidBottomTypes.Add(Type);

		DustType = DustID.Sand;
	}

	public override void MouseOver(int i, int j)
	{
		Player player = Main.LocalPlayer;
		player.noThrow = 2;
		player.cursorItemIconEnabled = true;
		player.cursorItemIconID = ModContent.ItemType<PearlString>();
	}

	//public override bool RightClick(int i, int j)
	//{
	//	int y = 0;
	//	TileExtensions.GetTopLeft(ref i, ref y);

	//	WorldGen.KillTile(i, j);
	//	if (Main.netMode != NetmodeID.SinglePlayer)
	//		NetMessage.SendTileSquare(-1, i, j, 2, 1);

	//	return true;
	//}

	public override bool CreateDust(int i, int j, ref int type)
	{
		var tile = Framing.GetTileSafely(i, j);
		type = (tile.TileFrameY / 18) switch
		{
			1 => DustID.Corruption,
			2 => DustID.Crimson,
			3 => DustID.Pearlsand,
			_ => DustID.Sand,
		};

		return true;
	}

	public override void PostTileFrame(int i, int j, int up, int down, int left, int right, int upLeft, int upRight, int downLeft, int downRight)
	{
		int type = Framing.GetTileSafely(i, j + 1).TileType;

		switch (type)
		{
			case TileID.Sand:
				Framing.GetTileSafely(i, j).TileFrameY = 0;
				break;

			case TileID.Ebonsand:
				Framing.GetTileSafely(i, j).TileFrameY = 18;
				break;

			case TileID.Crimsand:
				Framing.GetTileSafely(i, j).TileFrameY = 36;
				break;

			case TileID.Pearlsand:
				Framing.GetTileSafely(i, j).TileFrameY = 54;
				break;
		}
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var tile = Framing.GetTileSafely(i, j);
		var texture = TextureAssets.Tile[tile.TileType].Value;

		var source = new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16);
		var offset = Lighting.LegacyEngine.Mode > 1 && Main.GameZoomTarget == 1 ? Vector2.Zero : Vector2.One * 12;
		var position = (new Vector2(i, j) + offset) * 16 - Main.screenPosition;

		spriteBatch.Draw(texture, position, source, Lighting.GetColor(i, j), 0, Vector2.Zero, 1, SpriteEffects.None, 0);
		spriteBatch.Draw(GlowmaskTile.TileIdToGlowmask[Type].Glowmask.Value, position, source, Lighting.GetColor(i, j) * 2, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

		var rect = new Rectangle(i * 16, j * 16, 16, 16);
		if (!Main.gamePaused && Main.rand.NextBool(50) && Main.LocalPlayer.Distance(rect.Center()) < 100) //Nearby dust effects
		{
			var dustPos = Main.rand.NextVector2FromRectangle(rect);

			var dust = Dust.NewDustPerfect(dustPos, DustID.SilverCoin, Scale: .2f);
			dust.noGravity = true;
			dust.velocity = Vector2.Zero;
			dust.noLightEmittence = true;

			ParticleHandler.SpawnParticle(new Particles.GlowParticle(dustPos, Vector2.Zero, Main.DiscoColor * .8f, Color.Black, .75f, 50));
		}

		return false;
	}
}

public class PearlStringTileRubble : PearlStringTile
{
	public override string Texture => base.Texture.Remove(base.Texture.Length - 6, 6); //Remove "Rubble"

	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();
		
		FlexibleTileWand.RubblePlacementSmall.AddVariation(ModContent.ItemType<PearlString>(), Type, 0);
		TileID.Sets.CanDropFromRightClick[Type] = false;
	}

	public override void KillMultiTile(int i, int j, int frameX, int frameY)
	{
		if (Main.netMode == NetmodeID.MultiplayerClient)
			return;

		int item = Item.NewItem(null, new Rectangle(i * 16, j * 16, 16, 16), ModContent.ItemType<PearlString>());
		Main.item[item].ResetPrefix();
	}

	public override bool CanDrop(int i, int j) => false; //Don't drop the default item
	public override void MouseOver(int i, int j) { }
	public override bool RightClick(int i, int j) => false;
}