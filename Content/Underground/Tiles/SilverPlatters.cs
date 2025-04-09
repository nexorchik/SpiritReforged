using SpiritReforged.Common.TileCommon.PresetTiles;

namespace SpiritReforged.Content.Underground.Tiles;

public class SilverPlatters : PotTile, ILootTile
{
	public override Dictionary<string, int[]> TileStyles => new() { { string.Empty, [0, 1, 2] } };

	public LootTable AddLoot(int objectStyle)
	{
		var loot = new LootTable();
		return loot;
	}
}