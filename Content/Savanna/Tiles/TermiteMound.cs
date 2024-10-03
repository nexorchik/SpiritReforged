using SpiritReforged.Common.TileCommon;
using Terraria.DataStructures;

namespace SpiritReforged.Content.Savanna.Tiles;

[DrawOrder(DrawOrderAttribute.Layer.Solid, true)]
public class TermiteMoundLarge : ModTile
{
	public override string Texture => base.Texture.Replace("Large", string.Empty);

	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = false;
		Main.tileFrameImportant[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
		TileObjectData.newTile.Height = 5;
		TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16, 18];
		TileObjectData.newTile.Origin = new(2, 4);
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, 3, 0);
		TileObjectData.newTile.AnchorValidTiles = [ModContent.TileType<SavannaGrass>()];
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.addTile(Type);

		AddMapEntry(new Color(100, 92, 50));
		DustType = DustID.Dirt;
	}

	public override void NumDust(int i, int j, bool fail, ref int num) => num = 3;

	public override void KillMultiTile(int i, int j, int frameX, int frameY)
	{
		if (Main.netMode == NetmodeID.MultiplayerClient)
			return;

		for (int x = 0; x < 5; x++)
		{
			var position = new Vector2(i, j) * 16 + Main.rand.NextVector2FromRectangle(new Rectangle(0, 0, 3 * 16, 5 * 16));
			var velocity = (Vector2.UnitY * -Main.rand.NextFloat(6f)).RotatedByRandom(2f);

			var npc = NPC.NewNPCDirect(new EntitySource_TileBreak(i, j), position, ModContent.NPCType<NPCs.Termite.Termite>());
			npc.velocity = velocity;
			npc.netUpdate = true;
		}
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		if (!DrawOrderHandler.drawingInOrder)
			return true;

		var tile = Framing.GetTileSafely(i, j);
		if (tile.TileFrameY == 0)
		{
			var texture = TextureAssets.Tile[Type].Value;
			var frame = new Point(18 * 7, 18 * (2 + tile.TileFrameX / 18));
			var source = new Rectangle(frame.X, frame.Y, 16, 16);
			var position = new Vector2(i, j + 5) * 16 - Main.screenPosition - new Vector2(0, 2);

			spriteBatch.Draw(texture, position, source, Lighting.GetColor(i, j), 0, Vector2.Zero, 1, SpriteEffects.None, 0);
		}

		return false;
	}
}

[DrawOrder(DrawOrderAttribute.Layer.Solid, true)]
public class TermiteMoundSmall : ModTile
{
	public override string Texture => base.Texture.Replace("Small", string.Empty);

	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = false;
		Main.tileFrameImportant[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
		TileObjectData.newTile.Height = 4;
		TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 18];
		TileObjectData.newTile.Origin = new(1, 3);
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, 2, 0);
		TileObjectData.newTile.AnchorValidTiles = [ModContent.TileType<SavannaGrass>()];
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.RandomStyleRange = 2;
		TileObjectData.addTile(Type);

		AddMapEntry(new Color(100, 92, 50));
		DustType = DustID.Dirt;
	}

	public override void NumDust(int i, int j, bool fail, ref int num) => num = 3;

	public override void KillMultiTile(int i, int j, int frameX, int frameY)
	{
		if (Main.netMode == NetmodeID.MultiplayerClient)
			return;

		for (int x = 0; x < 3; x++)
		{
			var position = new Vector2(i, j) * 16 + Main.rand.NextVector2FromRectangle(new Rectangle(0, 0, 2 * 16, 4 * 16));
			var velocity = (Vector2.UnitY * -Main.rand.NextFloat(6f)).RotatedByRandom(2f);

			var npc = NPC.NewNPCDirect(new EntitySource_TileBreak(i, j), position, ModContent.NPCType<NPCs.Termite.Termite>());
			npc.velocity = velocity;
			npc.netUpdate = true;
		}
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var tile = Framing.GetTileSafely(i, j);
		var texture = TextureAssets.Tile[Type].Value;

		if (!DrawOrderHandler.drawingInOrder)
		{
			var frame = new Point(tile.TileFrameX + 18 * 3, tile.TileFrameY + 18);
			var source = new Rectangle(frame.X, frame.Y, 16, tile.TileFrameY == 18 * 3 ? 18 : 16);

			var zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
			var position = new Vector2(i, j) * 16 - Main.screenPosition + zero;

			spriteBatch.Draw(texture, position, source, Lighting.GetColor(i, j), 0, Vector2.Zero, 1, SpriteEffects.None, 0);
		}
		else if (tile.TileFrameY == 0)
		{
			var frame = new Point(18 * 7, 18 * (2 + tile.TileFrameX % (18 * 2) / 18));
			var source = new Rectangle(frame.X, frame.Y, 16, 16);
			var position = new Vector2(i, j + 4) * 16 - Main.screenPosition - new Vector2(0, 2);

			spriteBatch.Draw(texture, position, source, Lighting.GetColor(i, j), 0, Vector2.Zero, 1, SpriteEffects.None, 0);
		}

		return false;
	}
}
