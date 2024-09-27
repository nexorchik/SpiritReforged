using SpiritReforged.Common.TileCommon;
using SpiritReforged.Common.TileCommon.FurnitureTiles;

namespace SpiritReforged.Content.Savanna.Tiles.Furniture;

public class DrywoodClock : ClockTile, IAutoloadTileItem
{
	public void SetItemDefaults(ModItem item) => item.Item.value = Item.sellPrice(copper: 60);

	public void AddItemRecipes(ModItem item) => item.CreateRecipe()
		.AddRecipeGroup(RecipeGroupID.IronBar, 3)
		.AddIngredient(ItemID.Glass, 6)
		.AddIngredient<Items.Drywood.Drywood>(10)
		.AddTile(TileID.Sawmill)
		.Register();
}
