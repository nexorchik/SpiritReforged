using SpiritReforged.Common.ModCompat.Classic;

namespace SpiritReforged.Content.Ocean.Items;

public class PirateKey : ModItem
{
	public override bool IsLoadingEnabled(Mod mod) => SpiritClassic.Enabled;
	public override void SetDefaults()
	{
		Item.width = 14;
		Item.height = 20;
		Item.maxStack = Item.CommonMaxStack;
		Item.rare = ItemRarityID.Pink;
		Item.value = Item.buyPrice(0, 5, 0, 0);
	}
}