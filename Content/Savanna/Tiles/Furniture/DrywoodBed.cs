using SpiritReforged.Common.TileCommon;
using SpiritReforged.Common.TileCommon.FurnitureTiles;

namespace SpiritReforged.Content.Savanna.Tiles.Furniture;

public class DrywoodBed : BedTile, IAutoloadTileItem
{
	public void SetItemDefaults(ModItem item) => item.Item.value = Item.sellPrice(silver: 4);

	public void AddItemRecipes(ModItem item) => item.CreateRecipe()
		.AddIngredient<Items.Drywood.Drywood>(15)
		.AddIngredient(ItemID.Silk, 5)
		.AddTile(TileID.Sawmill)
		.Register();
}
