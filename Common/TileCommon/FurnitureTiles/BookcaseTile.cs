using Terraria.DataStructures;

namespace SpiritReforged.Common.TileCommon.FurnitureTiles;

public abstract class BookcaseTile : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
		TileObjectData.newTile.Origin = new Point16(2, 3);
		TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 18];
		TileObjectData.addTile(Type);

		AddMapEntry(new Color(100, 100, 60), Language.GetText("ItemName.Bookcase"));
		AdjTiles = [TileID.Bookcases];
		DustType = -1;
	}
}
