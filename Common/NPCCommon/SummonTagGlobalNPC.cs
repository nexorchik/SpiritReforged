namespace SpiritReforged.Common.NPCCommon;

public class SummontTagGlobalNPC : GlobalNPC
{
	public int summonTag;

	public override bool InstancePerEntity => true;

	public override void ResetEffects(NPC npc) => summonTag = 0;

	public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
	{
		bool summon = projectile.minion || ProjectileID.Sets.MinionShot[projectile.type] || ProjectileID.Sets.SentryShot[projectile.type] || projectile.sentry;

		if (summon)
			modifiers.FinalDamage.Flat += summonTag;
	}
}
