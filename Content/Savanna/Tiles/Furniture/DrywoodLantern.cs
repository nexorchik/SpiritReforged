using SpiritReforged.Common.TileCommon.PresetTiles;

namespace SpiritReforged.Content.Savanna.Tiles.Furniture;

public class DrywoodLantern : LanternTile
{
	public override int CoreMaterial => ModContent.ItemType<Items.Drywood.Drywood>();
	public override bool BlurGlowmask => false;
}
