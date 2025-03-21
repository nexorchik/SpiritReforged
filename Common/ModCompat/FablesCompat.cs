using SpiritReforged.Common.ItemCommon.Backpacks;
using SpiritReforged.Common.WorldGeneration.Micropasses.Passes;
using SpiritReforged.Common.WorldGeneration.PointOfInterest;
using SpiritReforged.Content.Forest.Backpacks;
using SpiritReforged.Content.Forest.Botanist.Items;
using SpiritReforged.Content.Forest.Botanist.Tiles;
using SpiritReforged.Content.Forest.Stargrass.Tiles;
using SpiritReforged.Content.Ocean.Items.KoiTotem;
using SpiritReforged.Content.Ocean.Items.Reefhunter.OceanPendant;
using SpiritReforged.Content.Ocean.Items.Vanity.DiverSet;
using SpiritReforged.Content.Savanna.Items.HuntingRifle;
using SpiritReforged.Content.Savanna.Items.Vanity;
using SpiritReforged.Content.Savanna.Tiles;
using Terraria.DataStructures;
using Terraria.WorldBuilding;

namespace SpiritReforged.Common.ModCompat;

internal class FablesCompat : ModSystem
{
	public static Mod Instance;
	public static bool Enabled => Instance != null;

	public override void Load()
	{
		Instance = null;
		if (!ModLoader.TryGetMod("CalamityFables", out Instance))
			return;
	}
}
