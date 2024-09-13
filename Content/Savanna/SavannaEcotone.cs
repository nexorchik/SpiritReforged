using SpiritReforged.Common.WorldGeneration;
using SpiritReforged.Common.WorldGeneration.Ecotones;
using SpiritReforged.Content.Savanna.Tiles;
using System.Diagnostics;
using System.Linq;
using Terraria.DataStructures;
using Terraria.GameContent.Generation;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace SpiritReforged.Content.Savanna;

internal class SavannaEcotone : EcotoneBase
{
	private static Rectangle SavannaArea = Rectangle.Empty;

	private static int Steps = 0;

	protected override void InternalLoad() => On_WorldGen.SpreadGrass += HijackSpreadGrass;

	private void HijackSpreadGrass(On_WorldGen.orig_SpreadGrass orig, int i, int j, int dirt, int grass, bool repeat, TileColorCache color)
	{
		ushort tileType = Main.tile[i, j].TileType;

		if (tileType == ModContent.TileType<SavannaGrass>() || tileType == ModContent.TileType<SavannaDirt>())
			return;

		orig(i, j, dirt, grass, repeat, color);
	}

	public override void AddTasks(List<GenPass> tasks, List<EcotoneSurfaceMapping.EcotoneEntry> entries)
	{
		int index = tasks.FindIndex(x => x.Name == "Pyramids");
		int secondIndex = tasks.FindIndex(x => x.Name == "Shinies");

		if (index == -1 || secondIndex == -1)
			return;

		tasks.Insert(index, new PassLegacy("Savanna Base", BaseGeneration(entries)));
		tasks.Insert(index + 1, new PassLegacy("Grow Savanna", PopulateSavanna));
	}

	private void PopulateSavanna(GenerationProgress progress, GameConfiguration configuration)
	{
		Dictionary<Point16, OpenFlags> tiles = [];
		Dictionary<Point16, int> grassLocations = [];

		for (int i = SavannaArea.Left; i < SavannaArea.Right; ++i)
		{
			for (int j = SavannaArea.Top; j < SavannaArea.Bottom; ++j)
			{
				OpenFlags flags = OpenTools.GetOpenings(i, j);

				if (flags != OpenFlags.None)
				{
					tiles.Add(new Point16(i, j), flags);

					if (Main.tile[i, j].TileType == ModContent.TileType<SavannaDirt>() && WorldGen.genRand.NextBool(25))
						grassLocations.Add(new Point16(i, j), WorldGen.genRand.Next(8, 16));
				}
			}
		}

		foreach ((Point16 position, OpenFlags flags) in tiles)
		{
			Tile tile = Main.tile[position];

			if (tile.TileType == ModContent.TileType<SavannaDirt>())
			{
				tile.TileType = (ushort)ModContent.TileType<SavannaGrass>();

				var nearestGrassLocation = grassLocations.MinBy(x => x.Key.ToVector2().DistanceSQ(position.ToVector2()));
				GrowStuffOnGrass(position, (nearestGrassLocation.Key, nearestGrassLocation.Value));
				tile.WallType = WallID.None;
			}
		}
	}

	private static void GrowStuffOnGrass(Point16 position, (Point16 location, int size) nearestGrass)
	{
		float grassLoc = position.ToVector2().DistanceSQ(nearestGrass.location.ToVector2());

		if (grassLoc < nearestGrass.size * nearestGrass.size)
			WorldGen.PlaceTile(position.X, position.Y - 1, ModContent.TileType<ElephantGrass>(), true);
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

		Steps = WorldGen.genRand.Next(7, 11);
		SavannaArea = new Rectangle(startX, startY, endX - startX, Math.Max(endY - startY, startY - endY) + 40);

		Dictionary<int, int> stepOffset = [];
		int offset = 0;

		for (int i = 0; i < Steps; ++i)
		{
			offset = WorldGen.genRand.Next(6, 18);

			if (i == Steps - 1)
				offset = 0;

			stepOffset.Add(i, offset);
		}

		var sandNoise = new FastNoiseLite(WorldGen.genRand.Next());
		sandNoise.SetFrequency(0.06f);
		int xOffsetForFactor = 0;

		for (int x = startX; x < endX; ++x)
		{
			float factor = (MathF.Min(x + xOffsetForFactor, endX) - startX) / (endX - startX);
			factor = ModifyLerpFactor(factor);
			int addY = (int)MathHelper.Lerp(startY, endY, factor);
			int y = addY + stepOffset[(int)(factor * (Steps - 1))] - (int)(sandNoise.GetNoise(x, 600) * 2);
			int depth = WorldGen.genRand.Next(20);

			for (int i = -80; i < 90 + depth; ++i)
			{
				Tile tile = Main.tile[x, y + i];

				if (i >= 0)
				{
					if (i >= 0 && i < 15)
						tile.HasTile = true;

					if (tile.HasTile && !validIds.Contains(tile.TileType))
						continue;

					float noise = (sandNoise.GetNoise(x, 0) + 1) * 5 + 6;
					int type = i <= noise ? ModContent.TileType<SavannaDirt>() : GetSandType(x, y + i);

					if (i > 90 + depth - noise)
						type = TileID.Sandstone;

					if (tile.TileType == TileID.Stone)
						type = TileID.ClayBlock;

					tile.TileType = (ushort)type;
					tile.WallType = WallID.DirtUnsafe;
				}
				else
				{
					tile.Clear(TileDataType.All);
				}
			}

			xOffsetForFactor += (int)Math.Round(sandNoise.GetNoise(x, 0) * 2);
		}

		return;

		static ushort GetSandType(int x, int y)
		{
			int off = 0;

			while (WorldGen.SolidOrSlopedTile(x, y))
			{
				y++;
				off++;
			}

			return off < 3 ? TileID.Sandstone : TileID.Sand;
		}
	};

	private static float ModifyLerpFactor(float factor)
	{
		float adj = Steps;
		factor = (int)((factor + 0.1f) * adj) / adj;
		return factor;
	}
}
