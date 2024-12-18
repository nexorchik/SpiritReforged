using SpiritReforged.Common.ItemCommon;
using SpiritReforged.Common.Particle;
using SpiritReforged.Common.PlayerCommon;
using System.Linq;

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

		var nearby = Main.npc.Where(x => x.active && UndeadNPC.undeadTypes.Contains(x.type)).OrderBy(x => x.Distance(player.Center)).FirstOrDefault();

		if (nearby != default)
		{
			float mult = MathHelper.Clamp(1f - nearby.Distance(player.Center) / 1200f, 0, 1);
			var color = Color.Yellow.ToVector3() * .34f * mult;

			Lighting.AddLight(player.Center, color.X, color.Y, color.Z);

			if (!hideVisual && mult > .75f && Main.rand.NextBool(30))
			{
				var position = Main.rand.NextVector2FromRectangle(player.getRect());
				var newCol = Color.Lerp(Color.Gold, Color.Orange, Main.rand.NextFloat());

				ParticleHandler.SpawnParticle(new Particles.GlowParticle(position, Vector2.UnitY * -Main.rand.NextFloat(.5f, 1f), newCol, Main.rand.NextFloat(.2f, .4f), 60, 20));
			}
		}
	}
}

public class UndeadModPlayer : ModPlayer
{
	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		if (Player.HasAccessory<SafekeeperRing>() && UndeadNPC.undeadTypes.Contains(target.type))
			modifiers.FinalDamage *= 1.25f;
	}
}
