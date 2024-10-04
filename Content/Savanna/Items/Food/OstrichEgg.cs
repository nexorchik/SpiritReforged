namespace SpiritReforged.Content.Savanna.Items.Food;

public class OstrichEgg : ModItem
{
	public override void SetDefaults()
	{
		Item.width = 24;
		Item.height = 26;
		Item.rare = ItemRarityID.Blue;
		Item.maxStack = Item.CommonMaxStack;
		Item.value = Item.sellPrice(silver: 10);
		Item.useStyle = ItemUseStyleID.EatFood;
		Item.useTime = Item.useAnimation = 20;
		Item.noMelee = true;
		Item.consumable = true;
		Item.UseSound = SoundID.Item2;
		Item.buffTime = 2 * 60 * 60;
		Item.buffType = BuffID.WellFed;
	}
}
