using SpiritReforged.Common.TileCommon;
using SpiritReforged.Common.TileCommon.FurnitureTiles;

namespace SpiritReforged.Content.Ocean.Tiles.Driftwood;

public class DriftwoodPiano : PianoTile, IAutoloadTileItem
{
	public void SetItemDefaults(ModItem item) => item.Item.value = Item.sellPrice(copper: 60);

	public void AddItemRecipes(ModItem item) => item.CreateRecipe()
		.AddIngredient(ItemID.Bone, 4)
		.AddIngredient<Items.Driftwood.DriftwoodTileItem>(15)
		.AddIngredient(ItemID.Book)
		.AddTile(TileID.Sawmill)
		.Register();
}
