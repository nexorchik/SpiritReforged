using System.Linq;

namespace SpiritReforged.Common.TileCommon;

/// <summary> Includes helper methods related to placing tiles. </summary>
public static class Placer
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
	public static bool PlaceTile(int i, int j, int type, int style = -1)
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
				style = Main.rand.Next(data.RandomStyleRange);
		}

		WorldGen.PlaceTile(i, j, type, true, style: style);

		if (Main.tile[i, j].TileType == type)
		{
			if (Main.netMode != NetmodeID.SinglePlayer)
			{
				TileExtensions.GetTopLeft(ref i, ref j);
				NetMessage.SendTileSquare(-1, i, j, width, height);
			}

			return true;
		}

		return false;
	}

	///<inheritdoc cref="PlaceTile(int, int, int, int)"/>
	public static bool PlaceTile<T>(int i, int j, int style = -1) where T : ModTile => PlaceTile(i, j, ModContent.TileType<T>(), style);

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

	/// <summary> Tries to place or extend a vine at the given coordinates. </summary>
	/// <param name="i"> The tile's X coordinate. </param>
	/// <param name="j"> The tile's Y coordinate. </param>
	/// <param name="type"> The tile's type. </param>
	/// <param name="maxLength"> The maximum length this vine can grow. Does NOT instantly grow a vine of the given length. </param>
	/// <param name="reversed"> Whether this vine grows from the ground up. </param>
	/// <param name="sync"> Whether the tile changes should be automatically synced. </param>
	/// <returns> Whether the tile was successfully placed. </returns>
	public static bool GrowVine(int i, int j, int type, int maxLength = 15, bool reversed = false, bool sync = true)
	{
		if (reversed)
		{
			while (Main.tile[i, j + 1].HasTile && Main.tile[i, j + 1].TileType == type)
				j++; //Move to the bottom of the vine

			for (int x = 0; x < maxLength; x++)
			{
				if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == type)
					j--; //Move to the next available tile above
			}
		}
		else
		{
			while (Main.tile[i, j - 1].HasTile && Main.tile[i, j - 1].TileType == type)
				j--; //Move to the top of the vine

			for (int x = 0; x < maxLength; x++)
			{
				if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == type)
					j++; //Move to the next available tile below
			}
		}

		if (Main.tile[i, j].TileType == type)
			return false; //The tile already exists; we've hit the max length

		WorldGen.PlaceObject(i, j, type, true);

		if (Main.tile[i, j].TileType != type)
			return false; //Tile placement failed

		if (Main.netMode != NetmodeID.SinglePlayer && sync)
			NetMessage.SendTileSquare(-1, i, j, 1, 1);

		return true;
	}
}