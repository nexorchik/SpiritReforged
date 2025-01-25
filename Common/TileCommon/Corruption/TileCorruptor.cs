using Terraria.DataStructures;

namespace SpiritReforged.Common.TileCommon.Corruption;

internal static class TileCorruptor
{
	/// <summary> Converts a single tile at the given coordinates using the <paramref name="type"/> conversion. </summary>
	/// <param name="source"> The source that converted this tile. Common examples include <see cref="EntitySource_Parent"/> and <see cref="EntitySource_TileUpdate"/>. </param>
	/// <param name="type"> The conversion type. </param>
	/// <param name="i"> The X tile coordinate. </param>
	/// <param name="j"> The Y tile coordinate. </param>
	/// <returns> Whether conversion was successful. </returns>
	public static bool Convert(IEntitySource source, ConversionType type, int i, int j)
	{
		Tile tile = Main.tile[i, j];

		if (tile.HasTile && ModContent.GetModTile(tile.TileType) is IConvertibleTile conv)
		{
			int oldType = Main.tile[i, j].TileType;
			if (conv.Convert(source, type, i, j))
			{
				if (oldType != Main.tile[i, j].TileType)
					WorldGen.SquareTileFrame(i, j);

				if (Main.netMode == NetmodeID.MultiplayerClient)
					NetMessage.SendTileSquare(-1, i, j);
			}
		}

		return false;
	}

	/// <inheritdoc cref="GetConversionType(int, ConversionType, int, int, int, int, out int)"/>
	/// <typeparam name="TPure">The class of the pure tile for reference as an ID.</typeparam>
	/// <typeparam name="TCorr">The class of the corrupt tile for reference as an ID.</typeparam>
	/// <typeparam name="TCrim">The class of the crimson tile for reference as an ID.</typeparam>
	/// <typeparam name="THall">The class of the hallowed tile for reference as an ID.</typeparam>
	public static bool GetConversionType<TPure, TCorr, TCrim, THall>(int type, ConversionType conversion, out int newId) 
		where TPure : ModTile where TCorr : ModTile where TCrim : ModTile where THall : ModTile 
		=> GetConversionType(type, conversion, ModContent.TileType<TPure>(), ModContent.TileType<TCorr>(), ModContent.TileType<TCrim>(), ModContent.TileType<THall>(), out newId);

	/// <summary>
	/// Converts the current <paramref name="type"/> into the appropriate tile ID given the <paramref name="conversion"/>.
	/// </summary>
	/// <param name="type">Current tile ID.</param>
	/// <param name="conversion">The conversion taking place.</param>
	/// <param name="pureType">The ID of the pure tile.</param>
	/// <param name="corrType">The ID of the corrupt tile.</param>
	/// <param name="crimType">The ID of the crimson tile.</param>
	/// <param name="hallType">The ID of the hallowed tile.</param>
	/// <param name="newType">The resulting type. This type will never be equal to <paramref name="type"/>; it'll either be one of the other types, or -1.</param>
	/// <returns>Whether the conversion was successful (namely, if the conversion didn't convert to the existing type, such as purifying a pure tile).</returns>
	/// <exception cref="Exception"></exception>
	public static bool GetConversionType(int type, ConversionType conversion, int pureType, int corrType, int crimType, int hallType, out int newType)
	{
		int convertType = -1;
		newType = -1;

		if (type == pureType)
		{
			if (conversion == ConversionType.Purify)
				return false;

			convertType = conversion switch
			{
				ConversionType.Corrupt => corrType,
				ConversionType.Crimson => crimType,
				ConversionType.Hallow => hallType,
				_ => type,
			};
		}
		else if (conversion == ConversionType.Purify)
			convertType = pureType;
		else if (type == corrType)
		{
			if (conversion == ConversionType.Corrupt)
				return false;

			convertType = conversion switch
			{
				ConversionType.Crimson => crimType,
				ConversionType.Hallow => hallType,
				_ => type,
			};
		}
		else if (type == crimType)
		{
			if (conversion == ConversionType.Crimson)
				return false;

			convertType = conversion switch
			{
				ConversionType.Corrupt => corrType,
				ConversionType.Hallow => hallType,
				_ => type,
			};
		}
		else if (type == hallType)
		{
			if (conversion == ConversionType.Hallow)
				return false;

			convertType = conversion switch
			{
				ConversionType.Crimson => crimType,
				ConversionType.Hallow => hallType,
				_ => type,
			};
		}

		newType = convertType;

		if (newType == -1)
			throw new Exception("How did this happen? Invalid tile conversion ID.");

		return true;
	}
}
