using System.Linq;

namespace SpiritReforged.Common.TileCommon;

public static class TilePlaceHelper
{
	private static readonly int[] Replaceable = [TileID.Plants, TileID.Plants2, TileID.JunglePlants, TileID.JunglePlants2, 
		TileID.CorruptPlants, TileID.CrimsonPlants, TileID.HallowedPlants, TileID.HallowedPlants2];

	/// <returns> Whether the tile at the given coordinates is generally replaceable and safe to be cleared. Useful for random plant growth. </returns>
	public static bool IsReplaceable(int i, int j)
	{
		var tile = Main.tile[i, j];

		if (!tile.HasTile)
			return true;

		if (TileObjectData.GetTileData(tile.TileType, 0) != null)
			return false;

		return Replaceable.Contains(tile.TileType) || Main.tileCut[tile.TileType] && TileID.Sets.BreakableWhenPlacing[tile.TileType];
	}

	/// <summary> Places a tile of <paramref name="type"/> at the given coordinates and automatically syncs it if necessary. </summary>
	/// <param name="type"> The tile type to place. </param>
	/// <param name="style"> The tile style to place. -1 tries to place a random style. </param>
	public static void PlaceTile(int i, int j, int type, int style = -1)
	{
		int width = 1;
		int height = 1;
		var data = TileObjectData.GetTileData(type, 0);

		if (data is null)
			style = 0;
		else
		{
			width = data.Width;
			height = data.Height;

			if (style == -1)
				style = data.RandomStyleRange;
		}

		WorldGen.PlaceTile(i, j, type, true, style: style);

		if (Main.tile[i, j].TileType == type && Main.netMode != NetmodeID.SinglePlayer)
		{
			TileExtensions.GetTopLeft(ref i, ref j);
			NetMessage.SendTileSquare(-1, i, j, width, height);
		}
	}

	/// <summary> Checks the surrounding area for herbs of <paramref name="type"/>.</summary>
	/// <returns> true if fewer than 4 herbs are in range. </returns>
	public static bool CanPlaceHerb(int i, int j, int type)
	{
		int radius = WorldGen.GetWorldSize() switch
		{
			1 => 45,
			2 => 61,
			_ => 31
		};

		return WorldGen.CountNearBlocksTypes(i, j, radius, 4, type) < 4;
	}
}
