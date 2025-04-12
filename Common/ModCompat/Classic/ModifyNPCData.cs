using Terraria.GameContent.ItemDropRules;

namespace SpiritReforged.Common.ModCompat.Classic;

/// <summary> Modifies Classic NPC shops and drop tables. </summary>
[Autoload(false)]
internal class ModifyNPCData : GlobalNPC
{
	public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
	{
		if (npc.type == NPCID.PirateShip && SpiritClassic.ClassicMod.TryFind("PirateKey", out ModItem key))
			npcLoot.RemoveWhere(rule => rule is CommonDrop drop && drop.itemId == key.Type);

		if (SpiritClassic.ClassicMod.TryFind("RawFish", out ModItem fish))
			npcLoot.RemoveWhere(rule => rule is CommonDrop drop && drop.itemId == fish.Type);
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
