namespace SpiritReforged.Content.Savanna.Items.Drywood;

public class DrywoodBow : ModItem
{
	public override void SetDefaults() => Item.CloneDefaults(ItemID.BorealWoodBow);

	public override void AddRecipes() => CreateRecipe().AddIngredient(ModContent.ItemType<Drywood>(), 10)
			.AddTile(TileID.WorkBenches).Register();
}
