using SpiritReforged.Common.ItemCommon;

namespace SpiritReforged.Content.Forest.ArcaneNecklace;

[AutoloadEquip(EquipType.Neck)]
public class ArcaneNecklaceGold : AccessoryItem
{
	public override void SetStaticDefaults() => ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<ArcaneNecklacePlatinum>();

	public override void SetDefaults()
	{
		Item.width = 26;
		Item.height = 34;
		Item.value = Item.sellPrice(0, 0, 25, 0);
		Item.rare = ItemRarityID.Blue;
		Item.accessory = true;
	}

	public override void SafeUpdateAccessory(Player player, bool hideVisual) => player.statManaMax2 += 20;
}
