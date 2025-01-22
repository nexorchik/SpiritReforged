using SpiritReforged.Common.TileCommon.TileSway;
using SpiritReforged.Content.Forest.Stargrass.Tiles;
using Terraria.DataStructures;

namespace SpiritReforged.Content.Forest.Botanist.Tiles;

public class Wheatgrass : ModTile, ISwayTile
{
	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = false;
		Main.tileFrameImportant[Type] = true;
		Main.tileCut[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
		TileObjectData.newTile.CoordinateHeights = [16, 18];
		TileObjectData.newTile.Origin = new(0, 1);
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 0);
		TileObjectData.newTile.AnchorValidTiles = [TileID.Grass, TileID.Dirt, ModContent.TileType<StargrassTile>()];
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.RandomStyleRange = 6;
		TileObjectData.addTile(Type);

		AddMapEntry(Color.Yellow);
		DustType = DustID.Hay;
		HitSound = SoundID.Grass;
	}

	public override IEnumerable<Item> GetItemDrops(int i, int j)
	{
		if (Main.player[Player.FindClosest(new Vector2(i, j).ToWorldCoordinates(0, 0), 16, 16)].HeldItem.type == ItemID.Sickle)
			yield return new Item(ItemID.Hay, Main.rand.Next(4, 9));
	}

	public override void NumDust(int i, int j, bool fail, ref int num) => num = 3;

	public void DrawSway(int i, int j, SpriteBatch spriteBatch, Vector2 offset, float rotation, Vector2 origin)
	{
		var tile = Framing.GetTileSafely(i, j);
		var texture = TextureAssets.Tile[Type].Value;

		for (int d = 2; d >= 0; d--)
		{
			int frameX = (tile.TileFrameX + 18 * d) % (18 * 6);
			var source = new Rectangle(frameX, tile.TileFrameY, 16, (tile.TileFrameY == 0) ? 16 : 18);
			var position = new Vector2(i, j) * 16 - Main.screenPosition + new Vector2((float)Math.Sin(d * d + tile.TileFrameX) * 8f, 0);
			var color = Lighting.GetColor(i, j).MultiplyRGB(Color.Lerp(Color.White, Color.Black, d * .25f % .5f));

			if (tile.TileFrameY != 0)
			{
				spriteBatch.Draw(texture, position + offset, source with { X = frameX % (18 * 3) + 18 * 6 },
					color, rotation * 1.25f, origin, 1, SpriteEffects.None, 0);
			}

			spriteBatch.Draw(texture, position + offset, source, color, rotation, origin, 1, SpriteEffects.None, 0);
		}
	}

	public float Physics(Point16 topLeft)
	{
		var data = TileObjectData.GetTileData(Framing.GetTileSafely(topLeft));
		float rotation = Main.instance.TilesRenderer.GetWindCycle(topLeft.X, topLeft.Y, TileSwaySystem.Instance.SunflowerWindCounter);

		if (!WorldGen.InAPlaceWithWind(topLeft.X, topLeft.Y, data.Width, data.Height))
			rotation = 0f;

		return (rotation + TileSwayHelper.GetHighestWindGridPushComplex(topLeft.X, topLeft.Y, data.Width, data.Height, 30, 2f, 1, true)) * 1.5f;
	}
}
