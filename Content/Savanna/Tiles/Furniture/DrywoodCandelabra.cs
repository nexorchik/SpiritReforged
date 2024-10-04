using SpiritReforged.Common.TileCommon;
using SpiritReforged.Common.TileCommon.FurnitureTiles;

namespace SpiritReforged.Content.Savanna.Tiles.Furniture;

public class DrywoodCandelabra : CandelabraTile, IAutoloadTileItem
{
	public void SetItemDefaults(ModItem item) => item.Item.value = Item.sellPrice(silver: 3);

	public void AddItemRecipes(ModItem item) => item.CreateRecipe()
		.AddIngredient<Items.Drywood.Drywood>(5)
		.AddIngredient(ItemID.Torch, 5)
		.AddTile(TileID.WorkBenches)
		.Register();
}
