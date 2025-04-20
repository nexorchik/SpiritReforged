using Terraria.DataStructures;

namespace SpiritReforged.Common.WorldGeneration.Micropasses.Passes.CaveEntrances;

internal class KarstEntrance : CaveEntrance
{
	public override CaveEntranceType Type => CaveEntranceType.Karst;

	public static readonly Dictionary<Point16, int> _direction = []; 

	public override void Generate(int x, int y)
	{
		y -= 10;

		int leftEdge = x - WorldGen.genRand.Next(16, 25);
		int rightEdge = x + WorldGen.genRand.Next(16, 25);
		int depth = y + WorldGen.genRand.Next(38, 55);

		for (int j = y; j < depth; ++j)
		{
			for (int i = leftEdge; i < rightEdge; ++i)
			{
				Tile tile = Main.tile[i, j];
				tile.HasTile = false;

				if (Vector2.Distance(new(i, j), new(x, y)) < 5)
				{
					tile.HasTile = true;
					tile.TileType = TileID.Meteorite;
				}
			}
		}
	}

	public override bool ModifyOpening(ref int x, ref int y, bool isCavinator)
	{

		return true;
	}
}
