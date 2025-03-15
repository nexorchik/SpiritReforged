using SpiritReforged.Common.TileCommon.TileSway;
using SpiritReforged.Common.Visuals.Glowmasks;
using Terraria.DataStructures;

namespace SpiritReforged.Content.Forest.Stargrass.Tiles;

[AutoloadGlowmask("230,230,195", false)]
public class Starflower : ModTile, ISwayTile
{
	public override void Load() => On_Player.FigureOutWhatToPlace += OverrideSunflower;

	/// <summary> Converts Sunflowers into Starflowers on stargrass. </summary>
	private static void OverrideSunflower(On_Player.orig_FigureOutWhatToPlace orig, Player self, Tile targetTile, Item sItem, out int tileToCreate, out int previewPlaceStyle, out bool? overrideCanPlace, out int? forcedRandom)
	{
		orig(self, targetTile, sItem, out tileToCreate, out previewPlaceStyle, out overrideCanPlace, out forcedRandom);

		if (tileToCreate != TileID.Sunflower)
			return;

		var below = Main.tile[Player.tileTargetX, Player.tileTargetY + 1];
		if (WorldGen.SolidTile(below) && below.TileType == ModContent.TileType<StargrassTile>())
			tileToCreate = ModContent.TileType<Starflower>();
	}

	public override void SetStaticDefaults()
	{
		Main.tileLighted[Type] = true;
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
		TileObjectData.newTile.Width = 2;
		TileObjectData.newTile.Height = 4;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.Origin = new Point16(0, 3);
		TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 18];
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop, TileObjectData.newTile.Width, 0);
		TileObjectData.newTile.AnchorValidTiles = [ModContent.TileType<StargrassTile>(), TileID.Grass];
		TileObjectData.newTile.RandomStyleRange = 3;
		TileObjectData.addTile(Type);

		DustType = DustID.Grass;
		AddMapEntry(new Color(20, 190, 130));
		RegisterItemDrop(ItemID.Sunflower);
	}

	public override void NumDust(int i, int j, bool fail, ref int num) => num = 3;

	public override void NearbyEffects(int i, int j, bool closer)
	{
		if (!closer)
			Main.SceneMetrics.HasSunflower = true;
	}

	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) => (r, g, b) = (.3f, .28f, .1f);

	public void DrawSway(int i, int j, SpriteBatch spriteBatch, Vector2 offset, float rotation, Vector2 origin)
	{
		var tile = Framing.GetTileSafely(i, j);
		var data = TileObjectData.GetTileData(tile);
		var drawPos = new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y);
		int heights = (tile.TileFrameY == 54) ? 18 : 16;

		var source = new Rectangle(tile.TileFrameX, tile.TileFrameY, data.CoordinateWidth, heights);

		spriteBatch.Draw(TextureAssets.Tile[Type].Value, drawPos + offset, source, Lighting.GetColor(i, j), rotation, origin, 1, SpriteEffects.None, 0);
		spriteBatch.Draw(GlowmaskTile.TileIdToGlowmask[Type].Glowmask.Value, drawPos + offset, source, Color.White, rotation, origin, 1, SpriteEffects.None, 0);
	}

	public float Physics(Point16 topLeft)
	{
		var data = TileObjectData.GetTileData(Framing.GetTileSafely(topLeft));
		float rotation = Main.instance.TilesRenderer.GetWindCycle(topLeft.X, topLeft.Y, TileSwaySystem.Instance.TreeWindCounter);

		if (!WorldGen.InAPlaceWithWind(topLeft.X, topLeft.Y, data.Width, data.Height))
			rotation = 0f;

		return rotation * .75f;
	}
}
