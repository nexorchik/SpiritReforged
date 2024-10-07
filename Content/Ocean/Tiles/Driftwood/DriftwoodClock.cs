using SpiritReforged.Common.TileCommon;
using SpiritReforged.Common.TileCommon.FurnitureTiles;

namespace SpiritReforged.Content.Ocean.Tiles.Driftwood;

public class DriftwoodClock : ClockTile, IAutoloadTileItem
{
	public void SetItemDefaults(ModItem item) => item.Item.value = Item.sellPrice(copper: 60);

	public void AddItemRecipes(ModItem item) => item.CreateRecipe()
		.AddRecipeGroup(RecipeGroupID.IronBar, 3)
		.AddIngredient(ItemID.Glass, 6)
		.AddIngredient<Items.Driftwood.DriftwoodTileItem>(10)
		.AddTile(TileID.Sawmill)
		.Register();
}
