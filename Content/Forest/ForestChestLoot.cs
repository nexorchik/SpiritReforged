using SpiritReforged.Common.Misc;
using SpiritReforged.Content.Forest.ArcaneNecklace;
using SpiritReforged.Content.Forest.Cloudstalk.Items;
using SpiritReforged.Content.Forest.RoguesCrest;

namespace SpiritReforged.Content.Underground;

public class ForestChestLoot : ModSystem
{
	public override void PostWorldGen()
	{
		ChestPoolUtils.AddToVanillaChest(new ChestPoolUtils.ChestInfo(new int[] { ModContent.ItemType<RogueCrest>() }, 1, 0.25f), ChestPoolUtils.woodChests, 1);
		ChestPoolUtils.AddToVanillaChest(new ChestPoolUtils.ChestInfo(new int[] { ModContent.ItemType<ArcaneNecklaceGold>(), ModContent.ItemType<ArcaneNecklacePlatinum>() }, 1, 0.125f), ChestPoolUtils.woodChests, 1);
		ChestPoolUtils.AddToVanillaChest(new ChestPoolUtils.ChestInfo(new int[] { ModContent.ItemType<DoubleJumpPotion>() }, Main.rand.Next(1, 3), 0.35f), ChestPoolUtils.woodChests, 4);
	}
}