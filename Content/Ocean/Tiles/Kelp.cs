using SpiritReforged.Common.TileCommon;
using SpiritReforged.Common.TileCommon.DrawPreviewHook;
using Terraria.DataStructures;

namespace SpiritReforged.Content.Ocean.Tiles;

public class Kelp2x3 : ModTile, IDrawPreview
{
	public virtual Point FrameOffset => Point.Zero;

	private Asset<Texture2D> glowmask;
	public override void Load() => glowmask = ModContent.Request<Texture2D>(Texture + "_Glow");
	public override void Unload() => glowmask = null;

	public override string Texture => base.Texture.Remove(base.Texture.Length - 3, 3); //Remove the size signature

	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;
		Main.tileCut[Type] = false;
		Main.tileLighted[Type] = true;
		SolidBottomGlobalTile.solidBottomTypes.Add(Type);

		SetObjectData();

		TileID.Sets.DisableSmartCursor[Type] = true;
		DustType = DustID.Grass;
		AddMapEntry(new Color(24, 105, 25), Language.GetText("Mods.SpiritReforged.Tiles.KelpMapEntry"));
	}

	public virtual void SetObjectData()
	{
		TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
		TileObjectData.newTile.Width = 2;
		TileObjectData.newTile.Height = 3;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.Origin = new Point16(1, 2);
		TileObjectData.newTile.CoordinateHeights = [16, 16, 16];
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop, TileObjectData.newTile.Width, 0);
		TileObjectData.newTile.AnchorValidTiles = [TileID.Sand, TileID.Crimsand, TileID.Ebonsand];
		TileObjectData.newTile.RandomStyleRange = 1;
		TileObjectData.addTile(Type);
	}

	public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
	{
		if (Framing.GetTileSafely(i, j).TileFrameY == 18)
			(r, g, b) = (0.3f * 1.5f, 0.3f * 1.5f, 0);
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var tile = Framing.GetTileSafely(i, j);
		var texture = TextureAssets.Tile[tile.TileType].Value;

		var frame = new Point(tile.TileFrameX + 18 * FrameOffset.X, tile.TileFrameY + 18 * FrameOffset.Y);
		var source = new Rectangle(frame.X, frame.Y, 16, 16);

		var zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
		var position = new Vector2(i, j) * 16 - Main.screenPosition + zero;

		spriteBatch.Draw(texture, position, source, Lighting.GetColor(i, j), 0, Vector2.Zero, 1, SpriteEffects.None, 0);
		return false;
	}

	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var t = Framing.GetTileSafely(i, j);
		var zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
		spriteBatch.Draw(glowmask.Value, new Vector2(i * 16, j * 16) - Main.screenPosition + zero, new Rectangle(t.TileFrameX + 18 * FrameOffset.X, t.TileFrameY + 18 * FrameOffset.Y, 16, 16), Color.LightYellow);
	}

	public void DrawPreview(SpriteBatch spriteBatch, TileObjectPreviewData op, Vector2 pos)
	{
		var texture = TextureAssets.Tile[op.Type].Value;

		for (int frameX = 0; frameX < op.Size.X; frameX++)
		{
			for (int frameY = 0; frameY < op.Size.Y; frameY++)
			{
				(int x, int y) = (op.Coordinates.X + frameX, op.Coordinates.Y + frameY);

				var color = ((op[frameX, frameY] == 1) ? Color.White : Color.Red * .7f) * .5f;
				var frame = new Point(frameX * 18 + 18 * FrameOffset.X, frameY * 18 + 18 * FrameOffset.Y);
				var source = new Rectangle(frame.X, frame.Y, 16, 16);

				var zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
				var position = new Vector2(x, y) * 16 - Main.screenPosition + zero;

				spriteBatch.Draw(texture, position, source, color, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
			}
		}
	}
}

public class Kelp2x3Rubble : Kelp2x3
{
	public override string Texture => base.Texture.Remove(base.Texture.Length - 6, 6); //Remove "Rubble"

	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();
		FlexibleTileWand.RubblePlacementLarge.AddVariation(ModContent.ItemType<Items.Kelp>(), Type, 0);
	}

	public override IEnumerable<Item> GetItemDrops(int i, int j)
	{
		yield return new Item(ModContent.ItemType<Items.Kelp>());
	}
}

public class Kelp2x2 : Kelp2x3
{
	public override Point FrameOffset => new(2, 1);

	public override void SetObjectData()
	{
		TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
		TileObjectData.newTile.Width = 2;
		TileObjectData.newTile.Height = 2;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.Origin = new Point16(1, 1);
		TileObjectData.newTile.CoordinateHeights = [16, 16];
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop, TileObjectData.newTile.Width, 0);
		TileObjectData.newTile.AnchorValidTiles = [TileID.Sand, TileID.Crimsand, TileID.Ebonsand];
		TileObjectData.newTile.RandomStyleRange = 1;
		TileObjectData.addTile(Type);
	}

	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
	{
		if (Framing.GetTileSafely(i, j).TileFrameY == 0)
			(r, g, b) = (0.28f * 1.5f, 0.28f * 1.5f, 0);
	}
}

public class Kelp2x2Rubble : Kelp2x2
{
	public override string Texture => base.Texture.Remove(base.Texture.Length - 6, 6); //Remove "Rubble"

	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();
		FlexibleTileWand.RubblePlacementMedium.AddVariation(ModContent.ItemType<Items.Kelp>(), Type, 0);
	}

	public override IEnumerable<Item> GetItemDrops(int i, int j)
	{
		yield return new Item(ModContent.ItemType<Items.Kelp>());
	}
}

public class Kelp1x2 : Kelp2x3
{
	public override Point FrameOffset => new(4, 1);

	public override void SetObjectData()
	{
		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
		TileObjectData.newTile.Width = 1;
		TileObjectData.newTile.Height = 2;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.Origin = new Point16(0, 1);
		TileObjectData.newTile.CoordinateHeights = [16, 16];
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop, TileObjectData.newTile.Width, 0);
		TileObjectData.newTile.AnchorValidTiles = [TileID.Sand, TileID.Crimsand, TileID.Ebonsand];
		TileObjectData.newTile.RandomStyleRange = 1;
		TileObjectData.addTile(Type);
	}

	public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects) => spriteEffects = (i % 2 == 0) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
	{
		if (Framing.GetTileSafely(i, j).TileFrameY == 0)
			(r, g, b) = (0.34f * 1.5f, 0.34f * 1.5f, 0);
	}
}

public class Kelp1x2Rubble : Kelp1x2
{
	public override string Texture => base.Texture.Remove(base.Texture.Length - 6, 6); //Remove "Rubble"

	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();
		FlexibleTileWand.RubblePlacementSmall.AddVariation(ModContent.ItemType<Items.Kelp>(), Type, 0);
	}

	public override IEnumerable<Item> GetItemDrops(int i, int j)
	{
		yield return new Item(ModContent.ItemType<Items.Kelp>());
	}
}
