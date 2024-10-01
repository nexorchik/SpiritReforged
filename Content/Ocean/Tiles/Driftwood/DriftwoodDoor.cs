using SpiritReforged.Common.TileCommon;
using SpiritReforged.Common.TileCommon.FurnitureTiles;

namespace SpiritReforged.Content.Ocean.Tiles.Driftwood;

public class DriftwoodDoor : DoorTile, IAutoloadTileItem
{
	public void SetItemDefaults(ModItem item) => item.Item.value = Item.sellPrice(copper: 40);

	public void AddItemRecipes(ModItem item) => item.CreateRecipe()
		.AddIngredient<Items.Driftwood.DriftwoodTileItem>(6)
		.AddTile(TileID.WorkBenches)
		.Register();
}
