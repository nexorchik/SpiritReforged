using SpiritReforged.Content.Ocean.Items.Reefhunter.Buffs;
using SpiritReforged.Common.ItemCommon;

namespace SpiritReforged.Content.Ocean.Items.Reefhunter.OceanPendant;

public class OceanPendant : AccessoryItem
{
	public override void SetDefaults()
	{
		Item.width = 32;
		Item.height = 36;
		Item.rare = ItemRarityID.Green;
		Item.value = Item.buyPrice(0, 0, 80, 0);
		Item.accessory = true;
	}

	public override void UpdateEquip(Player player) => player.AddBuff(ModContent.BuffType<EmpoweredSwim>(), 2);
}
