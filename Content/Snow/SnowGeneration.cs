using SpiritReforged.Common.Misc;
using SpiritReforged.Common.WorldGeneration;
using SpiritReforged.Content.Snow.Frostbite; 

namespace SpiritReforged.Content.Snow;

public class SnowGeneration : ModSystem
{
	public override void PostWorldGen() => ChestPoolUtils.AddToVanillaChest(new ChestPoolUtils.ChestInfo(new int[] { ModContent.ItemType<FrostbiteItem>() }, 1, 0.25f), (int)VanillaChestID.Ice, 1);
}