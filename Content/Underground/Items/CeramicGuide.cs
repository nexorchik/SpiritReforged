using SpiritReforged.Common.ItemCommon;
using Terraria.GameContent.ItemDropRules;

namespace SpiritReforged.Content.Underground.Items;

public class CeramicGuideInactive : ModItem
{
	public override void SetDefaults()
	{
		Item.width = Item.height = 24;
		Item.value = Item.buyPrice(0, 2, 50, 0);
		Item.rare = ItemRarityID.Blue;
	}

	public override void RightClick(Player player)
	{
		int type = (Type == ModContent.ItemType<CeramicGuide>()) ? ModContent.ItemType<CeramicGuideInactive>() : ModContent.ItemType<CeramicGuide>();
		Item.ChangeItemType(type);
	}

	public override bool CanRightClick() => true;
	public override bool ConsumeItem(Player player) => false;
}

public class CeramicGuide : CeramicGuideInactive
{
	public override void SetStaticDefaults()
	{
		CrateDatabase.AddCrateRule(ItemID.WoodenCrate, ItemDropRule.Common(Type, 20));
		CrateDatabase.AddCrateRule(ItemID.WoodenCrateHard, ItemDropRule.Common(Type, 20));
	}

	public override void UpdateInventory(Player player) => player.GetModPlayer<CeramicPreservationPlayer>().guideActive = true;
}

internal class CeramicPreservationPlayer : ModPlayer
{
	public bool guideActive;
	public override void ResetEffects() => guideActive = false;
}