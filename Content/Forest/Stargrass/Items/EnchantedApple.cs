using SpiritReforged.Common.ItemCommon;

namespace SpiritReforged.Content.Forest.Stargrass.Items;

public class EnchantedApple : FoodItem
{
	internal override Point Size => new(20, 22);
	public override void StaticDefaults() => SetFruitType();

	public override bool CanUseItem(Player player)
	{
		player.AddBuff(BuffID.ManaRegeneration, 3600);
		return true;
	}
}
