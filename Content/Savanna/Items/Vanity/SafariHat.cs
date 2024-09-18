namespace SpiritReforged.Content.Savanna.Items.Vanity;

[AutoloadEquip(EquipType.Head)]
public class SafariHat : ModItem
{
	public override void SetStaticDefaults() => ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;
	public override void SetDefaults()
	{
		Item.width = 26;
		Item.height = 22;
		Item.value = Item.sellPrice(0, 0, 9, 0);
		Item.rare = ItemRarityID.White;

		Item.vanity = true;
	}
}
