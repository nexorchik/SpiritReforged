using SpiritReforged.Common.Particle;
using SpiritReforged.Common.PlayerCommon;
using SpiritReforged.Content.Particles;
using Terraria.Audio;

namespace SpiritReforged.Content.Forest.ArcaneNecklace;

public class ArcaneNecklaceGlobalNPC : GlobalNPC
{
	public void DropStar(NPC target)
	{
		if (Main.rand.NextBool(15) && target.type != NPCID.TargetDummy)
		{
			SoundEngine.PlaySound(SoundID.Item9 with { PitchRange = (-0.5f, -0.1f), Volume = .35f }, target.Center);
			SoundEngine.PlaySound(SoundID.DD2_LightningBugZap with { PitchRange = (0.65f, 0.8f), Volume = 1f }, target.Center);

			for (int i = 0; i < 4; i++)
			{
				ParticleHandler.SpawnParticle(new GlowParticle(target.Center, Main.rand.NextVector2CircularEdge(1, 1) * Main.rand.NextFloat(1f, 2f), new Color(74, 110, 255), Main.rand.NextFloat(0.3f, 0.5f), Main.rand.Next(30, 50), 12, delegate (Particle p) { p.Velocity.Y *= -1f; }));
				ParticleHandler.SpawnParticle(new GlowParticle(target.Center, Main.rand.NextVector2CircularEdge(1, 1) * Main.rand.NextFloat(1f, 2f), new Color(74, 110, 255), Main.rand.NextFloat(0.1f, 0.3f), Main.rand.Next(30, 50), 12, delegate (Particle p) { p.Velocity.Y -= .025f; }));
			}

			target.DropItemInstanced(target.position, target.Size, ItemID.Star);
		}
	}

	public override void ModifyHitByItem(NPC npc, Player player, Item item, ref NPC.HitModifiers modifiers)
	{
		if ((player.HasAccessory<ArcaneNecklaceGold>() || player.HasAccessory<ArcaneNecklacePlatinum>()) && item.CountsAsClass(DamageClass.Magic) && player.statMana < player.statManaMax2)
			DropStar(npc);
	}

	public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
	{
		Player player = Main.player[projectile.owner];
		if ((player.HasAccessory<ArcaneNecklaceGold>() || player.HasAccessory<ArcaneNecklacePlatinum>()) && projectile.CountsAsClass(DamageClass.Magic) && player.statMana < player.statManaMax2)
			DropStar(npc);
	}
}
