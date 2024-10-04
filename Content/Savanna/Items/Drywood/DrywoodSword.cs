namespace SpiritReforged.Content.Savanna.Items.Drywood;

public class DrywoodSword : ModItem
{
	public override void SetDefaults() => Item.CloneDefaults(ItemID.BorealWoodSword);

	public override void AddRecipes() => CreateRecipe().AddIngredient(ModContent.ItemType<Drywood>(), 7)
			.AddTile(TileID.WorkBenches).Register();
}
