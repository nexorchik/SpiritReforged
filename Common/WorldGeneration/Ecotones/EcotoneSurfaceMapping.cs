using rail;
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
		public EcotoneEdgeDefinition Left;
		public EcotoneEdgeDefinition Right;

		public bool TileFits(int i, int j) => Definition.ValidIds.Contains(Main.tile[i, j].TileType);
		public bool SurroundedBy(string one, string two) => Left.Name == one && Right.Name == two || Left.Name == two && Right.Name == one;

		public override string ToString() => $"{Start} to {End}; of {Definition}:{SurfacePoints.Count}";
	}

	private List<EcotoneEntry> Entries = [];
	private HashSet<Point> TotalSurfacePoints = [];

	public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
	{
		int mapIndex = tasks.FindIndex(x => x.Name == "Shimmer");

		if (mapIndex == -1)
			return;

		foreach (var ecotone in EcotoneBase.Ecotones)
			ecotone.AddTasks(tasks, Entries, TotalSurfacePoints);

		tasks.Insert(mapIndex + 1, new PassLegacy("Map Ecotones", MapEcotones));
	}

	private void MapEcotones(GenerationProgress progress, GameConfiguration configuration)
	{
		const int StartX = 250;

		progress.Message = "Mapping ecotones";

		Entries.Clear();
		TotalSurfacePoints.Clear();

		int y = 0;

		while (Main.tile[StartX, ++y].TileType != TileID.Sand)
		{
		}

		int transitionCount = 0;
		EcotoneEntry entry = new EcotoneEntry(new Point(StartX, y), EcotoneEdgeDefinitions.GetEcotone("Ocean"));
		entry.Left = EcotoneEdgeDefinitions.GetEcotone("Ocean");

		for (int x = StartX; x < Main.maxTilesX - StartX; ++x)
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

			if (transitionCount > 20 && EcotoneEdgeDefinitions.TryGetEcotoneByTile(Main.tile[x, y].TileType, out var def) && def.Name != entry.Definition.Name)
			{ 
				EcotoneEdgeDefinition old = entry.Definition;
				entry.End = new Point(x, y);
				entry.Right = def;
				Entries.Add(entry);

				if (entry.SurroundedBy("Desert", "Jungle"))
				{
					int oie = 0;
				}
				
				entry = new EcotoneEntry(new Point(x, y), def);
				entry.Left = old;
				transitionCount = 0;
			}

			//WorldGen.PlaceTile(x, y - 3, entry.Definition.DisplayId, true, true);
			entry.SurfacePoints.Add(new Point(x, y));
			TotalSurfacePoints.Add(new Point(x, y));
		}

		entry.End = new Point(Main.maxTilesX - StartX, y);
		entry.Right = EcotoneEdgeDefinitions.GetEcotone("Ocean");
		Entries.Add(entry);
		Entries = new(Entries.OrderBy(x => x.Start.X));

		//foreach (var item in Entries)
		//{
		//	for (int x = item.Start.X; x < item.End.X; ++x)
		//	{
		//		for (int nY = 40; nY < 80; ++nY)
		//			WorldGen.PlaceTile(x, nY, item.Definition.DisplayId, true, true);
		//	}
		//}

		//foreach (var item in TotalSurfacePoints)
		//{
		//	WorldGen.KillTile(item.X, item.Y);
		//	WorldGen.PlaceTile(item.X, item.Y, TileID.GreenCandyCaneBlock, true, true);

		//	WorldGen.KillTile(item.X, item.Y + 1);
		//	WorldGen.PlaceTile(item.X, item.Y + 1, TileID.GreenCandyCaneBlock, true, true);
		//}
	}

	private static bool SolidTileOrWall(int x, int y) => WorldGen.SolidOrSlopedTile(x, y) || Main.tile[x, y].WallType != WallID.None;
}
