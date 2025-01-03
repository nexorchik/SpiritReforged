using SpiritReforged.Common.TileCommon.FurnitureTiles;

namespace SpiritReforged.Content.Ocean.Tiles.Driftwood.Furniture;

public class DriftwoodToilet : ToiletTile
{
	public override int CoreMaterial => ModContent.ItemType<Items.Driftwood.DriftwoodTileItem>();
}
