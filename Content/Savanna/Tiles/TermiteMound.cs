using SpiritReforged.Common.TileCommon;
using Terraria.DataStructures;

namespace SpiritReforged.Content.Savanna.Tiles;

[DrawOrder(DrawOrderAttribute.Layer.Solid, DrawOrderAttribute.Layer.Default)]
public class TermiteMoundLarge : ModTile
{
	public override string Texture => base.Texture.Replace("Large", string.Empty);

	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = false;
		Main.tileFrameImportant[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
		TileObjectData.newTile.Height = 5;
		TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16, 16];
		TileObjectData.newTile.Origin = new(2, 4);
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, 3, 0);
		TileObjectData.newTile.AnchorValidTiles = [ModContent.TileType<SavannaGrass>(), ModContent.TileType<SavannaDirt>()];
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
		DoDraw(i, j, spriteBatch);
		return false;
	}

	/// <summary> Helper for drawing all termite nest variants. </summary>
	public static void DoDraw(int i, int j, SpriteBatch spriteBatch, int frameXOffset = 0, int frameYOffset = 0)
	{
		var tile = Framing.GetTileSafely(i, j);
		var texture = TextureAssets.Tile[tile.TileType].Value;

		if (DrawOrderHandler.order == DrawOrderAttribute.Layer.Default)
		{
			var frame = new Point(tile.TileFrameX + 18 * frameXOffset, tile.TileFrameY + 18 * frameYOffset);
			var source = new Rectangle(frame.X, frame.Y, 16, 16);

			var zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
			var position = new Vector2(i, j) * 16 - Main.screenPosition + zero;

			spriteBatch.Draw(texture, position, source, Lighting.GetColor(i, j), 0, Vector2.Zero, 1, SpriteEffects.None, 0);
		}
		else if (tile.TileFrameY == 0)
		{
			var data = TileObjectData.GetTileData(tile);

			var frame = new Point(tile.TileFrameX + frameXOffset * 18, 18 * 5);
			var source = new Rectangle(frame.X, frame.Y, 16, 16);
			var position = new Vector2(i, j + data.Height) * 16 - Main.screenPosition;

			spriteBatch.Draw(texture, position, source, Lighting.GetColor(i, j + data.Height), 0, Vector2.Zero, 1, SpriteEffects.None, 0);
		}
	}
}

[DrawOrder(DrawOrderAttribute.Layer.Solid, DrawOrderAttribute.Layer.Default)]
public class TermiteMoundMedium : ModTile
{
	public override string Texture => base.Texture.Replace("Medium", string.Empty);

	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = false;
		Main.tileFrameImportant[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
		TileObjectData.newTile.Height = 4;
		TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16];
		TileObjectData.newTile.Origin = new(1, 3);
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, 3, 0);
		TileObjectData.newTile.AnchorValidTiles = [ModContent.TileType<SavannaGrass>(), ModContent.TileType<SavannaDirt>()];
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
		TermiteMoundLarge.DoDraw(i, j, spriteBatch, 3, 1);
		return false;
	}
}

[DrawOrder(DrawOrderAttribute.Layer.Solid, DrawOrderAttribute.Layer.Default)]
public class TermiteMoundSmall : ModTile
{
	public override string Texture => base.Texture.Replace("Small", string.Empty);

	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = false;
		Main.tileFrameImportant[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style2x1);
		TileObjectData.newTile.CoordinateHeights = [16];
		TileObjectData.newTile.Origin = new(1, 0);
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, 2, 0);
		TileObjectData.newTile.AnchorValidTiles = [ModContent.TileType<SavannaGrass>(), ModContent.TileType<SavannaDirt>()];
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.RandomStyleRange = 3;
		TileObjectData.addTile(Type);

		AddMapEntry(new Color(100, 92, 50));
		DustType = DustID.Dirt;
	}

	public override void NumDust(int i, int j, bool fail, ref int num) => num = 3;

	public override void KillMultiTile(int i, int j, int frameX, int frameY)
	{
		if (Main.netMode == NetmodeID.MultiplayerClient)
			return;

		for (int x = 0; x < 2; x++)
		{
			var position = new Vector2(i, j) * 16 + Main.rand.NextVector2FromRectangle(new Rectangle(0, 0, 2 * 16, 1 * 16));
			var velocity = (Vector2.UnitY * -Main.rand.NextFloat(6f)).RotatedByRandom(2f);

			var npc = NPC.NewNPCDirect(new EntitySource_TileBreak(i, j), position, ModContent.NPCType<NPCs.Termite.Termite>());
			npc.velocity = velocity;
			npc.netUpdate = true;
		}
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		TermiteMoundLarge.DoDraw(i, j, spriteBatch, 9, 4);
		return false;
	}
}
