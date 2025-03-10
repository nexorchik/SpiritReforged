namespace SpiritReforged.Content.Savanna.Items.Drywood;

[AutoloadEquip(EquipType.Head)]
public class DrywoodHelmet : ModItem
{
	public override void SetDefaults()
	{
		Item.width = 22;
		Item.height = 20;
		Item.value = Item.sellPrice(copper: 7);
		Item.defense = 1;
	}

	public override bool IsArmorSet(Item head, Item body, Item legs)
		=> (head.type, body.type, legs.type) == (Type, ModContent.ItemType<DrywoodBreastplate>(), ModContent.ItemType<DrywoodGreaves>());
	public override void UpdateArmorSet(Player player)
	{
		player.setBonus = Language.GetTextValue("Mods.SpiritReforged.SetBonuses.Drywood");

		if (player.ZoneOverworldHeight || player.ZoneSkyHeight)
			player.moveSpeed += .1f; //10% increase
	}

	public override void AddRecipes()
	{
		CreateRecipe().AddIngredient(ModContent.ItemType<Drywood>(), 20).AddTile(TileID.WorkBenches).Register();
		Recipe.Create(ModContent.ItemType<DrywoodBreastplate>()).AddIngredient(ModContent.ItemType<Drywood>(), 30).AddTile(TileID.WorkBenches).Register();
		Recipe.Create(ModContent.ItemType<DrywoodGreaves>()).AddIngredient(ModContent.ItemType<Drywood>(), 25).AddTile(TileID.WorkBenches).Register();
	}
}
