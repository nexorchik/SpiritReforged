namespace SpiritReforged.Content.Underground.Items;

public class CeramicShard : ModItem
{
	public override void SetDefaults()
	{
		Item.width = Item.height = 20;
		Item.value = Item.sellPrice(copper: 10);
		Item.rare = ItemRarityID.White;
		Item.maxStack = Item.CommonMaxStack;
	}
}