using System.Linq;
using Terraria.DataStructures;

namespace SpiritReforged.Common.WorldGeneration;

internal class QuickConversion
{
	public enum BiomeType
	{
		Purity,
		Jungle,
		Ice,
		Mushroom,
		Desert
	}

	public static BiomeType FindConversionBiome(Point16 position, Point16 size)
	{
		Dictionary<BiomeType, int> biomeCounts = new() { { BiomeType.Purity, 0 }, { BiomeType.Jungle, 0 }, { BiomeType.Ice, 0 }, { BiomeType.Mushroom, 0 }, 
			{ BiomeType.Desert, 0 } };

		for (int i = position.X; i < position.X + size.X; i++)
		{
			for (int j = position.Y; j < position.Y + size.Y; j++)
			{
				Tile tile = Main.tile[i, j];

				if (!tile.HasTile)
					continue;

				if (tile.TileType is TileID.JungleGrass or TileID.JungleVines or TileID.JunglePlants)
					biomeCounts[BiomeType.Jungle]++;
				else if (tile.TileType is TileID.Dirt or TileID.Stone or TileID.ClayBlock)
					biomeCounts[BiomeType.Purity]++;
				else if (tile.TileType is TileID.SnowBlock or TileID.IceBlock)
					biomeCounts[BiomeType.Ice]++;
				else if (tile.TileType is TileID.Sand or TileID.Sandstone or TileID.HardenedSand or TileID.FossilOre)
					biomeCounts[BiomeType.Desert]++;
				else if (tile.TileType is TileID.MushroomBlock or TileID.MushroomGrass or TileID.MushroomVines)
					biomeCounts[BiomeType.Mushroom] += 3;
			}
		}

		BiomeType biome = biomeCounts.MaxBy(x => x.Value).Key;
		return biome;
	}

	public static void SimpleConvert(List<TileCondition> conditions, BiomeType convertTo, bool growGrassIfApplicable)
	{
		HashSet<Point16> grasses = [];

		int grassType = -1;
		int dirtType = -1;

		if (convertTo == BiomeType.Jungle)
			(grassType, dirtType) = (TileID.JungleGrass, TileID.Mud);
		else if (convertTo == BiomeType.Mushroom)
			(grassType, dirtType) = (TileID.MushroomGrass, TileID.Mud);
		else if (convertTo == BiomeType.Purity)
			(grassType, dirtType) = (TileID.Grass, TileID.Dirt);

		foreach (var condition in conditions)
		{
			if (condition.HasChanged())
			{
				Tile tile = Main.tile[condition.Position];
				int turnId = -1;
				int wallId = -1;

				if (TileID.Sets.Stone[tile.TileType] || tile.TileType == TileID.Dirt && convertTo != BiomeType.Purity)
				{
					int conv = convertTo switch
					{
						BiomeType.Purity => TileID.Stone,
						BiomeType.Jungle or BiomeType.Mushroom => TileID.Mud,
						BiomeType.Ice => TileID.IceBlock,
						BiomeType.Desert => TileID.Sandstone,
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
						BiomeType.Desert => TileID.Sandstone,
						_ => -1
					};

					if (conv != -1)
						turnId = conv;
				}
				else
				{
					if (ConvertWood(condition.Position, convertTo, out int newId, out int newWallId))
					{
						if (newId != -1)
							turnId = newId;

						if (newWallId != -1)
							wallId = newWallId;
					}
				}

				if (turnId != -1)
				{
					tile.TileType = (ushort)turnId;

					if (OpenExtensions.GetOpenings(condition.Position.X, condition.Position.Y, false, false) != OpenFlags.None && growGrassIfApplicable)
						grasses.Add(condition.Position);
				}

				if (wallId != -1)
					tile.WallType = (ushort)wallId;
			}
		}

		foreach (var grass in grasses)
		{
			Tile tile = Main.tile[grass];
			tile.TileType = (ushort)grassType;

			WorldGen.TileFrame(grass.X, grass.Y, true);
		}
	}

	private static bool ConvertWood(Point16 position, BiomeType convertTo, out int newId, out int newWallId)
	{
		Tile tile = Main.tile[position];
		newWallId = -1;
		newId = -1;

		if (tile.WallType == WallID.Wood)
		{
			newWallId = convertTo switch
			{
				BiomeType.Desert => WallID.PalmWood,
				BiomeType.Ice => WallID.BorealWood,
				BiomeType.Jungle => WallID.RichMaogany,
				BiomeType.Mushroom => WallID.MushroomUnsafe,
				BiomeType.Purity or _ => WallID.Wood,
			};
		}

		if (tile.TileType == TileID.WoodBlock)
		{
			newId = convertTo switch
			{
				BiomeType.Desert => TileID.PalmWood,
				BiomeType.Ice => TileID.BorealWood,
				BiomeType.Jungle => TileID.RichMahogany,
				BiomeType.Mushroom => TileID.MushroomBlock,
				BiomeType.Purity or _ => TileID.WoodBlock,
			};
		}

		return newId != -1;
	}
}
