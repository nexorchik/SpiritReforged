namespace SpiritReforged.Content.Savanna.Items.Drywood;

[AutoloadEquip(EquipType.Body)]
public class DrywoodBreastplate : ModItem
{
	public override void SetDefaults()
	{
		Item.width = 30;
		Item.height = 20;
		Item.value = Item.sellPrice(copper: 12);
		Item.defense = 1;
	}

	public override void AddRecipes() => CreateRecipe().AddIngredient(ModContent.ItemType<Drywood>(), 30)
			.AddTile(TileID.WorkBenches).Register();
}
