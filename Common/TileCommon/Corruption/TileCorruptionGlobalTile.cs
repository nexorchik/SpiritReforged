using Terraria.DataStructures;

namespace SpiritReforged.Common.TileCommon.Corruption;

internal class TileCorruptionGlobalTile : GlobalTile
{
	/// <summary>
	/// <inheritdoc/><para/>
	/// Automatically converts tiles based on lower types. Useful for things like foliage.
	/// </summary>
	public override bool TileFrame(int i, int j, int type, ref bool resetFrame, ref bool noBreak)
	{
		int x = i;
		int y = j;

		TileExtensions.GetTopLeft(ref x, ref y);
		var data = TileObjectData.GetTileData(Main.tile[x, y]);

		if (data != null)
		{
			y += data.Height;
			var baseTile = Main.tile[x, y];

			var conversionType = ConversionType.Purify;

			if (TileID.Sets.Corrupt[baseTile.TileType])
				conversionType = ConversionType.Corrupt;
			else if (TileID.Sets.Crimson[baseTile.TileType])
				conversionType = ConversionType.Crimson;
			else if (TileID.Sets.Hallow[baseTile.TileType])
				conversionType = ConversionType.Hallow;

			TileCorruptor.Convert(new EntitySource_TileUpdate(i, j), conversionType, i, j);
		}

		return true;
	}

	/// <summary>
	/// <inheritdoc/><para/>
	/// Handles corruption spread for custom types.
	/// </summary>
	public override void RandomUpdate(int i, int j, int type)
	{
		if (type < TileID.Count || !WorldGen.AllowedToSpreadInfections)
			return;

		for (int k = 0; k < 2; ++k)
			CorruptNearby(i, j, type);
	}

	private static void CorruptNearby(int i, int j, int type)
	{
		Point16 pos = GetPos(i, j);
		Tile tile = Main.tile[pos];

		if (!WorldGen.SolidTile(tile))
			return;

		if (TileID.Sets.Corrupt[type])
			TileCorruptor.Convert(new EntitySource_TileUpdate(i, j), ConversionType.Corrupt, pos.X, pos.Y);

		if (TileID.Sets.Hallow[type])
			TileCorruptor.Convert(new EntitySource_TileUpdate(i, j), ConversionType.Hallow, pos.X, pos.Y);

		if (TileID.Sets.Crimson[type])
			TileCorruptor.Convert(new EntitySource_TileUpdate(i, j), ConversionType.Crimson, pos.X, pos.Y);
	}

	private static Point16 GetPos(int i, int j) => Main.rand.Next(8) switch
	{
		0 => new Point16(i - 1, j),
		1 => new Point16(i + 1, j),
		2 => new Point16(i, j + 1),
		3 => new Point16(i, j - 1),
		4 => new Point16(i - 1, j + 1),
		5 => new Point16(i - 1, j - 1),
		6 => new Point16(i + 1, j + 1),
		_ => new Point16(i + 1, j - 1),
	};
}
