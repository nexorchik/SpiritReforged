using SpiritReforged.Common.WorldGeneration;
using SpiritReforged.Common.WorldGeneration.Ecotones;
using SpiritReforged.Content.Savanna.Tiles;
using System.Linq;
using Terraria.DataStructures;
using Terraria.GameContent.Generation;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace SpiritReforged.Content.Savanna;

internal class SavannaEcotone : EcotoneBase
{
	private static Rectangle SavannaArea = Rectangle.Empty;

	public override void AddTasks(List<GenPass> tasks, List<EcotoneSurfaceMapping.EcotoneEntry> entries)
	{
		int index = tasks.FindIndex(x => x.Name == "Pyramids");
		int secondIndex = tasks.FindIndex(x => x.Name == "Shinies");

		if (index == -1 || secondIndex == -1)
			return;

		tasks.Insert(index, new PassLegacy("Savanna Base", BaseGeneration(entries)));
		tasks.Insert(index + 1, new PassLegacy("Grow Savanna", PopulateSavanna));
		//tasks.Insert(secondIndex, new PassLegacy("Savanna Cleanup", BaseGeneration(entries)));
	}

	private void PopulateSavanna(GenerationProgress progress, GameConfiguration configuration)
	{
		Dictionary<Point, OpenFlags> tiles = [];

		for (int i = SavannaArea.Left; i < SavannaArea.Right; ++i)
		{
			for (int j = SavannaArea.Top; j < SavannaArea.Bottom; ++j)
			{
				OpenFlags flags = OpenTools.GetOpenings(i, j);

				if (flags != OpenFlags.None)
					tiles.Add(new Point(i, j), flags);
			}
		}

		foreach ((Point position, OpenFlags flags) in tiles)
		{
			Tile tile = Main.tile[position];

			if (tile.TileType == ModContent.TileType<SavannaDirt>())
				tile.TileType = (ushort)ModContent.TileType<SavannaGrass>();

			tile.WallType = WallID.None;
		}
	}

	private static WorldGenLegacyMethod BaseGeneration(List<EcotoneSurfaceMapping.EcotoneEntry> entries) => (progress, _) =>
	{
		var entry = entries.Find(x => x.SurroundedBy("Desert", "Jungle"));

		if (entry is null)
			return;

		int startX = entry.Start.X - 0;
		int endX = entry.End.X + 0;
		short startY = EcotoneSurfaceMapping.TotalSurfaceY[(short)entry.Start.X];
		short endY = EcotoneSurfaceMapping.TotalSurfaceY[(short)entry.End.X];
		int[] validIds = [TileID.Dirt, TileID.Grass, TileID.ClayBlock, TileID.CrimsonGrass, TileID.CorruptGrass, TileID.Stone];

		SavannaArea = new Rectangle(startX, startY, endX - startX, Math.Max(endY - startY, startY - endY));

		var sandNoise = new FastNoiseLite(WorldGen.genRand.Next());
		sandNoise.SetFrequency(0.06f);

		for (int x = startX; x < endX; ++x)
		{
			float factor = ((float)x - startX) / (endX - startX);
			factor = ModifyLerpFactor(factor);
			int y = (int)MathHelper.Lerp(startY, endY, factor);
			int depth = WorldGen.genRand.Next(20);

			for (int i = -80; i < 90 + depth; ++i)
			{
				Tile tile = Main.tile[x, y + i];

				if (i >= 0)
				{
					if (!tile.HasTile || !validIds.Contains(tile.TileType))
						continue;

					float noise = (sandNoise.GetNoise(x, 0) + 1) * 5;
					int type = i <= noise ? ModContent.TileType<SavannaDirt>() : TileID.Sand;

					if (i > 90 + depth - noise)
						type = TileID.Sandstone;

					if (tile.TileType == TileID.Stone)
						type = TileID.ClayBlock;

					tile.TileType = (ushort)type;
				}
				else
				{
					tile.Clear(TileDataType.All);
				}
			}
		}

		return;		
	};

	private static float ModifyLerpFactor(float factor)
	{
		float adj = 8f;
		factor = (int)((factor + 0.1f) * adj) / adj;
		return factor;
	}
}
