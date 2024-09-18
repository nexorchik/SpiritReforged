namespace SpiritReforged.Content.Savanna.Items.Vanity;

[AutoloadEquip(EquipType.Legs)]
public class SafariShorts : ModItem
{
	public override void SetDefaults()
	{
		Item.width = 22;
		Item.height = 20;
		Item.value = Item.sellPrice(0, 0, 6, 0);
		Item.rare = ItemRarityID.White;

		Item.vanity = true;
	}
}
