using System.Linq;
using Terraria.DataStructures;

namespace SpiritReforged.Common.WorldGeneration;

internal class QuickConversion
{
	public enum BiomeType
	{
		Purity,
		Jungle,
		Ice
	}

	public static BiomeType FindConversionBiome(Point16 position, Point16 size)
	{
		Dictionary<BiomeType, int> biomeCounts = new() { { BiomeType.Purity, 0 }, { BiomeType.Jungle, 0 }, { BiomeType.Ice, 0 } };

		for (int i = position.X; i < position.X + size.X; i++)
		{
			for (int j = position.Y; j < position.Y + size.Y; j++)
			{
				Tile tile = Main.tile[i, j];

				if (!tile.HasTile)
					continue;

				if (tile.TileType is TileID.Mud or TileID.JungleGrass or TileID.JungleVines or TileID.JunglePlants)
					biomeCounts[BiomeType.Jungle]++;
				else if (tile.TileType is TileID.Dirt or TileID.Stone or TileID.ClayBlock)
					biomeCounts[BiomeType.Purity]++;
				else if (tile.TileType is TileID.SnowBlock or TileID.IceBlock)
					biomeCounts[BiomeType.Ice]++;
			}
		}

		BiomeType biome = biomeCounts.MaxBy(x => x.Value).Key;
		return biome;
	}

	public static void SimpleConvert(List<TileCondition> conditions, BiomeType convertTo)
	{
		foreach (var condition in conditions)
		{
			if (condition.HasChanged())
			{
				Tile tile = Main.tile[condition.Position];
				int turnId = -1;

				if (TileID.Sets.Stone[tile.TileType] || tile.BlockType == TileID.Dirt && convertTo != BiomeType.Purity)
				{
					int conv = convertTo switch 
					{ 
						BiomeType.Purity => TileID.Stone,
						BiomeType.Jungle => TileID.Mud,
						BiomeType.Ice => TileID.IceBlock,
						_ => -1
					};

					if (conv != -1)
						turnId = conv;
				}
				else if (TileID.Sets.Grass[tile.TileType])
				{
					int conv = convertTo switch
					{
						BiomeType.Purity => TileID.Grass,
						BiomeType.Jungle => TileID.JungleGrass,
						BiomeType.Ice => TileID.IceBlock,
						_ => -1
					};

					if (conv != -1)
						turnId = conv;
				}

				if (turnId != -1)
					tile.TileType = (ushort)turnId;
			}
		}
	}
}
