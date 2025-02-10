namespace SpiritReforged.Content.Ocean.Items.Driftwood;

public class DriftwoodBow : ModItem
{
	public override void SetDefaults()
	{
		Item.CloneDefaults(ItemID.ShadewoodBow);
		Item.damage = 8;
	}

	public override void AddRecipes() => CreateRecipe().AddIngredient(ModContent.ItemType<DriftwoodTileItem>(), 10).AddTile(TileID.WorkBenches).Register();
}
