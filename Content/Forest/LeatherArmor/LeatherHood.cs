using SpiritReforged.Content.Forest.Botanist.Items;

namespace SpiritReforged.Content.Forest.LeatherArmor;

[AutoloadEquip(EquipType.Head)]
public class LeatherHood : ModItem
{
	public override void SetDefaults()
	{
		Item.width = 22;
		Item.height = 12;
		Item.value = 3200;
		Item.rare = ItemRarityID.Blue;
		Item.defense = 1;
	}

	public override bool IsArmorSet(Item head, Item body, Item legs)
		=> body.type == ModContent.ItemType<LeatherPlate>() && legs.type == ModContent.ItemType<LeatherLegs>();

	public override void UpdateEquip(Player player) => player.GetCritChance(DamageClass.Generic) += 5;

	public override void UpdateArmorSet(Player player)
	{
		player.setBonus = Language.GetTextValue("Mods.SpiritReforged.SetBonuses.Leather");
		player.GetModPlayer<MarksmanPlayer>().active = true;
	}

	public override void ArmorSetShadows(Player player)
	{
		if (player.GetModPlayer<MarksmanPlayer>().concentrated)
			player.armorEffectDrawOutlinesForbidden = true;
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient(ItemID.Leather, 6);
		recipe.AddIngredient(ItemID.IronBar, 2);
		recipe.AddTile(TileID.Anvils);
		recipe.Register();
	}
}
