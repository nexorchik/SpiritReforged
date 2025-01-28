namespace SpiritReforged.Content.Ocean.Items.Driftwood.DriftwoodArmor;

[AutoloadEquip(EquipType.Head)]
public class DriftwoodHelmet : ModItem
{
	public override void SetDefaults()
	{
		Item.width = 28;
		Item.height = 24;
		Item.value = 0;
		Item.rare = ItemRarityID.Blue;
		Item.defense = 2;
	}

	public override bool IsArmorSet(Item head, Item body, Item legs) => body.type == ModContent.ItemType<DriftwoodChestplate>() && legs.type == ModContent.ItemType<DriftwoodLeggings>();

	public override void UpdateArmorSet(Player player)
	{
		player.setBonus = Language.GetTextValue("Mods.SpiritReforged.SetBonuses.Driftwood");
		player.fishingSkill += 5;

		if (player.wet)
			player.velocity.Y = MathHelper.Clamp(player.velocity.Y -= 0.35f, -4, 100000);
	}

	public override void AddRecipes() => CreateRecipe().AddIngredient(ModContent.ItemType<DriftwoodTileItem>(), 15).AddTile(TileID.WorkBenches).Register();
}
