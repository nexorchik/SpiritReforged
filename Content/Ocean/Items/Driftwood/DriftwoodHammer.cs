namespace SpiritReforged.Content.Ocean.Items.Driftwood;

public class DriftwoodHammer : ModItem
{
	public override void SetDefaults()
	{
		Item.CloneDefaults(ItemID.ShadewoodHammer);
		Item.hammer = 39;
	}

	public override void AddRecipes() => CreateRecipe().AddIngredient(ModContent.ItemType<DriftwoodTileItem>(), 8).AddTile(TileID.WorkBenches).Register();
}
