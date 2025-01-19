using Terraria.DataStructures;

namespace SpiritReforged.Common.TileCommon.Corruption;

internal class TileCorruptionGlobalTile : GlobalTile
{
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
