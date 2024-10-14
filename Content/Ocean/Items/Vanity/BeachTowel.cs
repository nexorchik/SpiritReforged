namespace SpiritReforged.Content.Ocean.Items.Vanity;

[AutoloadEquip(EquipType.Body)]
public class BeachTowel : ModItem
{
	public override void SetDefaults()
	{
		Item.width = 26;
		Item.height = 26;
		Item.value = Item.buyPrice(0, 5, 0, 0);
		Item.rare = ItemRarityID.Blue;
		Item.vanity = true;
	}
}
