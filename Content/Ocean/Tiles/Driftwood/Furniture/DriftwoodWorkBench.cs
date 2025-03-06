using SpiritReforged.Common.ModCompat.Classic;
using SpiritReforged.Common.TileCommon.PresetTiles;

namespace SpiritReforged.Content.Ocean.Tiles.Driftwood.Furniture;

public class DriftwoodWorkBench : WorkBenchTile
{
	public override int CoreMaterial => ModContent.ItemType<Items.Driftwood.DriftwoodTileItem>();

	public override void StaticDefaults()
	{
		base.StaticDefaults();

		if (SpiritClassic.Enabled && SpiritClassic.ClassicMod.TryFind("DriftwoodWorkbenchItem", out ModItem workbench))
			SpiritClassic.ClassicToReforged.Add(workbench.Type, Mod.Find<ModItem>("DriftwoodWorkBenchItem").Type);
	}
}
