using SpiritReforged.Common.WorldGeneration.Chests;
using SpiritReforged.Content.Ocean.Items.PoolNoodle;
using SpiritReforged.Content.Ocean.Items.Vanity;

namespace SpiritReforged.Content.Ocean;

public class OceanChestLoot : ModSystem
{
	public override void PostWorldGen()
	{
		ChestPoolUtils.AddToVanillaChest(new ChestPoolUtils.ChestInfo(new int[] { ModContent.ItemType<PoolNoodle>() }, 1, 0.33f), (int)VanillaChestID.Water, 1);
		ChestPoolUtils.AddToVanillaChest(new ChestPoolUtils.ChestInfo(new int[] { ModContent.ItemType<BeachTowel>(), ModContent.ItemType<BikiniBottom>(), ModContent.ItemType<BikiniTop>(), ModContent.ItemType<SwimmingTrunks>(), ModContent.ItemType<TintedGlasses>() }, 1, 0.33f), (int)VanillaChestID.Water, 1);
	}
}