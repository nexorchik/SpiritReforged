using SpiritReforged.Common.NPCCommon;

namespace SpiritReforged.Content.Buffs.SummonTag;

public class SummonTag3 : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("Summon Tag");
		Main.debuff[Type] = true;
		Main.pvpBuff[Type] = true;
		Main.buffNoTimeDisplay[Type] = false;
	}

	public override void Update(NPC npc, ref int buffIndex) => npc.GetGlobalNPC<SummontTagGlobalNPC>().summonTag = 3;
}