using Terraria.DataStructures;
using Terraria.WorldBuilding;

namespace SpiritReforged.Common.WorldGeneration.Micropasses.Passes.CaveEntrances;

internal class KarstEntrance : CaveEntrance
{
	public override CaveEntranceType Type => CaveEntranceType.Karst;

	public static readonly Dictionary<Point16, int> _direction = []; 

	public override void Generate(int x, int y)
	{
		y += 16;
		ShapeData data = new();

		WorldUtils.Gen(new Point(x, y), new Shapes.Circle(20, 8), 
			Actions.Chain(new Modifiers.Blotches(7, 4, 0.2f), new Actions.ClearWall().Output(data), new Modifiers.Blotches(3, 2, 0.6f), 
			new Actions.ClearTile().Output(data)));

		int count = WorldGen.genRand.Next(1, 4);

		for (int i = 1; i < count + 1; ++i)
		{
			var origin = new Point(x + WorldGen.genRand.Next(-5 * i, 5 * i + 1), y + WorldGen.genRand.Next(2, 5 * i));
			bool circle = WorldGen.genRand.NextBool();
			GenShape shape = circle ? new Shapes.Circle(12 + 3 * i, 3 + i) : new Shapes.Rectangle(12 + 3 * i, 4 + i);
			var blotches = new Modifiers.Blotches(circle ? 3 : 7, circle ? 2 : 4, circle ? 0.9f : 0.6f);
			WorldUtils.Gen(origin, shape, Actions.Chain(blotches, new Actions.ClearTile().Output(data)));
		}

		WorldUtils.Gen(new Point(x, y - 26), new Shapes.Rectangle(8, 25), 
			Actions.Chain(new Modifiers.Blotches(7, 4, 0.2f), new Actions.ClearWall(), new Modifiers.Blotches(5, 3, 0.6f), new Actions.ClearTile()));

		foreach (var pos in data.GetData())
		{
			if (pos.Y > 0)
			{
				Tile tile = Main.tile[new Point16(pos.X + x, pos.Y + y)];
				tile.LiquidAmount = 255;
				tile.LiquidType = LiquidID.Water;
			}

			GrowGrass(pos.X + x, pos.Y + y + 1);
			GrowGrass(pos.X + x, pos.Y + y - 1);
			GrowGrass(pos.X + x + 1, pos.Y + y);
			GrowGrass(pos.X + x - 1, pos.Y + y);
		}
	}

	private static void GrowGrass(int x, int y)
	{
		Tile tile = Main.tile[x, y];

		if (!tile.HasTile)
		{
			return;
		}

		if (tile.TileType == TileID.Dirt)
		{
			tile.TileType = TileID.Grass;
		}
		else if (tile.TileType == TileID.Mud)
		{
			tile.TileType = TileID.JungleGrass;
		}
	}

	public override bool ModifyOpening(ref int x, ref int y, bool isCavinator)
	{
		if (isCavinator)
		{
			x += WorldGen.genRand.NextBool() ? WorldGen.genRand.Next(-20, -12) : WorldGen.genRand.Next(13, 21);
		}

		return true;
	}
}
