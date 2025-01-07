using SpiritReforged.Common.Misc;
using SpiritReforged.Content.Forest.ArcaneNecklace;
using SpiritReforged.Content.Forest.RoguesCrest;

namespace SpiritReforged.Content.Underground;

public class ForestChestLoot : ModSystem
{
    public override void PostWorldGen() => ChestPoolUtils.AddToVanillaChest(new ChestPoolUtils.ChestInfo(new int[] { ModContent.ItemType<RogueCrest>(), ModContent.ItemType<ArcaneNecklaceItem>() }, 1, 0.25f), ChestPoolUtils.woodChests, 1);
}