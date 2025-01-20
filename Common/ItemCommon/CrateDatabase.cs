using Terraria.GameContent.ItemDropRules;

namespace SpiritReforged.Common.ItemCommon;

internal class CrateDatabase : GlobalItem
{
	public readonly record struct CrateDrop(int CrateType, IItemDropRule Rule);

	internal static List<CrateDrop> CrateRules = [];

	public static void AddCrateRule(int crateType, IItemDropRule rule) => CrateRules.Add(new CrateDrop(crateType, rule));

	public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
	{
		foreach (var rule in CrateRules)
			if (item.type == rule.CrateType)
				itemLoot.Add(rule.Rule);
	}
}
