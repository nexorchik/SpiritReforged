namespace SpiritReforged.Content.Ocean;

internal class OceanGlobalItem : GlobalItem
{
	public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
	{
		if (item.type is ItemID.OceanCrate or ItemID.OceanCrateHard)
			itemLoot.AddCommon(ModContent.ItemType<Items.Lifesaver.Lifesaver>(), 12);
	}
}
