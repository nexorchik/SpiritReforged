using SpiritReforged.Content.Ocean.Items.JellyCandle;

namespace SpiritReforged.Content.Ocean;

internal class OceanGlobalNPC : GlobalNPC
{
	public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
	{
		//assumedly will add more here
		npcLoot.AddCommon(ModContent.ItemType<JellyCandle>(), 50);
	}
}