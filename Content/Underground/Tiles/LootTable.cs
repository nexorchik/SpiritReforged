using Terraria.GameContent.ItemDropRules;

namespace SpiritReforged.Content.Underground.Tiles;

public interface ILootTile
{
	public LootTable AddLoot(int objectStyle);
}

/// <summary> A self-contained loot table. </summary>
public readonly struct LootTable() : ILoot
{
	public readonly List<IItemDropRule> rules = [];

	public IItemDropRule Add(IItemDropRule entry)
	{
		rules.Add(entry);
		return entry;
	}

	public IItemDropRule Remove(IItemDropRule entry)
	{
		rules.Remove(entry);
		return entry;
	}

	public List<IItemDropRule> Get(bool includeGlobalDrops = true) => rules;
	public void RemoveWhere(Predicate<IItemDropRule> predicate, bool includeGlobalDrops = true)
	{
		foreach (var entry in Get())
		{
			if (predicate(entry))
				Remove(entry);
		}
	}
}

public static class LootTableHelper
{
	public static DropAttemptInfo GetInfo(Player player, NPC npc = null)
	{
		var info = default(DropAttemptInfo);
		info.player = player;
		info.npc = npc;
		info.IsExpertMode = Main.expertMode;
		info.IsMasterMode = Main.masterMode;
		info.IsInSimulation = false;
		info.rng = Main.rand;

		return info;
	}

	public static void Resolve(this LootTable t, DropAttemptInfo info)
	{
		foreach (var rule in t.Get())
			ResolveRule(rule, info);
	}

	private static ItemDropAttemptResult ResolveRule(IItemDropRule rule, DropAttemptInfo info)
	{
		if (!rule.CanDrop(info))
		{
			ItemDropAttemptResult itemDropAttemptResult = default;
			itemDropAttemptResult.State = ItemDropAttemptResultState.DoesntFillConditions;
			ItemDropAttemptResult itemDropAttemptResult2 = itemDropAttemptResult;
			ResolveRuleChains(ref info, ref itemDropAttemptResult2, rule.ChainedRules);

			return itemDropAttemptResult2;
		}

		ItemDropAttemptResult itemDropAttemptResult3 = (rule is not INestedItemDropRule nestedItemDropRule) ? rule.TryDroppingItem(info) : nestedItemDropRule.TryDroppingItem(info, ResolveRule);
		ResolveRuleChains(ref info, ref itemDropAttemptResult3, rule.ChainedRules);

		return itemDropAttemptResult3;
	}

	private static void ResolveRuleChains(ref DropAttemptInfo info, ref ItemDropAttemptResult parentResult, List<IItemDropRuleChainAttempt> ruleChains)
	{
		if (ruleChains == null)
			return;

		for (int i = 0; i < ruleChains.Count; i++)
		{
			IItemDropRuleChainAttempt itemDropRuleChainAttempt = ruleChains[i];

			if (itemDropRuleChainAttempt.CanChainIntoRule(parentResult))
				ResolveRule(itemDropRuleChainAttempt.RuleToChain, info);
		}
	}
}