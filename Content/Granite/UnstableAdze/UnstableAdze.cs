using SpiritReforged.Common.ItemCommon;
using SpiritReforged.Common.NPCCommon;
using SpiritReforged.Common.Visuals.Glowmasks;
using Terraria.GameContent.ItemDropRules;

namespace SpiritReforged.Content.Granite.UnstableAdze;

[AutoloadGlowmask("255, 255, 255")]
public class UnstableAdze : ClubItem
{
	internal override float DamageScaling => 1.5f;

	public override void SetStaticDefaults()
	{
		NPCLootDatabase.AddLoot(new(NPCLootDatabase.MatchId(NPCID.GraniteFlyer), ItemDropRule.Common(Type, 20)));
		NPCLootDatabase.AddLoot(new(NPCLootDatabase.MatchId(NPCID.GraniteGolem), ItemDropRule.Common(Type, 20)));
	}

	public override void SafeSetDefaults()
	{
		Item.damage = 53;
		Item.knockBack = 8;
		ChargeTime = 40;
		SwingTime = 30;
		Item.width = 60;
		Item.height = 60;
		Item.crit = 4;
		Item.value = Item.sellPrice(0, 0, 30, 0);
		Item.rare = ItemRarityID.Blue;
		Item.shoot = ModContent.ProjectileType<UnstableAdzeProj>();
	}
}