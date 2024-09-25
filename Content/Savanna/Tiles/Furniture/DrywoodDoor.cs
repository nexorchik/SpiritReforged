using SpiritReforged.Common.TileCommon;
using SpiritReforged.Common.TileCommon.FurnitureTiles;

namespace SpiritReforged.Content.Savanna.Tiles.Furniture;

public class DrywoodDoor : DoorTile, IAutoloadTileItem
{
	public void SetItemDefaults(ModItem item) => item.Item.value = Item.sellPrice(copper: 40);

	public void AddItemRecipes(ModItem item) => item.CreateRecipe()
		.AddIngredient<Items.Drywood.Drywood>(6)
		.AddTile(TileID.WorkBenches)
		.Register();
}
