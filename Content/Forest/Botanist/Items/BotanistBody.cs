namespace SpiritReforged.Content.Forest.Botanist.Items;

[AutoloadEquip(EquipType.Body, EquipType.Waist)]
public class BotanistBody : ModItem
{
	public override void SetDefaults()
	{
		Item.width = 30;
		Item.height = 20;
		Item.value = Item.sellPrice(0, 0, 10, 0);
		Item.rare = ItemRarityID.White;
		Item.defense = 1;
	}

	public override void AddRecipes() => CreateRecipe()
			.AddIngredient(ItemID.Silk, 10)
			.AddTile(TileID.Loom)
			.Register();
}
