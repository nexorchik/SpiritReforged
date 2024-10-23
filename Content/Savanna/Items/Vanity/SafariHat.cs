namespace SpiritReforged.Content.Savanna.Items.Vanity;

[AutoloadEquip(EquipType.Head)]
public class SafariHat : ModItem
{
	public override void SetStaticDefaults() => ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;

	public override void SetDefaults()
	{
		Item.width = 26;
		Item.height = 22;
		Item.value = Item.sellPrice(silver: 9);
		Item.rare = ItemRarityID.White;
		Item.vanity = true;
	}
}
