using Terraria.DataStructures;
using Terraria.GameContent.Drawing;

namespace SpiritReforged.Common.TileCommon.FurnitureTiles;

public abstract class LanternTile : ModTile
{
	protected Asset<Texture2D> glowTexture;

	public override void Load()
	{
		if (!Main.dedServ && ModContent.RequestIfExists<Texture2D>(Texture + "_Glow", out var texture))
			glowTexture = texture;
	}

	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLighted[Type] = true;
		Main.tileLavaDeath[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
		TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 0);
		TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
		TileObjectData.newTile.Origin = new Point16(0, 0);
		TileObjectData.newTile.CoordinateHeights = [16, 18];
		TileObjectData.addTile(Type);

		RegisterItemDrop(Mod.Find<ModItem>(Name + "Item").Type);
		AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
		AddMapEntry(new Color(100, 100, 60), Language.GetText("MapObject.Lantern"));
		AdjTiles = [TileID.HangingLanterns];
		DustType = -1;
	}

	public override void HitWire(int i, int j)
	{
		var data = TileObjectData.GetTileData(Type, 0);
		int width = data.CoordinateFullWidth;

		j -= Framing.GetTileSafely(i, j).TileFrameY / 18; //Move to the multitile's top

		for (int h = 0; h < 2; h++)
		{
			var tile = Framing.GetTileSafely(i, j + h);
			tile.TileFrameX += (short)((tile.TileFrameX < width) ? width : -width);

			Wiring.SkipWire(i, j + h);
		}

		NetMessage.SendTileSquare(-1, i, j, data.Width, data.Height);
	}

	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
	{
		var tile = Framing.GetTileSafely(i, j);
		var color = Color.Orange;

		if (tile.TileFrameX < 18 && tile.TileFrameY == 18)
			(r, g, b) = (color.R / 255f, color.G / 255f, color.B / 255f);
	}

	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var tile = Framing.GetTileSafely(i, j);
		if (!TileDrawing.IsVisible(tile) || glowTexture is null)
			return;

		var data = TileObjectData.GetTileData(tile);
		int height = data.CoordinateHeights[tile.TileFrameY / data.CoordinateFullHeight];
		var texture = glowTexture.Value;
		var source = new Rectangle(tile.TileFrameX, tile.TileFrameY, data.CoordinateWidth, height);
		var lightOffset = Lighting.LegacyEngine.Mode > 1 && Main.GameZoomTarget == 1 ? Vector2.Zero : Vector2.One * 12;
		var position = (new Vector2(i, j) + lightOffset) * 16 - Main.screenPosition;

		spriteBatch.Draw(texture, position, source, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
	}
}
