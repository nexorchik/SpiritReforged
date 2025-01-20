using SpiritReforged.Common.TileCommon.PresetTiles;

namespace SpiritReforged.Content.Ocean.Tiles.Driftwood.Furniture;

public class DriftwoodBed : BedTile
{
	public override int CoreMaterial => ModContent.ItemType<Items.Driftwood.DriftwoodTileItem>();
}
