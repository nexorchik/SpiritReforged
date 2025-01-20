using SpiritReforged.Common.Particle;
using SpiritReforged.Common.PlayerCommon;
using SpiritReforged.Content.Particles;
using Terraria.Audio;

namespace SpiritReforged.Content.Forest.ArcaneNecklace;

internal class ArcaneNecklaceGlobalNPC : GlobalNPC
{
	public override void ModifyHitByItem(NPC npc, Player player, Item item, ref NPC.HitModifiers modifiers)
	{
		if (item.CountsAsClass(DamageClass.Magic))
			TryDropStar(player, npc);
	}

	public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
	{
		if (projectile.CountsAsClass(DamageClass.Magic))
			TryDropStar(Main.player[projectile.owner], npc);
	}

	private static void TryDropStar(Player player, NPC target)
	{
		if (target.type == NPCID.TargetDummy)
			return;

		if ((player.HasAccessory<ArcaneNecklaceGold>() || player.HasAccessory<ArcaneNecklacePlatinum>()) && player.statMana < player.statManaMax2 && Main.rand.NextBool(15))
		{
			SoundEngine.PlaySound(SoundID.Item9 with { PitchRange = (-0.5f, -0.1f), Volume = .35f }, target.Center);
			SoundEngine.PlaySound(SoundID.DD2_LightningBugZap with { PitchRange = (0.65f, 0.8f) }, target.Center);

			for (int i = 0; i < 4; i++)
			{
				ParticleHandler.SpawnParticle(new GlowParticle(target.Center, Main.rand.NextVector2CircularEdge(1, 1) * Main.rand.NextFloat(1f, 2f), new Color(74, 110, 255), Main.rand.NextFloat(0.3f, 0.5f), Main.rand.Next(30, 50), 12, delegate (Particle p) { p.Velocity.Y *= -1f; }));
				ParticleHandler.SpawnParticle(new GlowParticle(target.Center, Main.rand.NextVector2CircularEdge(1, 1) * Main.rand.NextFloat(1f, 2f), new Color(74, 110, 255), Main.rand.NextFloat(0.1f, 0.3f), Main.rand.Next(30, 50), 12, delegate (Particle p) { p.Velocity.Y -= .025f; }));
			}

			target.DropItemInstanced(target.position, target.Size, ItemID.Star);
		}
	}
}
