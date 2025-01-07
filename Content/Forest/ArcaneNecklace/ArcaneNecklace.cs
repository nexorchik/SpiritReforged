using SpiritReforged.Common.ItemCommon;

namespace SpiritReforged.Content.Forest.ArcaneNecklace;

[AutoloadEquip(EquipType.Neck)]
public class ArcaneNecklace : AccessoryItem
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("Arcane Necklace");
		// Tooltip.SetDefault("Increases maximum mana by 20\nEnemies have 20% chance to drop an extra Mana Star");
	}

	public override void SetDefaults()
	{
		Item.width = 36;
		Item.height = 42;
		Item.value = Item.sellPrice(0, 0, 25, 0);
		Item.rare = ItemRarityID.Blue;
		Item.accessory = true;
	}

	public override void SafeUpdateAccessory(Player player, bool hideVisual) => player.statManaMax2 += 20;
}
