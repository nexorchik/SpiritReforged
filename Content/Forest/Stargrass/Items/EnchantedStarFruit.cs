using SpiritReforged.Common.ItemCommon;

namespace SpiritReforged.Content.Forest.Stargrass.Items;

public class EnchantedStarFruit : FoodItem
{
	internal override Point Size => new(22, 26);

	public override bool CanUseItem(Player player)
	{
		player.AddBuff(BuffID.ManaRegeneration, 3600);
		return true;
	}
}
