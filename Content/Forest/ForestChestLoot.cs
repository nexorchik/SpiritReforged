using SpiritReforged.Common.Misc;
using SpiritReforged.Common.WorldGeneration;
using SpiritReforged.Content.Forest.ArcaneNecklace;
using SpiritReforged.Content.Forest.Cloudstalk.Items;
using SpiritReforged.Content.Forest.Misc;
using SpiritReforged.Content.Forest.Misc.Maps;
using SpiritReforged.Content.Forest.RoguesCrest;

namespace SpiritReforged.Content.Underground;

public class ForestChestLoot : ModSystem
{
	public override void PostWorldGen()
	{
		ChestPoolUtils.AddToVanillaChest(new ChestPoolUtils.ChestInfo(new int[] { ModContent.ItemType<RogueCrest>(), ModContent.ItemType<CraneFeather>() }, 1, 0.25f), (int)VanillaChestID.Wood, 1);
		ChestPoolUtils.AddToVanillaChest(new ChestPoolUtils.ChestInfo(new int[] { ModContent.ItemType<ArcaneNecklaceGold>(), ModContent.ItemType<ArcaneNecklacePlatinum>() }, 1, 0.125f), (int)VanillaChestID.Wood, 1);
		ChestPoolUtils.AddToVanillaChest(new ChestPoolUtils.ChestInfo(new int[] { ModContent.ItemType<DoubleJumpPotion>() }, 3, 0.35f), (int)VanillaChestID.Wood, Main.rand.Next(1, 3));
		ChestPoolUtils.AddToVanillaChest(new ChestPoolUtils.ChestInfo(new int[] { ModContent.ItemType<TornMapPiece>() }, 2, 0.2f), (int)VanillaChestID.Wood, Main.rand.Next(1, 3));
	}
}