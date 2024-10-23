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

	private static int Steps = 0;
	private static bool HasSavanna = false;

	public override void AddTasks(List<GenPass> tasks, List<EcotoneSurfaceMapping.EcotoneEntry> entries)
	{
		int index = tasks.FindIndex(x => x.Name == "Pyramids");
		int secondIndex = tasks.FindIndex(x => x.Name == "Piles") + 2;

		if (index == -1 || secondIndex == -1)
			return;

		tasks.Insert(index, new PassLegacy("Savanna", BaseGeneration(entries)));
		tasks.Insert(secondIndex, new PassLegacy("Populate Savanna", PopulateSavanna));
	}

	private void PopulateSavanna(GenerationProgress progress, GameConfiguration configuration)
	{
		if (!HasSavanna)
			return;

		progress.Message = Language.GetTextValue("Mods.SpiritReforged.Generation.SavannaObjects");
		for (int i = SavannaArea.Left; i < SavannaArea.Right; ++i)
		{
			for (int j = SavannaArea.Top - 2; j < SavannaArea.Bottom; ++j)
			{
				OpenFlags flags = OpenTools.GetOpenings(i, j);
				var tile = Main.tile[i, j];

				if (tile.TileType == TileID.SmallPiles)
				{
					WorldGen.KillTile(i, j);
					WorldGen.PlaceTile(i, j, ModContent.TileType<SavannaRockSmall>(), true, style: WorldGen.genRand.Next(3));
				}

				if (tile.TileType == TileID.LargePiles)
				{
					WorldGen.KillTile(i, j);
					WorldGen.PlaceTile(i + 1, j + 1, ModContent.TileType<SavannaRockLarge>(), true, style: WorldGen.genRand.Next(3));
				}

				if (flags == OpenFlags.None)
					continue;

				if (tile.TileType == ModContent.TileType<SavannaDirt>())
				{
					tile.TileType = (ushort)ModContent.TileType<SavannaGrass>();
					GrowStuffOnGrass(i, j);
				}
			}
		}
	}

	private static void GrowStuffOnGrass(int i, int j)
	{
		if (WorldGen.genRand.NextBool(20))
			CreateTallgrassPatch(WorldGen.genRand.Next(3, 10));

		if (WorldGen.genRand.NextBool(10))
		{
			int type = WorldGen.genRand.NextFromList(ModContent.TileType<TermiteMoundSmall>(), ModContent.TileType<TermiteMoundMedium>(), ModContent.TileType<TermiteMoundLarge>());
			int style = WorldGen.genRand.Next(TileObjectData.GetTileData(type, 0).RandomStyleRange);

			WorldGen.PlaceTile(i, j - 1, type, true, style: style);
		}

		if (WorldGen.genRand.NextBool(12))
			WorldGen.PlaceTile(i, j - 1, ModContent.TileType<SavannaShrubs>(), true, style: WorldGen.genRand.Next(11));

		if (WorldGen.genRand.NextBool(3))
			WorldGen.PlaceTile(i, j - 1, ModContent.TileType<SavannaFoliage>(), true, style: WorldGen.genRand.Next(12));

		void CreateTallgrassPatch(int size)
		{
			for (int v = 0; v < 2; v++)
				for (int x = i - size / 2; x < i + size / 2; x++)
				{
					if (v == 0) //Place short grass
						WorldGen.PlaceTile(x, j - 1, ModContent.TileType<ElephantGrassShort>(), true, style: WorldGen.genRand.Next(3));
					else if (ElephantGrass.IsElephantGrass(x - 1, j - 1) && ElephantGrass.IsElephantGrass(x + 1, j - 1)) //Grow short grass
						WorldGen.PlaceTile(x, j - 1, ModContent.TileType<ElephantGrass>(), true, style: WorldGen.genRand.Next(5));
				}
		}
	}

	private static WorldGenLegacyMethod BaseGeneration(List<EcotoneSurfaceMapping.EcotoneEntry> entries) => (progress, _) =>
	{
		//Don't generate next to the ocean
		static bool NotOcean(EcotoneSurfaceMapping.EcotoneEntry e) => e.Start.X > GenVars.leftBeachEnd 
			&& e.End.X > GenVars.leftBeachEnd && e.Start.X < GenVars.rightBeachStart && e.End.X < GenVars.rightBeachStart;

		IEnumerable<EcotoneSurfaceMapping.EcotoneEntry> validEntries 
			= entries.Where(x => x.SurroundedBy("Desert", "Jungle") && Math.Abs(x.Start.Y - x.End.Y) < 120 && NotOcean(x));

		if (!validEntries.Any())
			return;

		var entry = validEntries.ElementAt(WorldGen.genRand.Next(validEntries.Count()));

		if (entry is null)
			return;

		HasSavanna = true;
		progress.Message = Language.GetTextValue("Mods.SpiritReforged.Generation.SavannaTerrain");

		int startX = entry.Start.X - 0;
		int endX = entry.End.X + 0;
		short startY = EcotoneSurfaceMapping.TotalSurfaceY[(short)entry.Start.X];
		short endY = EcotoneSurfaceMapping.TotalSurfaceY[(short)entry.End.X];
		int[] validIds = [TileID.Dirt, TileID.Grass, TileID.ClayBlock, TileID.CrimsonGrass, TileID.CorruptGrass, TileID.Stone];

		var topBottomY = new Point(Math.Min(startY, endY), Math.Max(startY, endY));

		Steps = WorldGen.genRand.Next(2, 5);

		var sandNoise = new FastNoiseLite(WorldGen.genRand.Next());
		sandNoise.SetFrequency(0.04f);
		int xOffsetForFactor = -1;
		float curve = 0;

		for (int x = startX; x < endX; ++x)
		{
			float factor = GetBaseLerpFactorForX(startX, endX, xOffsetForFactor, x); //Step height

			int addY = (int)MathHelper.Lerp(startY, endY, curve);
			int y = addY - (int)(sandNoise.GetNoise(x, 600) * 2);
			int depth = WorldGen.genRand.Next(20);
			int minDepth = (int)Main.worldSurface - y;

			if (curve < factor) //easing (hills)
			{
				int fullHeight = (startY - endY) / Steps; //The average height of each step
				const float steepness = .05f;

				//Control hill shape using a lazy sine that remains similar between steps
				float amount = (float)Math.Sin(1f + y % fullHeight / (float)fullHeight * 2) * steepness;
				curve += Math.Max(amount, .008f);
			}

			bool hitSolid = false;
			float taper = Math.Clamp((float)Math.Sin((float)(x - startX) / (endX - startX) * Math.PI) * 1.75f, 0, 1);
			for (int i = -80; i < (30 + depth + minDepth) * taper; ++i)
			{
				int realY = y + i;
				var tile = Main.tile[x, realY];

				if (i >= 0)
				{
					if (i >= 15 && tile.HasTile)
						hitSolid = true;

					if (i >= 0 && !hitSolid)
						tile.HasTile = true;

					if (tile.HasTile && !validIds.Contains(tile.TileType) && !TileID.Sets.Ore[tile.TileType])
						continue;

					if (realY < topBottomY.X)
						topBottomY.X = realY;

					if (realY > topBottomY.Y)
						topBottomY.Y = realY;

					float noise = (sandNoise.GetNoise(x, 0) + 1) * 5 + 6;
					int type = i <= noise ? ModContent.TileType<SavannaDirt>() : GetSandType(x, realY);

					if (i > 90 + depth - noise)
						type = TileID.Sandstone;

					if (tile.TileType == TileID.Stone || TileID.Sets.Ore[tile.TileType])
						type = GetHardConversion(type, y);

					tile.TileType = (ushort)type;

					if (i > 1)
					{
						if (tile.WallType == WallID.None)
							tile.WallType = WallID.DirtUnsafe; //Place dirt walls (don't replace existing walls)
					}
					else
						tile.Clear(TileDataType.Wall); //Clear walls above the Savanna surface
				}
				else
					tile.Clear(TileDataType.All);
			}

			xOffsetForFactor += (int)Math.Round(Math.Max(sandNoise.GetNoise(x, 0), 0) * 5);
		}

		SavannaArea = new Rectangle(startX, topBottomY.X, endX - startX, topBottomY.Y - topBottomY.X);
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

		static ushort GetHardConversion(int tileType, int y)
		{
			if (y < 6 && TileID.Sets.Ore[tileType] || tileType == TileID.Stone)
				return (ushort)ModContent.TileType<SavannaDirt>(); //Convert ores and stone close to the surface

			return (ushort)((tileType == TileID.Stone) ? TileID.ClayBlock : tileType);
		}
	};

	private static float GetBaseLerpFactorForX(int startX, int endX, int xOffsetForFactor, int x)
	{
		float factor = (MathF.Min(x + xOffsetForFactor, endX) - startX) / (endX - startX);
		factor = ModifyLerpFactor(factor);
		return factor;
	}

	private static float ModifyLerpFactor(float factor)
	{
		float adj = Steps;
		factor = (int)((factor + 0.1f) * adj) / adj;
		return factor;
	}
}
