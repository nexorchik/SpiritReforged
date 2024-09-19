namespace SpiritReforged.Content.Savanna.Items.Vanity;

[AutoloadEquip(EquipType.Body)]
public class SafariVest : ModItem
{
	public override void SetDefaults()
	{
		Item.width = 30;
		Item.height = 30;
		Item.value = Item.sellPrice(0, 0, 7, 0);
		Item.rare = ItemRarityID.White;
		Item.vanity = true;
	}
}
