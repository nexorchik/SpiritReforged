using SpiritReforged.Common.TileCommon;
using Terraria.DataStructures;

namespace SpiritReforged.Content.Savanna.Tiles.Furniture;

public class DrywoodBookcase : ModTile, IAutoloadTileItem
{
	public void SetItemDefaults(ModItem item) => item.Item.value = Item.sellPrice(copper: 60);

	public void AddItemRecipes(ModItem item) => item.CreateRecipe()
		.AddIngredient<Items.Drywood.Drywood>(20)
		.AddIngredient(ItemID.Book, 10)
		.AddTile(TileID.Sawmill)
		.Register();

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
