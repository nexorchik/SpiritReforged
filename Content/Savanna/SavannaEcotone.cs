using SpiritReforged.Common.WorldGeneration.Ecotones;
using SpiritReforged.Content.Savanna.Tiles;
using System.Linq;
using Terraria.GameContent.Generation;
using Terraria.WorldBuilding;

namespace SpiritReforged.Content.Savanna;

internal class SavannaEcotone : EcotoneBase
{
	public override void AddTasks(List<GenPass> tasks, List<EcotoneSurfaceMapping.EcotoneEntry> entries, HashSet<Point> totalSurfacePoints)
	{
		int index = tasks.FindIndex(x => x.Name == "Pyramids");
		int secondIndex = tasks.FindIndex(x => x.Name == "Dye Plants");

		if (index == -1 || secondIndex == -1)
			return;

		tasks.Insert(index, new PassLegacy("Savanna Base", BaseGeneration(entries, totalSurfacePoints)));
		tasks.Insert(secondIndex, new PassLegacy("Savanna Cleanup", BaseGeneration(entries, totalSurfacePoints)));
	}

	private static WorldGenLegacyMethod BaseGeneration(List<EcotoneSurfaceMapping.EcotoneEntry> entries, HashSet<Point> totalSurfacePoints) => (progress, _) =>
	{
		var entry = entries.Find(x => x.SurroundedBy("Desert", "Jungle"));

		if (entry is null)
			return;

		int startX = entry.Start.X - 70;
		int endX = entry.End.X + 70;

		for (int x = startX; x < endX; ++x)
		{
			float factor = ((float)endX - x) / endX;
			int y = (int)MathHelper.Lerp(entry.Start.Y, entry.End.Y, factor);
			int depth = WorldGen.genRand.Next(20);

			for (int i = -80; i < 60 + depth; ++i)
			{
				Tile tile = Main.tile[x, y + i];

				if (!tile.HasTile || tile.TileType is TileID.Cloud or TileID.RainCloud)
					continue;

				if (tile.TileType is TileID.LeafBlock or TileID.LivingWood || tile.WallType == WallID.LivingWoodUnsafe)
				{
					if (i < 0)
						tile.Clear(Terraria.DataStructures.TileDataType.All);
					else
					{
						WorldGen.PlaceTile(x, y + i, ModContent.TileType<SavannaDirt>(), true, true);
						tile.WallType = WallID.Dirt;
					}

					continue;
				}

				if (tile.TileType == TileID.Dirt)
					tile.TileType = !WorldGen.SolidTile(x, y + i - 1) ? (ushort)ModContent.TileType<SavannaGrass>() : (ushort)ModContent.TileType<SavannaDirt>();
				else if (tile.TileType == TileID.Grass)
					tile.TileType = (ushort)ModContent.TileType<SavannaGrass>();
				else if (tile.TileType == TileID.Stone)
					tile.TileType = TileID.ClayBlock;

				WorldGen.TileFrame(x, y + i, true);
			}
		}

		return;
		HashSet<Point> points = [.. totalSurfacePoints.Where(x => x.X > entry.Start.X - 70 && x.X < entry.End.X + 70)];

		foreach (Point position in points)
		{
			for (int i = -80; i < 60; ++i)
			{
				if (position.X < entry.Start.X - 60 && WorldGen.genRand.NextBool(20 - Math.Max(entry.Start.X - 60 - position.X, 1) * 2))
					continue;

				Tile tile = Main.tile[position.X, position.Y + i];

				if (!tile.HasTile || tile.TileType is TileID.Cloud or TileID.RainCloud)
					continue;

				if (tile.TileType is TileID.LeafBlock or TileID.LivingWood || tile.WallType == WallID.LivingWoodUnsafe)
				{
					if (i < 0)
						tile.Clear(Terraria.DataStructures.TileDataType.All);
					else
					{
						WorldGen.PlaceTile(position.X, position.Y + i, ModContent.TileType<SavannaDirt>(), true, true);
						tile.WallType = WallID.Dirt;
					}

					continue;
				}

				if (tile.TileType == TileID.Dirt)
					tile.TileType = !WorldGen.SolidTile(position.X, position.Y + i - 1) ? (ushort)ModContent.TileType<SavannaGrass>() : (ushort)ModContent.TileType<SavannaDirt>();
				else if (tile.TileType == TileID.Grass)
					tile.TileType = (ushort)ModContent.TileType<SavannaGrass>();
				else if (tile.TileType == TileID.Stone)
					tile.TileType = TileID.ClayBlock;

				WorldGen.TileFrame(position.X, position.Y + i, true);
			}
		}
	};

	//private static void CleanupGeneration(List<GenPass> tasks, EcotoneSurfaceMapping.EcotoneEntry entry, HashSet<Point> totalSurfacePoints)
	//{
	//	int index = tasks.FindIndex(x => x.Name == "Surface Ore and Stone");

	//	if (index == -1)
	//		return;

	//	tasks.Insert(index, new PassLegacy("Savanna Cleanup", (progress, _) =>
	//	{
	//		if (entry is null)
	//			return;

	//		HashSet<Point> points = [.. totalSurfacePoints.Where(x => x.X > entry.Start.X - 60 && x.X < entry.End.X + 60)];

	//		foreach (Point position in points)
	//		{
	//			for (int i = -80; i < 60; ++i)
	//			{
	//				if (position.X < entry.Start.X - 50 && WorldGen.genRand.NextBool(((entry.Start.X - 50) - position.X) / 10))
	//				{

	//				}

	//				Tile tile = Main.tile[position.X, position.Y + i];

	//				if (!tile.HasTile)
	//					continue;

	//				if (tile.TileType is TileID.Cloud or TileID.RainCloud)
	//					continue;

	//				if (tile.TileType is TileID.LeafBlock or TileID.LivingWood || tile.WallType == WallID.LivingWoodUnsafe)
	//				{
	//					if (i < 0)
	//						tile.Clear(Terraria.DataStructures.TileDataType.All);
	//					else
	//					{
	//						WorldGen.PlaceTile(position.X, position.Y + i, ModContent.TileType<SavannaDirt>(), true, true);
	//						tile.WallType = WallID.Dirt;
	//					}

	//					continue;
	//				}

	//				if (tile.TileType == TileID.Dirt)
	//					tile.TileType = !WorldGen.SolidTile(position.X, position.Y + i - 1) ? (ushort)ModContent.TileType<SavannaGrass>() : (ushort)ModContent.TileType<SavannaDirt>();
	//				else if (tile.TileType == TileID.Grass)
	//					tile.TileType = (ushort)ModContent.TileType<SavannaGrass>();

	//				WorldGen.TileFrame(position.X, position.Y + i, true);
	//			}
	//		}
	//	}));
	//}
}
