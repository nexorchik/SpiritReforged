using SpiritReforged.Content.Underground.Items.Railgun;
using static SpiritReforged.Common.Misc.ChestPoolUtils;

namespace SpiritReforged.Content.Underground;

public class UndergroundGeneration : ModSystem
{
	public override void PostWorldGen() => AddToVanillaChest(new ChestInfo(new int[] { ModContent.ItemType<ZiplineGun>() }, 1, 0.25f), goldChests, 1);
}