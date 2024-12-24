namespace SpiritReforged.Content.Forest.LeatherArmor;

[AutoloadEquip(EquipType.Body)]
public class LeatherPlate : ModItem
{
	public override void SetStaticDefaults() => ArmorIDs.Body.Sets.NeedsToDrawArm[Item.bodySlot] = true;

	public override void SetDefaults()
	{
		Item.width = 30;
		Item.height = 18;
		Item.value = 4000;
		Item.rare = ItemRarityID.Blue;
		Item.defense = 2;
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient(ItemID.Leather, 8);
		recipe.AddIngredient(ItemID.IronBar, 4);
		recipe.AddTile(TileID.Anvils);
		recipe.Register();
	}
}
