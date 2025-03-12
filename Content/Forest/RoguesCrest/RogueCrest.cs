using SpiritReforged.Common.ItemCommon;
using Terraria.GameContent.ItemDropRules;

namespace SpiritReforged.Content.Forest.RoguesCrest;

[AutoloadEquip(EquipType.Neck)]
public class RogueCrest : MinionAccessory
{
	public override MinionAccessoryData Data => new(ModContent.ProjectileType<RogueKnifeMinion>(), 5);

	public override void StaticDefaults()
	{
		CrateDatabase.AddCrateRule(ItemID.WoodenCrate, ItemDropRule.Common(Type, 4));
		CrateDatabase.AddCrateRule(ItemID.WoodenCrateHard, ItemDropRule.Common(Type, 4));
	}

	public override void Defaults()
	{
		Item.width = 38;
		Item.height = 36;
		Item.value = Item.buyPrice(0, 3, 0, 0);
		Item.rare = ItemRarityID.Green;
		Item.defense = 1;
	}
}