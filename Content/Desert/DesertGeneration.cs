using SpiritReforged.Common.WorldGeneration.Chests;

namespace SpiritReforged.Content.Desert;

public class DesertGeneration : ModSystem
{
	public override void PostWorldGen() => ChestPoolUtils.AddToVanillaChest(new ChestPoolUtils.ChestInfo(new int[] { ModContent.ItemType<GildedScarab.GildedScarab>() }, 1, 0.25f), (int)VanillaChestID2.Sandstone, 1, TileID.Containers2);
}