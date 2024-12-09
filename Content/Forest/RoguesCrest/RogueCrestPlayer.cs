using SpiritReforged.Common.PlayerCommon;
using SpiritReforged.Content.Forest.RoguesCrest;

namespace SpiritReforged.Content.Forest.RoguesCrest;

public class RogueCrestPlayer : ModPlayer
{
	public override void PostUpdateEquips()
	{
		if (Player.HasAccessory<RogueCrest>() && Player.ownedProjectileCounts[ModContent.ProjectileType<RogueKnifeMinion>()] < 1)
			Projectile.NewProjectile(Terraria.Entity.GetSource_NaturalSpawn(), Player.Center, Vector2.Zero, ModContent.ProjectileType<RogueKnifeMinion>(), (int)Player.GetDamage(DamageClass.Summon).ApplyTo(5), .5f, Player.whoAmI);
	}
}
