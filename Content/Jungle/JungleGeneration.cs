using SpiritReforged.Common.WorldGeneration.Chests;
using SpiritReforged.Content.Jungle.Misc.DyeCrate;
using SpiritReforged.Content.Jungle.Toucane;

namespace SpiritReforged.Content.Jungle;

public class JungleGeneration : ModSystem
{
	public override void PostWorldGen()
	{
		ChestPoolUtils.AddToVanillaChest(new ChestPoolUtils.ChestInfo(new int[] { ModContent.ItemType<ToucaneItem>() }, 1, 0.25f), (int)VanillaChestID.Ivy, 1);
		ChestPoolUtils.AddToVanillaChest(new ChestPoolUtils.ChestInfo(new int[] { ModContent.ItemType<DyeCrateItem>() }, 1, 0.5f), (int)VanillaChestID.Ivy, 1);
		ChestPoolUtils.AddToVanillaChest(new ChestPoolUtils.ChestInfo(new int[] { ModContent.ItemType<DyeCrateItem>() }, 1, 0.33f), (int)VanillaChestID.Jungle, 1);
	}
}