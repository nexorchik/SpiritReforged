using SpiritReforged.Common.WorldGeneration;

namespace SpiritReforged.Common.TileCommon;

public static class TileExtensions
{
	public static Vector2 TileOffset => Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
	public static Vector2 DrawPosition(this ModTile _, int i, int j, Vector2 off = default) => DrawPosition(i, j, off);
	public static Vector2 DrawPosition(int i, int j, Vector2 off = default) => new Vector2(i, j) * 16 - Main.screenPosition - off + TileOffset;

	public static void DrawSloped(this ModTile _, int i, int j, Texture2D texture, Color color, Vector2 positionOffset, bool overrideFrame = false)
		=> DrawSloped(i, j, texture, color, positionOffset, overrideFrame);

	public static void DrawSloped(int i, int j, Texture2D texture, Color color, Vector2 positionOffset, bool overrideFrame = false)
	{
		Tile tile = Main.tile[i, j];
		int frameX = tile.TileFrameX;
		int frameY = tile.TileFrameY;

		if (overrideFrame)
		{
			frameX = 0;
			frameY = 0;
		}

		int width = 16;
		int height = 16;
		var location = new Vector2(i * 16, j * 16);
		Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
		Vector2 offsets = -Main.screenPosition + zero + positionOffset;
		Vector2 drawLoc = location + offsets;

		if (tile.Slope == 0 && !tile.IsHalfBlock || Main.tileSolid[tile.TileType] && Main.tileSolidTop[tile.TileType]) //second one should be for platforms
			Main.spriteBatch.Draw(texture, drawLoc, new Rectangle(frameX, frameY, width, height), color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
		else if (tile.IsHalfBlock)
			Main.spriteBatch.Draw(texture, new Vector2(drawLoc.X, drawLoc.Y + 8), new Rectangle(frameX, frameY, width, 8), color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
		else
		{
			SlopeType b = tile.Slope;
			Rectangle frame;
			Vector2 drawPos;

			if (b is SlopeType.SlopeDownLeft or SlopeType.SlopeDownRight)
			{
				int length;
				int height2;

				for (int a = 0; a < 8; ++a)
				{
					if (b == SlopeType.SlopeDownRight)
					{
						length = 16 - a * 2 - 2;
						height2 = 14 - a * 2;
					}
					else
					{
						length = a * 2;
						height2 = 14 - length;
					}

					frame = new Rectangle(frameX + length, frameY, 2, height2);
					drawPos = new Vector2(i * 16 + length, j * 16 + a * 2) + offsets;
					Main.spriteBatch.Draw(texture, drawPos, frame, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
				}

				frame = new Rectangle(frameX, frameY + 14, 16, 2);
				drawPos = new Vector2(i * 16, j * 16 + 14) + offsets;
				Main.spriteBatch.Draw(texture, drawPos, frame, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
			}
			else
			{
				int length;
				int height2;

				for (int a = 0; a < 8; ++a)
				{
					if (b == SlopeType.SlopeUpLeft)
					{
						length = a * 2;
						height2 = 16 - length;
					}
					else
					{
						length = 16 - a * 2 - 2;
						height2 = 16 - a * 2;
					}

					frame = new Rectangle(frameX + length, frameY + 16 - height2, 2, height2);
					drawPos = new Vector2(i * 16 + length, j * 16) + offsets;
					Main.spriteBatch.Draw(texture, drawPos, frame, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
				}

				drawPos = new Vector2(i * 16, j * 16) + offsets;
				frame = new Rectangle(frameX, frameY, 16, 2);
				Main.spriteBatch.Draw(texture, drawPos, frame, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
			}
		}
	}

	/// <summary> Gets the top left tile in a multitile using the given coordinates. Useful for things like tile entities whos data is stored only in a single tile. <br/>
	/// This method relies on tileFrame to get the tile and may not work depending on how those variables are used. </summary>
	public static void GetTopLeft(ref int i, ref int j)
	{
		var tile = Framing.GetTileSafely(i, j);
		var data = TileObjectData.GetTileData(tile);
		if (data is null)
			return;

		(i, j) = (i - tile.TileFrameX % data.CoordinateFullWidth / 18, j - tile.TileFrameY % data.CoordinateFullHeight / 18);
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

	/// <summary> Checks if the tile at i, j is a chest, and returns what kind of chest it is if so. </summary>
	/// <param name="i">X position.</param>
	/// <param name="j">Y position.</param>
	/// <param name="type">The type of the chest, if any.</param>
	/// <returns>If the tile is a chest or not.</returns>
	public static bool TryGetChestID(int i, int j, out VanillaChestID type)
	{
		Tile tile = Main.tile[i, j];
		type = VanillaChestID.Wood;

		if (tile.HasTile && tile.TileType == TileID.Containers && tile.TileFrameX % 36 == 0 && tile.TileFrameY == 0)
		{
			type = (VanillaChestID)(tile.TileFrameX / 36);
			return true;
		}

		return false;
	}

	/// <summary>
	/// Quickly retrieves a given tile's data.
	/// </summary>
	/// <param name="tile">The tile to get data from.</param>
	/// <returns>The tile data.</returns>
	public static TileObjectData SafelyGetData(this Tile tile) => TileObjectData.GetTileData(tile);

	/// <summary>
	/// Mutually merges the given tile with all of the ids in <paramref name="otherIds"/>.
	/// </summary>
	/// <param name="tile">The tile to merge with.</param>
	/// <param name="otherIds">All other tiles to merge with.</param>
	public static void Merge(this ModTile tile, params int[] otherIds)
	{
		foreach (int id in otherIds)
		{
			Main.tileMerge[tile.Type][id] = true;
			Main.tileMerge[id][tile.Type] = true;
		}
	}
}
