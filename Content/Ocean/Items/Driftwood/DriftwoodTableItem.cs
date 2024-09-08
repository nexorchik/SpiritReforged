namespace SpiritReforged.Content.Ocean.Items.Driftwood;

public class DriftwoodTableItem : ModItem
{
	public override void SetDefaults()
	{
		Item.width = 44;
		Item.height = 25;
		Item.value = 150;
		Item.maxStack = Item.CommonMaxStack;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.useTime = 10;
		Item.useAnimation = 15;
		Item.useTurn = true;
		Item.autoReuse = true;
		Item.consumable = true;
		Item.createTile = ModContent.TileType<DriftwoodTable>();
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient(ModContent.ItemType<DriftwoodTileItem>(), 10);
		recipe.AddTile(TileID.WorkBenches);
		recipe.Register();
	}
}