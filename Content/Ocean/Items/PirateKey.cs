using SpiritReforged.Common.NPCCommon;
using Terraria.GameContent.ItemDropRules;

namespace SpiritReforged.Content.Ocean.Items;

public class PirateKey : ModItem
{
	public override bool IsLoadingEnabled(Mod mod) => false;

	public override void SetStaticDefaults()
	{
		Item.ResearchUnlockCount = 1;
		NPCLootDatabase.AddLoot(new(NPCLootDatabase.MatchId(NPCID.PirateShip), ItemDropRule.Common(Type)));
	}

	public override void SetDefaults()
	{
		Item.width = 14;
		Item.height = 20;
		Item.maxStack = Item.CommonMaxStack;
		Item.rare = ItemRarityID.Pink;
		Item.value = Item.buyPrice(0, 5, 0, 0);
	}
}