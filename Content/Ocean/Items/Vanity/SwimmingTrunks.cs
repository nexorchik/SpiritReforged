namespace SpiritReforged.Content.Ocean.Items.Vanity;

[AutoloadEquip(EquipType.Legs)]
public class SwimmingTrunks : ModItem
{
	public override void SetDefaults()
	{
		Item.width = 28;
		Item.height = 20;
		Item.value = Item.buyPrice(0, 5, 0, 0);
		Item.rare = ItemRarityID.Blue;
		Item.vanity = true;
	}
}