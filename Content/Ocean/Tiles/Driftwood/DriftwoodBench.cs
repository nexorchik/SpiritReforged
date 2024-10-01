using SpiritReforged.Common.TileCommon;
using SpiritReforged.Common.TileCommon.FurnitureTiles;

namespace SpiritReforged.Content.Ocean.Tiles.Driftwood;

public class DriftwoodBench : SofaTile, IAutoloadTileItem
{
	public void SetItemDefaults(ModItem item) => item.Item.value = Item.sellPrice(copper: 60);

	public void AddItemRecipes(ModItem item) => item.CreateRecipe()
		.AddIngredient<Items.Driftwood.DriftwoodTileItem>(5)
		.AddIngredient(ItemID.Silk, 2)
		.AddTile(TileID.Sawmill)
		.Register();
}
