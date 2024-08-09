using System.Linq;
using Terraria.GameContent.Generation;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace SpiritReforged.Common.WorldGeneration.Ecotones;

internal class EcotoneSurfaceMapping : ModSystem
{
	public class EcotoneEntry(Point start, EcotoneEdgeDefinition definition)
	{
		public Point Start = start;
		public Point End;
		public HashSet<Point> SurfacePoints = [];
		public EcotoneEdgeDefinition Definition = definition;

		public bool TileFits(int i, int j) => Definition.ValidIds.Contains(Main.tile[i, j].TileType);

		public override string ToString() => $"{Start} to {End}; of {Definition}";
	}

	public List<EcotoneEntry> Entries = [];

	public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
	{
		int index = tasks.FindIndex(x => x.Name == "Tile Cleanup");

		if (index != -1)
			tasks.Insert(index + 1, new PassLegacy("Map Ecotones", MapEcotones));
	}

	private void MapEcotones(GenerationProgress progress, GameConfiguration configuration)
	{
		progress.Message = "Mapping ecotones";

		int y = 0;

		while (Main.tile[150, ++y].TileType != TileID.Sand)
		{
		}

		int transitionCount = 0;
		EcotoneEntry entry = new EcotoneEntry(new Point(150, y), EcotoneEdgeDefinitions.GetEcotone("Desert"));

		for (int x = 150; x < Main.maxTilesX - 150; ++x)
		{
			int dir = WorldGen.SolidOrSlopedTile(x, y) ? -1 : 1;

			if (dir == -1)
			{
				while (SolidTileOrWall(x, y))
					y -= 1;

				y -= dir;
			}
			else
			{
				while (!SolidTileOrWall(x, y))
					y += 1;
			}

			if (!entry.TileFits(x, y))
				transitionCount++;

			if (transitionCount > 20 && EcotoneEdgeDefinitions.TryGetEcotoneByTile(Main.tile[x, y].TileType, out var newDefinition))
			{
				entry.End = new Point(x, y);
				Entries.Add(entry);
				entry = new EcotoneEntry(new Point(x, y), newDefinition);
				transitionCount = 0;
			}

			WorldGen.PlaceTile(x, y - 2, entry.Definition.DisplayId, true, true);
			WorldGen.PlaceTile(x, y - 3, entry.Definition.DisplayId, true, true);
			entry.SurfacePoints.Add(new Point(x, y));
		}

		foreach (var item in Entries)
		{
			for (int x = item.Start.X; x < item.End.X; ++x)
			{
				for (int nY = 40; nY < 80; ++nY)
					WorldGen.PlaceTile(x, nY, item.Definition.DisplayId, true, true);
			}
		}
	}

	private static bool SolidTileOrWall(int x, int y) => WorldGen.SolidOrSlopedTile(x, y) || Main.tile[x, y].WallType != WallID.None;
}
