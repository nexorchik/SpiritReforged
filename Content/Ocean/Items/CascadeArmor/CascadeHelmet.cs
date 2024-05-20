namespace SpiritReforged.Content.Ocean.Items.CascadeArmor;

[AutoloadEquip(EquipType.Head)]
public class CascadeHelmet : ModItem
{
	public override void SetDefaults()
	{
		Item.width = 28;
		Item.height = 24;
		Item.value = 4800;
		Item.rare = ItemRarityID.Blue;
		Item.defense = 3;
	}

	public override bool IsArmorSet(Item head, Item body, Item legs) => body.type == ModContent.ItemType<CascadeChestplate>() && legs.type == ModContent.ItemType<CascadeLeggings>();

	public override void UpdateArmorSet(Player player)
	{
		player.setBonus = Language.GetTextValue("Mods.SpiritReforged.SetBonuses.Cascade");
		player.GetModPlayer<CascadeArmorPlayer>().setActive = true;
	}

	public override void UpdateEquip(Player player) => player.gills = true;

	public override void AddRecipes()
	{
		var recipe = CreateRecipe();
		recipe.AddIngredient(ModContent.ItemType<DeepCascadeShard>(), 12);
		recipe.AddTile(TileID.WorkBenches);
		recipe.Register();
	}
}
