using Terraria.GameContent.ItemDropRules;

namespace SpiritReforged.Common.ModCompat.Classic;

/// <summary> Modifies Classic NPC shops and drop tables. </summary>
[Autoload(false)]
internal class ModifyNPCData : GlobalNPC
{
	public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
	{
		foreach (var rule in npcLoot.Get())
		{
			if (rule is CommonDrop drop && SpiritClassic.ClassicToReforged.TryGetValue(drop.itemId, out int reforged))
				drop.itemId = reforged;
		}
	}

	public override void ModifyShop(NPCShop shop)
	{
		foreach (var entry in shop.ActiveEntries)
		{
			if (SpiritClassic.ClassicToReforged.TryGetValue(entry.Item.type, out int reforgedType))
				entry.Item.ChangeItemType(reforgedType);
		}
	}
}
