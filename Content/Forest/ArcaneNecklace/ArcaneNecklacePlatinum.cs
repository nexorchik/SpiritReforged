using SpiritReforged.Common.ItemCommon;
using SpiritReforged.Content.Ocean.Items.PoolNoodle;
using System.IO;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace SpiritReforged.Content.Forest.ArcaneNecklace;

[AutoloadEquip(EquipType.Neck)]
public class ArcaneNecklacePlatinum : ArcaneNecklaceGold
{
	public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
	{
		return incomingItem.type != ModContent.ItemType<ArcaneNecklaceGold>();
	}
}