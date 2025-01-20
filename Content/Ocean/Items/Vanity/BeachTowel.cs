namespace SpiritReforged.Content.Ocean.Items.Vanity;

[AutoloadEquip(EquipType.HandsOn)]
public class BeachTowel : ModItem
{
	public override void SetDefaults()
	{
		Item.width = Item.height = 26;
		Item.value = Item.buyPrice(0, 5, 0, 0);
		Item.rare = ItemRarityID.Blue;
		Item.accessory = true;
		Item.vanity = true;
	}
}
