using SpiritReforged.Common.TileCommon;
using SpiritReforged.Common.TileCommon.FurnitureTiles;

namespace SpiritReforged.Content.Savanna.Tiles.Furniture;

public class DrywoodCandle : CandleTile, IAutoloadTileItem
{
	public void SetItemDefaults(ModItem item) => item.Item.value = Item.sellPrice(copper: 30);

	public void AddItemRecipes(ModItem item) => item.CreateRecipe()
		.AddIngredient<Items.Drywood.Drywood>(4)
		.AddIngredient(ItemID.Torch)
		.AddTile(TileID.WorkBenches)
		.Register();
}
