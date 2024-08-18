using Terraria.DataStructures;

namespace SpiritReforged.Content.Ocean.Tiles;

public class Coral2x2 : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
		TileObjectData.newTile.Width = 2;
		TileObjectData.newTile.Height = 2;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.Origin = new Point16(0, 1);
		TileObjectData.newTile.CoordinateHeights = [16, 18];
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop, TileObjectData.newTile.Width, 0);
		TileObjectData.newTile.AnchorValidTiles = [TileID.Sand, TileID.Crimsand, TileID.Ebonsand];
		TileObjectData.newTile.RandomStyleRange = 3;
		TileObjectData.addTile(Type);

		TileID.Sets.DisableSmartCursor[Type] = true;
		DustType = DustID.Coralstone;

		AddMapEntry(new Color(87, 61, 51), Language.GetText("Mods.SpiritReforged.Tiles.CoralMapEntry"));
	}

	public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

	public override IEnumerable<Item> GetItemDrops(int i, int j)
	{
		yield return new Item(ItemID.Coral) { stack = Main.rand.Next(3, 6) };
	}
}

public class Coral2x2Rubble : Coral2x2
{
	public override string Texture => base.Texture.Replace("Rubble", "");

	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
		TileObjectData.newTile.Width = 2;
		TileObjectData.newTile.Height = 2;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.Origin = new Point16(0, 1);
		TileObjectData.newTile.CoordinateHeights = [16, 18];
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop, TileObjectData.newTile.Width, 0);
		TileObjectData.newTile.AnchorValidTiles = [TileID.Sand, TileID.Crimsand, TileID.Ebonsand];
		TileObjectData.newTile.RandomStyleRange = 3;
		TileObjectData.addTile(Type);

		TileID.Sets.DisableSmartCursor[Type] = true;
		DustType = DustID.Coralstone;

		AddMapEntry(new Color(87, 61, 51), Language.GetText("Mods.SpiritReforged.Tiles.CoralMapEntry"));

		FlexibleTileWand.RubblePlacementMedium.AddVariation(ItemID.Coral, Type, 0);
	}

	public override IEnumerable<Item> GetItemDrops(int i, int j) { yield return new Item(ItemID.Coral); }
}
