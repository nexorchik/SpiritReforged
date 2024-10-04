using SpiritReforged.Common.Visuals.Glowmasks;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;

namespace SpiritReforged.Common.TileCommon.FurnitureTiles;

[AutoloadGlowmask("255,165,0", false)]
public abstract class CandelabraTile : FurnitureTile
{
	public override void StaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLighted[Type] = true;
		Main.tileLavaDeath[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
		TileObjectData.newTile.Origin = new Point16(1, 1);
		TileObjectData.newTile.CoordinateHeights = [16, 18];
		TileObjectData.addTile(Type);

		AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
		AddMapEntry(new Color(100, 100, 60), Language.GetText("ItemName.Candelabra"));
		AdjTiles = [TileID.Candelabras];
		DustType = -1;
	}

	public override void HitWire(int i, int j)
	{
		var data = TileObjectData.GetTileData(Type, 0);
		int width = data.CoordinateFullWidth;

		//Move to the multitile's top left
		(i, j) = (i - Framing.GetTileSafely(i, j).TileFrameY / 18, j - Framing.GetTileSafely(i, j).TileFrameX % width / 18);

		for (int y = 0; y < 2; y++)
		{
			for (int x = 0; x < 2; x++)
			{
				var tile = Framing.GetTileSafely(i + x, j + y);
				tile.TileFrameX += (short)((tile.TileFrameX < width) ? width : -width);

				Wiring.SkipWire(i + x, j + y);
			}
		}

		NetMessage.SendTileSquare(-1, i, j, data.Width, data.Height);
	}

	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
	{
		var tile = Framing.GetTileSafely(i, j);
		var color = Color.Orange;

		if (tile.TileFrameX == 18 && tile.TileFrameY == 0)
			(r, g, b) = (color.R / 255f, color.G / 255f, color.B / 255f);
	}

	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var tile = Framing.GetTileSafely(i, j);
		if (!TileDrawing.IsVisible(tile))
			return;

		var data = TileObjectData.GetTileData(tile);
		int height = data.CoordinateHeights[tile.TileFrameY / data.CoordinateFullHeight];
		var texture = GlowmaskTile.TileIdToGlowmask[Type].Glowmask.Value;
		var source = new Rectangle(tile.TileFrameX, tile.TileFrameY, data.CoordinateWidth, height);
		var zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
		var position = new Vector2(i, j) * 16 - Main.screenPosition + zero;

		spriteBatch.Draw(texture, position, source, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

		if (tile.TileFrameY == 0 && tile.TileFrameX == 18)
			DrawFlames(i, j, spriteBatch);
	}

	/// <summary> Called by <see cref="PostDraw"/>. </summary>
	public virtual void DrawFlames(int i, int j, SpriteBatch spriteBatch)
	{
		var zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
		var texture = TextureAssets.Flames[0].Value;
		var origin = new Rectangle(0, 0, 22, 20);
		ulong randSeed = Main.TileFrameSeed ^ (ulong)((long)j << 32 | (uint)i);

		for (int x = 0; x < 3; x++)
		{
			for (int c = 0; c < 7; c++)
			{
				float shakeX = Utils.RandomInt(ref randSeed, -10, 11) * 0.15f;
				float shakeY = Utils.RandomInt(ref randSeed, -10, 1) * 0.35f;
				var offset = new Vector2(shakeX - 10 + 10 * x, shakeY + 22);

				var position = new Vector2(i, j) * 16 - Main.screenPosition + offset + zero;
				spriteBatch.Draw(texture, position, origin, new Color(100, 100, 100, 0), 0, origin.Bottom(), 1, SpriteEffects.None, 0f);
			}
		}

		if (Main.rand.NextBool(100))
		{
			float offX = -20 + Main.rand.NextFloat(3f) * 10;
			Dust.NewDustPerfect(new Vector2(i, j) * 16 + new Vector2(8 + offX, 8), DustID.Torch, (Vector2.UnitY * -Main.rand.NextFloat(2f)).RotatedByRandom(.5f));
		}
	}
}
