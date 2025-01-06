using Terraria.DataStructures;

namespace SpiritReforged.Common.TileCommon.Corruption;

internal static class TileCorruptor
{
	public static void Convert(IEntitySource source, ConversionType type, int i, int j)
	{
		Tile tile = Main.tile[i, j];

		if (tile.TileType > TileID.Count && ModContent.GetModTile(tile.TileType) is IConvertibleTile conv && conv.Convert(source, type, i, j))
			NetMessage.SendTileSquare(-1, i, j);
	}
}
