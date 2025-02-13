using RubbleAutoloader;
using SpiritReforged.Common.TileCommon;
using SpiritReforged.Common.TileCommon.DrawPreviewHook;
using SpiritReforged.Common.Visuals.Glowmasks;
using SpiritReforged.Content.Ocean.Items;
using Terraria.DataStructures;

namespace SpiritReforged.Content.Ocean.Tiles;

[AutoloadGlowmask("255,255,255", false)]
public class Kelp2x3 : ModTile, IDrawPreview, IAutoloadRubble
{
	public virtual Point FrameOffset => Point.Zero;

	public override string Texture => base.Texture.Remove(base.Texture.Length - 3, 3); //Remove the size signature

	public virtual IAutoloadRubble.RubbleData Data => new(ModContent.ItemType<Kelp>(), IAutoloadRubble.RubbleSize.Large);

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
		TileObjectData.newTile.AnchorValidTiles = [TileID.Sand, TileID.Crimsand, TileID.Ebonsand, TileID.Pearlsand];
		TileObjectData.newTile.RandomStyleRange = 1;
		TileObjectData.addTile(Type);
	}

	public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

	protected static Vector3 GetGlowColor(int x)
	{
		Color[] selections = [new Color(110, 150, 138), Color.LightSeaGreen, new Color(100, 220, 110), new Color(240, 240, 180), Color.LightSkyBlue, Color.Teal, Color.PowderBlue];
		int seed = (int)((1f + (float)Math.Sin(Main.ActiveWorldFileData.Seed)) * selections.Length) % selections.Length;

		var unit = selections[seed];

		const float variance = .5f;
		return Color.Lerp(unit, selections[(seed + 1) % selections.Length], variance + (float)Math.Sin(x / 20f) * variance).ToVector3();
	}

	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
	{
		if (Framing.GetTileSafely(i, j).TileFrameY == 18)
		{
			var col = GetGlowColor(i) / 2.5f;
			(r, g, b) = (col.X, col.Y, col.Z);
		}
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

		spriteBatch.Draw(GlowmaskTile.TileIdToGlowmask[Type].Glowmask.Value, new Vector2(i * 16, j * 16) - Main.screenPosition + zero, 
			new Rectangle(t.TileFrameX + 18 * FrameOffset.X, t.TileFrameY + 18 * FrameOffset.Y, 16, 16), new Color(GetGlowColor(0)));
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

public class Kelp2x2 : Kelp2x3
{
	public override Point FrameOffset => new(2, 1);

	public override IAutoloadRubble.RubbleData Data => new(ModContent.ItemType<Kelp>(), IAutoloadRubble.RubbleSize.Medium);

	public override void SetObjectData()
	{
		TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
		TileObjectData.newTile.Width = 2;
		TileObjectData.newTile.Height = 2;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.Origin = new Point16(1, 1);
		TileObjectData.newTile.CoordinateHeights = [16, 16];
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop, TileObjectData.newTile.Width, 0);
		TileObjectData.newTile.AnchorValidTiles = [TileID.Sand, TileID.Crimsand, TileID.Ebonsand, TileID.Pearlsand];
		TileObjectData.newTile.RandomStyleRange = 1;
		TileObjectData.addTile(Type);
	}

	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
	{
		if (Framing.GetTileSafely(i, j).TileFrameY == 0)
		{
			var col = GetGlowColor(i) / 2.9f;
			(r, g, b) = (col.X, col.Y, col.Z);
		}
	}
}

public class Kelp1x2 : Kelp2x3
{
	public override Point FrameOffset => new(4, 1);

	public override IAutoloadRubble.RubbleData Data => new(ModContent.ItemType<Kelp>(), IAutoloadRubble.RubbleSize.Small);

	public override void SetObjectData()
	{
		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
		TileObjectData.newTile.Width = 1;
		TileObjectData.newTile.Height = 2;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.Origin = new Point16(0, 1);
		TileObjectData.newTile.CoordinateHeights = [16, 16];
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop, TileObjectData.newTile.Width, 0);
		TileObjectData.newTile.AnchorValidTiles = [TileID.Sand, TileID.Crimsand, TileID.Ebonsand, TileID.Pearlsand];
		TileObjectData.newTile.RandomStyleRange = 1;
		TileObjectData.addTile(Type);
	}

	public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects) => spriteEffects = (i % 2 == 0) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
	{
		if (Framing.GetTileSafely(i, j).TileFrameY == 0)
		{
			var col = GetGlowColor(i) / 2.9f;
			(r, g, b) = (col.X, col.Y, col.Z);
		}
	}
}