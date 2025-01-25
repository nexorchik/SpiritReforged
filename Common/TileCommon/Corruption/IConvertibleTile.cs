using Terraria.DataStructures;

namespace SpiritReforged.Common.TileCommon.Corruption;

public enum ConversionType : byte
{
	Purify,
	Corrupt,
	Crimson,
	Hallow,
}

internal interface IConvertibleTile
{
	/// <summary>
	/// Runs conversion code automatically at the given location of the given type.
	/// </summary>
	/// <param name="source">The source of the conversion.</param>
	/// <param name="type">The type of conversion being run.</param>
	/// <param name="i">X position of the tile.</param>
	/// <param name="j">Y position of the tile.</param>
	/// <returns>Whether to automatically run syncing and framing code. Only runs for a 1x1 tile at the current position.</returns>
	public bool Convert(IEntitySource source, ConversionType type, int i, int j);
}
