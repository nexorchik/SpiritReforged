using System.Linq;
using Terraria.DataStructures;

namespace SpiritReforged.Common.TileCommon;

public struct PlaceAttempt(bool success)
{
	public bool success = success;
	/// <summary> Can only be safely used if <see cref="success"/> is true. </summary>
	public TileObject data;

	public readonly Point16 Coords => new(data.xCoord, data.yCoord);
}

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

	/// <summary> Places a tile of <paramref name="type"/> at the given coordinates and returns the resulting <see cref="PlaceAttempt"/>.<para/>
	/// <see cref="Placer"/> Contains various methods to chain for additional functionality. </summary>
	/// <param name="type"> The tile type to place. </param>
	/// <param name="style"> The tile style to place. -1 tries to place a random style. </param>
	public static PlaceAttempt PlaceTile(int i, int j, int type, int style = -1)
	{
		var data = TileObjectData.GetTileData(type, 0);

		if (data is null)
			style = 0;
		else if (style == -1)
			style = Main.rand.Next(data.RandomStyleRange);

		if (TileObject.CanPlace(i, j, type, style, 0, out var objectData))
		{
			if (TileObject.Place(objectData) && Main.tile[i, j].TileType == type)
				return new(true) { data = objectData };
		}

		return new(false);
	}

	/// <summary> Calls <see cref="TileObjectData.CallPostPlacementPlayerHook"/> for this attempt, and outputs entity of T. </summary>
	public static PlaceAttempt PostPlacement<T>(this PlaceAttempt a, out T entity) where T : class
	{
		if (a.success)
		{
			var t = Framing.GetTileSafely(a.Coords);
			var data = TileObjectData.GetTileData(t);

			if (data != null)
			{
				TileObjectData.CallPostPlacementPlayerHook(a.data.xCoord, a.data.yCoord, a.data.type, a.data.style, 1, a.data.alternate, a.data);

				int i = a.Coords.X;
				int j = a.Coords.Y;

				TileExtensions.GetTopLeft(ref i, ref j);

				if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out var value) && value is T valueOfType)
				{
					entity = valueOfType;
					return a;
				}
			}
		}

		entity = null;
		return a with { success = false };
	}

	/// <summary> Calls <see cref="NetMessage.SendTileSquare"/> for this attempt.<br/>
	/// This is almost always necessary for tiles placed during gameplay to sync in multiplayer. </summary>
	public static PlaceAttempt Send(this PlaceAttempt a)
	{
		if (a.success)
		{
			var data = TileObjectData.GetTileData(a.data.type, 0);

			if (Main.netMode != NetmodeID.SinglePlayer && data != null)
			{
				int i = a.Coords.X;
				int j = a.Coords.Y;

				TileExtensions.GetTopLeft(ref i, ref j);
				NetMessage.SendTileSquare(-1, i, j, data.Width, data.Height);
			}
		}

		return a;
	}

	///<inheritdoc cref="PlaceTile(int, int, int, int)"/>
	public static PlaceAttempt PlaceTile<T>(int i, int j, int style = -1) where T : ModTile => PlaceTile(i, j, ModContent.TileType<T>(), style);

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