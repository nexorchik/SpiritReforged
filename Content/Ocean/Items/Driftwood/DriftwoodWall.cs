namespace SpiritReforged.Content.Ocean.Items.Driftwood;

public class DriftwoodWallItem : ModItem
{
	public override void SetDefaults()
	{
		Item.width = 22;
		Item.height = 22;

		Item.maxStack = Item.CommonMaxStack;

		Item.useStyle = ItemUseStyleID.Swing;
		Item.useTime = 7;
		Item.useAnimation = 15;

		Item.useTurn = true;
		Item.autoReuse = true;
		Item.consumable = true;

		Item.createWall = ModContent.WallType<DriftwoodWall>();
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe(4);
		recipe.AddIngredient(ModContent.ItemType<DriftwoodTileItem>());
		recipe.AddTile(TileID.WorkBenches);
		recipe.Register();

		var recipe1 = Recipe.Create(ModContent.ItemType<DriftwoodTileItem>());
		recipe1.AddIngredient(this, 4);
		recipe1.AddTile(TileID.WorkBenches);
		recipe1.Register();
	}
}

public class DriftwoodWall : ModWall
{
	public override void SetStaticDefaults()
	{
		Main.wallHouse[Type] = true;
		AddMapEntry(new Color(87, 61, 44));
	}
}