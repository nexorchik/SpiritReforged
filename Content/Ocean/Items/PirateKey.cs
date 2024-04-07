namespace SpiritReforged.Content.Ocean.Items;

public class PirateKey : ModItem
{
	public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

	public override void SetDefaults()
	{
		Item.width = 14;
		Item.height = 20;
		Item.maxStack = Item.CommonMaxStack;
		Item.rare = ItemRarityID.Pink;
		Item.value = Item.buyPrice(0, 5, 0, 0);
	}
}