namespace SpiritReforged.Content.Ocean.Items.Driftwood;

public class DriftwoodChairItem : ModItem
{
	public override void SetDefaults()
	{
		Item.width = 24;
		Item.height = 18;
		Item.value = 50;
		Item.maxStack = Item.CommonMaxStack;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.useTime = 10;
		Item.useAnimation = 15;
		Item.useTurn = true;
		Item.autoReuse = true;
		Item.consumable = true;
		Item.createTile = ModContent.TileType<DriftwoodChair>();
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient(ModContent.ItemType<DriftwoodTileItem>(), 4);
		recipe.AddTile(TileID.WorkBenches);
		recipe.Register();
	}
}