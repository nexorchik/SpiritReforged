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
		SpiritClassic.AddReplacement("DriftwoodWorkbenchItem", this.AutoItem().type);
	}
}
