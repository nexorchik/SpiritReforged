using SpiritReforged.Common.TileCommon.FurnitureTiles;

namespace SpiritReforged.Content.Ocean.Tiles.Driftwood.Furniture;

public class DriftwoodBed : BedTile
{
	public override int CoreMaterial => ModContent.ItemType<Items.Driftwood.DriftwoodTileItem>();
}
