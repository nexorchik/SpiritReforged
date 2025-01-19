namespace SpiritReforged.Content.Ocean.Items.Reefhunter.CascadeArmor;

[AutoloadEquip(EquipType.Legs)]
public class CascadeLeggings : ModItem
{
	public override void SetDefaults()
	{
		Item.width = 38;
		Item.height = 26;
		Item.value = 4000;
		Item.rare = ItemRarityID.Blue;
		Item.defense = 3;
	}

	public override void UpdateEquip(Player player)
	{
		if (player.wet)
			player.moveSpeed += .15f;
	}

	public override void AddRecipes() => CreateRecipe().AddIngredient(ModContent.ItemType<MineralSlag>(), 10).AddTile(TileID.Anvils).Register();
}
