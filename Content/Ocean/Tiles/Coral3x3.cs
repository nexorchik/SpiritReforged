using Terraria.DataStructures;

namespace SpiritReforged.Content.Ocean.Tiles;

public class Coral3x3 : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
		TileObjectData.newTile.Width = 3;
		TileObjectData.newTile.Height = 3;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.Origin = new Point16(1, 2);
		TileObjectData.newTile.CoordinateHeights = [16, 16, 18];
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop, TileObjectData.newTile.Width, 0);
		TileObjectData.newTile.AnchorValidTiles = [TileID.Sand, TileID.Crimsand, TileID.Ebonsand];
		TileObjectData.newTile.RandomStyleRange = 1;
		TileObjectData.addTile(Type);

		TileID.Sets.DisableSmartCursor[Type] = true;
		DustType = DustID.Coralstone;

		AddMapEntry(new Color(87, 61, 51), Language.GetText("Mods.SpiritReforged.Tiles.CoralMapEntry"));
	}

	public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
}

public class Coral3x3Rubble : Coral3x3
{
	public override string Texture => base.Texture.Replace("Rubble", "");

	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
		TileObjectData.newTile.Width = 3;
		TileObjectData.newTile.Height = 3;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.Origin = new Point16(1, 2);
		TileObjectData.newTile.CoordinateHeights = [16, 16, 18];
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop, TileObjectData.newTile.Width, 0);
		TileObjectData.newTile.AnchorValidTiles = [TileID.Sand, TileID.Crimsand, TileID.Ebonsand];
		TileObjectData.newTile.RandomStyleRange = 1;
		TileObjectData.addTile(Type);

		TileID.Sets.DisableSmartCursor[Type] = true;
		DustType = DustID.Coralstone;

		AddMapEntry(new Color(87, 61, 51), Language.GetText("Mods.SpiritReforged.Tiles.CoralMapEntry"));

		FlexibleTileWand.RubblePlacementLarge.AddVariation(ItemID.Coral, Type, 0);
		RegisterItemDrop(ItemID.Coral);
	}
}
