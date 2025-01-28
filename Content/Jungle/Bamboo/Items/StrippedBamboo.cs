namespace SpiritReforged.Content.Jungle.Bamboo.Items;

public class StrippedBamboo : ModItem
{
	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.StrippedBambooTile>());
		Item.width = 28;
		Item.height = 20;
		Item.value = 1;
	}

	public override void AddRecipes() => CreateRecipe().AddIngredient(ItemID.BambooBlock).AddTile(TileID.WorkBenches).Register();
}
