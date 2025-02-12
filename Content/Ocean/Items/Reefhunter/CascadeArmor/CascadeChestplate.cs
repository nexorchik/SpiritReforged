namespace SpiritReforged.Content.Ocean.Items.Reefhunter.CascadeArmor;

[AutoloadEquip(EquipType.Body)]
public class CascadeChestplate : ModItem
{
	public override void SetDefaults()
	{
		Item.width = 38;
		Item.height = 26;
		Item.value = 5600;
		Item.rare = ItemRarityID.Blue;
		Item.defense = 4;
	}

	public override void AddRecipes() => CreateRecipe().AddIngredient(ModContent.ItemType<MineralSlag>(), 14)
		.AddIngredient(ItemID.SharkFin).AddTile(TileID.Anvils).Register();
}
