using SpiritReforged.Common.TileCommon;
using SpiritReforged.Common.TileCommon.FurnitureTiles;

namespace SpiritReforged.Content.Ocean.Tiles.Driftwood;

public class DriftwoodBed : BedTile, IAutoloadTileItem
{
	public void SetItemDefaults(ModItem item) => item.Item.value = Item.sellPrice(silver: 4);

	public void AddItemRecipes(ModItem item) => item.CreateRecipe()
		.AddIngredient<Items.Driftwood.DriftwoodTileItem>(15)
		.AddIngredient(ItemID.Silk, 5)
		.AddTile(TileID.Sawmill)
		.Register();
}
