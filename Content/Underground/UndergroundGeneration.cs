using SpiritReforged.Content.Underground.Railgun;
using SpiritReforged.Common.Misc;
using SpiritReforged.Common.WorldGeneration;

namespace SpiritReforged.Content.Underground;

public class UndergroundGeneration : ModSystem
{
	public override void PostWorldGen() => ChestPoolUtils.AddToVanillaChest(new ChestPoolUtils.ChestInfo(new int[] { ModContent.ItemType<ZiplineGun>(), ModContent.ItemType<ExplorerTreads.ExplorerTreads>() }, 1, 0.25f), (int)VanillaChestID.Gold, 1);
}