using static SpiritReforged.Common.Misc.ReforgedMultiplayer;

namespace SpiritReforged.Common.NPCCommon;

public class SummontTagGlobalNPC : GlobalNPC
{
	public override bool InstancePerEntity => true;

	/// <summary> The common duration for summon tag bonuses. </summary>
	public const int Duration = 240;

	private int _summonTag;
	private int _duration;

	/// <summary> Applies summon tag to an NPC. Use <see cref="NPCHelper.ApplySummonTag"/> instead. </summary>
	public void ApplySummonTag(int damage, int npcIndex, bool sync = true)
	{
		_summonTag = damage;
		_duration = Duration;

		if (sync && Main.netMode != NetmodeID.SinglePlayer)
			SendTagPacket(npcIndex, damage);
	}

	public static void SendTagPacket(int npcIndex, int damage, int ignoreClient = -1)
	{
		ModPacket packet = SpiritReforgedMod.Instance.GetPacket(MessageType.SummonTag, 3);
		packet.Write(npcIndex);
		packet.Write((byte)damage);
		packet.Send(-1, ignoreClient);
	}

	public override void ResetEffects(NPC npc)
	{
		if (--_duration <= 0)
			_summonTag = 0;
	}

	public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
	{
		if (projectile.IsMinionOrSentryRelated)
			modifiers.FinalDamage.Flat += _summonTag;
	}
}
