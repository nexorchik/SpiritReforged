using System.Linq;
using Terraria.DataStructures;

namespace SpiritReforged.Common.TileCommon.FurnitureTiles;

public abstract class PaintingTile : FurnitureTile, IAutoloadTileItem
{
	public virtual int TileHeight => 2;
	public virtual int TileWidth => 2;
	public virtual void SetItemDefaults(ModItem item) => item.Item.value = Item.buyPrice(gold: 2);
	public override void StaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;

		TileID.Sets.FramesOnKillWall[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
		TileObjectData.newTile.Height = TileHeight;
		TileObjectData.newTile.Width = TileWidth;
		TileObjectData.newTile.CoordinateHeights = Enumerable.Repeat(16, TileHeight).ToArray();

		TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
		TileObjectData.newTile.AnchorTop = AnchorData.Empty;
		TileObjectData.newTile.AnchorWall = true;
		TileObjectData.addTile(Type);
		DustType = DustID.WoodFurniture;
		AddMapEntry(new Color(23, 23, 23), Language.GetText("MapObject.Painting"));
	}
	public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 3 : 10;
}
