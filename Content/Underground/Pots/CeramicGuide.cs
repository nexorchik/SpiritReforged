namespace SpiritReforged.Content.Underground.Pots;

public class CeramicGuide : ModItem
{
	public override void SetDefaults()
	{
		Item.width = 28;
		Item.height = 20;
		Item.value = Item.buyPrice(0, 3, 75, 0);
		Item.rare = ItemRarityID.Blue;
	}

	public override void RightClick(Player player)
	{
		int type = (Type == ModContent.ItemType<CeramicGuide>()) ? ModContent.ItemType<CeramicGuideInactive>() : ModContent.ItemType<CeramicGuide>();
		Item.ChangeItemType(type);
	}

	public override bool CanRightClick() => true;
	public override bool ConsumeItem(Player player) => false;
	public override void UpdateInventory(Player player) => player.GetModPlayer<CeramicPreservationPlayer>().guideActive = true;
}

public class CeramicGuideInactive : CeramicGuide
{
	public override void UpdateInventory(Player player) { }
}

internal class CeramicPreservationPlayer : ModPlayer
{
	public bool guideActive;
	public override void ResetEffects() => guideActive = false;
}