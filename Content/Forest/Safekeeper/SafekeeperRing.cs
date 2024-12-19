using SpiritReforged.Common.ItemCommon;
using SpiritReforged.Common.Particle;
using SpiritReforged.Common.PlayerCommon;
using System.Linq;
using SpiritReforged.Content.Particles;
using Terraria.Audio;

namespace SpiritReforged.Content.Forest.Safekeeper;

//[AutoloadEquip(EquipType.HandsOn)]
public class SafekeeperRing : AccessoryItem
{
	public override void SetDefaults()
	{
		Item.width = Item.height = 12;
		Item.value = Item.buyPrice(gold: 2, silver: 50);
		Item.rare = ItemRarityID.Green;
		Item.accessory = true;
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		base.UpdateAccessory(player, hideVisual);

		var nearby = Main.npc.Where(x => x.active && UndeadNPC.IsUndeadType(x.type)).OrderBy(x => x.Distance(player.Center)).FirstOrDefault();

		if (nearby != default)
		{
			float mult = MathHelper.Clamp(1f - nearby.Distance(player.Center) / 1200f, 0, 1);
			var color = Color.Yellow.ToVector3() * .34f * mult;

			Lighting.AddLight(player.Center, color.X, color.Y, color.Z);

			if (!hideVisual && mult > .75f && Main.rand.NextBool(30))
			{
				var position = Main.rand.NextVector2FromRectangle(player.getRect());
				var newCol = Color.Lerp(Color.Gold, Color.Orange, Main.rand.NextFloat());

				ParticleHandler.SpawnParticle(new GlowParticle(position, Vector2.UnitY * -Main.rand.NextFloat(.5f, 1f), newCol, Main.rand.NextFloat(.2f, .4f), 60, 20));
			}
		}
	}
}

public class UndeadModPlayer : ModPlayer
{
	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		if (Player.HasAccessory<SafekeeperRing>() && UndeadNPC.IsUndeadType(target.type))
			modifiers.FinalDamage *= 1.25f;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		const int particleTime = 24;

		if (!Main.dedServ && Player.HasAccessory<SafekeeperRing>() && UndeadNPC.IsUndeadType(target.type) && target.life < MathHelper.Min(100, target.lifeMax * .25f))
		{
			var start = Main.rand.NextVector2FromRectangle(target.getRect());
			var end = Main.rand.NextVector2FromRectangle(target.getRect());

			ParticleHandler.SpawnParticle(new GlowParticle(start, Vector2.Zero, Color.White, Color.DarkGoldenrod, .5f, particleTime));
			ParticleHandler.SpawnParticle(new GlowParticle(end, Vector2.Zero, Color.White, Color.DarkGoldenrod, .5f, particleTime));

			ParticleHandler.SpawnParticle(new LightningParticle(start, end, Color.Yellow, particleTime, 10f));

			SoundEngine.PlaySound(SoundID.DD2_LightningBugZap with { Pitch = .5f }, target.Center);
		}
	}
}
