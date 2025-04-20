namespace SpiritReforged.Common.WorldGeneration.Micropasses.Passes.CaveEntrances;

internal class ArchwayEntrance : CaveEntrance
{
	public override CaveEntranceType Type => CaveEntranceType.Archways;

	public override void Generate(int x, int y)
	{
		for (int i = x - 20; i < x + 20; ++i)
		{
			for (int j = y - 20; j < y + 20; ++j)
			{
				Tile tile = Main.tile[i, j];
				tile.HasTile = true;
				tile.BlockType = BlockType.Solid;
				tile.TileType = TileID.Dirt;
			}
		}
	}

	public override bool ModifyOpening(ref int x, ref int y, bool isCavinator) => isCavinator;
}
