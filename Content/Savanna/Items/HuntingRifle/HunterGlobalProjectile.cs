using System.IO;
using Terraria.ModLoader.IO;

namespace SpiritReforged.Content.Savanna.Items.HuntingRifle;

public class HunterGlobalProjectile : GlobalProjectile
{
	private const float damageMultiplier = 2f;
	private const float maxRange = 16 * 50; //At this range or greater, our full damage multiplier will be applied

	public bool hasDistanceMultiplier;

	public override bool InstancePerEntity => true;

	public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
		=> lateInstantiation && entity.CountsAsClass(DamageClass.Ranged) && !entity.arrow;

	private float GetMultiplier(Projectile proj)
		=> hasDistanceMultiplier ? MathHelper.Clamp(Main.player[proj.owner].Distance(proj.Center) / maxRange, 0, 1) * (damageMultiplier - 1f) : 0;

	public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
		=> modifiers.SourceDamage *= 1f + GetMultiplier(projectile);
	public override void ModifyHitPlayer(Projectile projectile, Player target, ref Player.HurtModifiers modifiers)
		=> modifiers.SourceDamage *= 1f + GetMultiplier(projectile);

	//Sync hasDistanceMultiplier because it will be assigned to in local-client-only locations
	public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter) => bitWriter.WriteBit(hasDistanceMultiplier);

	public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader) => hasDistanceMultiplier = bitReader.ReadBit();
}
