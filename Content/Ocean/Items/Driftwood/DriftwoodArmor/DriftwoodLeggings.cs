namespace SpiritReforged.Content.Ocean.Items.Driftwood.DriftwoodArmor;

[AutoloadEquip(EquipType.Legs)]
public class DriftwoodLeggings : ModItem
{
	// public override void SetStaticDefaults() => DisplayName.SetDefault("Driftwood Leggings");

	public override void SetDefaults()
	{
		Item.width = 38;
		Item.height = 26;
		Item.value = 0;
		Item.rare = ItemRarityID.Blue;
		Item.defense = 1;
	}

	public override void AddRecipes()
	{
		var recipe = CreateRecipe();
		recipe.AddIngredient(ModContent.ItemType<DriftwoodTileItem>(), 12);
		recipe.AddTile(TileID.WorkBenches);
		recipe.Register();
	}
}
