using SpiritReforged.Common.ItemCommon;

namespace SpiritReforged.Content.Forest.ArcaneNecklace;

[AutoloadEquip(EquipType.Neck)]
public class ArcaneNecklacePlatinum : ArcaneNecklaceGold
{
	public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
	{
		return incomingItem.type != ModContent.ItemType<ArcaneNecklaceGold>();
	}
}