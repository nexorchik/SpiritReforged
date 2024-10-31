namespace SpiritReforged.Content.Savanna.Tiles;

public class SavannaCrateTile : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileTable[Type] = true;
		Main.tileLavaDeath[Type] = true;
		Main.tileSolidTop[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
		TileObjectData.newTile.CoordinateHeights = [16, 16];
		TileObjectData.addTile(Type);

		DustType = DustID.WoodFurniture;

		AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);
		AddMapEntry(new Color(123, 104, 84));
	}

	public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
}

public class SavannaCrateHardmodeTile : SavannaCrateTile { }