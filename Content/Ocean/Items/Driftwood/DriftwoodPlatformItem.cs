namespace SpiritReforged.Content.Ocean.Items.Driftwood;

public class DriftwoodPlatformItem : ModItem
{
	public override void SetDefaults()
	{
		Item.width = 8;
		Item.height = 10;
		Item.maxStack = Item.CommonMaxStack;
		Item.useTurn = true;
		Item.autoReuse = true;
		Item.useAnimation = 15;
		Item.useTime = 10;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.consumable = true;
		Item.createTile = ModContent.TileType<DriftwoodPlatform>();
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe(2);
		recipe.AddIngredient(ModContent.ItemType<DriftwoodTileItem>());
		recipe.Register();
	}
}