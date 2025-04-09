
using SpiritReforged.Content.Ocean.Items.JellyfishStaff;
using SpiritReforged.Content.Vanilla.Food;
using Terraria.ModLoader;

namespace SpiritReforged.Common.ModCompat;

internal class ThoriumCompat : ModSystem
{
	public static Mod Instance;
	public static bool Enabled => Instance != null;

	public override void Load()
	{
		Instance = null;
		if (!ModLoader.TryGetMod("ThoriumMod", out Instance))
			return;
	}
}

internal class ThoriumGlobalNPC : GlobalNPC
{
	public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
	{
		if (ThoriumCompat.Enabled)
		{
			if (ThoriumCompat.Instance.TryFind("RagingMinotaur", out ModNPC minotaur))
			{
				if (npc.type == minotaur.Type)
					npcLoot.AddCommon(ItemID.Leather, 1, 5, 10);
			}

			if (ThoriumCompat.Instance.TryFind("ManofWar", out ModNPC bigJellyfish))
			{
				if (npc.type == bigJellyfish.Type)
					npcLoot.AddCommon(ModContent.ItemType<JellyfishStaff>(), 50);
			}
		}
	}
}