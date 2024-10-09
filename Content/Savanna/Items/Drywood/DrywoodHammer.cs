namespace SpiritReforged.Content.Savanna.Items.Drywood;

public class DrywoodHammer : ModItem
{
	public override void SetDefaults() => Item.CloneDefaults(ItemID.BorealWoodHammer);

	public override void AddRecipes() => CreateRecipe().AddIngredient(ModContent.ItemType<Drywood>(), 8)
			.AddTile(TileID.WorkBenches).Register();
}
