using SpiritReforged.Common.ItemCommon;
using SpiritReforged.Common.ModCompat.Classic;
using SpiritReforged.Common.TileCommon.PresetTiles;

namespace SpiritReforged.Content.Ocean.Tiles.Furniture;

public class DriftwoodWorkBench : WorkBenchTile
{
	public override int CoreMaterial => ItemMethods.AutoItemType<Driftwood>();

	public override void StaticDefaults()
	{
		base.StaticDefaults();

		//Manually include for Classic compat because this item is autoloaded
		if (SpiritClassic.Enabled && SpiritClassic.ClassicMod.TryFind("DriftwoodWorkbenchItem", out ModItem workbench))
			SpiritClassic.ClassicToReforged.Add(workbench.Type, this.AutoItem().type);
	}
}
