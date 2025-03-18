using SpiritReforged.Common.TileCommon;
using SpiritReforged.Common.TileCommon.Corruption;
using SpiritReforged.Common.TileCommon.PresetTiles;
using Terraria.DataStructures;

namespace SpiritReforged.Content.Savanna.Tiles;

public class SavannaGrass : GrassTile, IConvertibleTile
{
	protected override int DirtType => ModContent.TileType<SavannaDirt>();
	protected virtual Color MapColor => new(104, 156, 70);

	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();

		RegisterItemDrop(Mod.Find<ModItem>("SavannaDirtItem").Type);
		AddMapEntry(MapColor);
		this.Merge(ModContent.TileType<SavannaGrass>(), ModContent.TileType<SavannaGrassCorrupt>(), ModContent.TileType<SavannaGrassHallow>(), ModContent.TileType<SavannaGrassCrimson>());
	}

	public override void RandomUpdate(int i, int j)
	{
		if (SpreadHelper.Spread(i, j, Type, 4, DirtType) && Main.netMode != NetmodeID.SinglePlayer)
			NetMessage.SendTileSquare(-1, i, j, 3, TileChangeType.None); //Try spread grass

		GrowTiles(i, j);
	}

	protected virtual void GrowTiles(int i, int j)
	{
		var above = Framing.GetTileSafely(i, j - 1);

		if (!above.HasTile && above.LiquidAmount < 80)
		{
			int grassChance = GrassAny() ? 6 : 40;

			if (Main.rand.NextBool(grassChance) && WorldGen.PlaceTile(i, j - 1, ModContent.TileType<ElephantGrass>(), true, style: Main.rand.Next(5, 8)))
				NetMessage.SendTileSquare(-1, i, j - 2, 1, 2);

			if (Main.rand.NextBool(15) && WorldGen.PlaceTile(i, j - 1, ModContent.TileType<SavannaFoliage>(), true, style: Main.rand.Next(SavannaFoliage.StyleRange)))
				NetMessage.SendTileSquare(-1, i, j - 1);

			if (Main.rand.NextBool(1400) && WorldGen.PlaceTile(i, j - 1, TileID.DyePlants, true, style: 2))
				NetMessage.SendTileSquare(-1, i, j - 1);

			if (Main.rand.NextBool(100) && !WorldGen.PlayerLOS(i, j))
			{
				if (Main.rand.NextBool() && WorldGen.PlaceObject(i, j, ModContent.TileType<TermiteMoundSmall>(), true, style: Main.rand.Next(3)))
					NetMessage.SendTileSquare(-1, i, j - 1, 2, 1);
				else if (Main.rand.NextBool(3) && WorldGen.PlaceObject(i, j, ModContent.TileType<TermiteMoundMedium>(), true, style: Main.rand.Next(2)))
					NetMessage.SendTileSquare(-1, i, j - 4, 3, 4);
				else if (Main.rand.NextBool(5) && WorldGen.PlaceObject(i, j, ModContent.TileType<TermiteMoundLarge>(), true))
					NetMessage.SendTileSquare(-1, i, j - 5, 3, 5);
			}
		}

		if (Main.rand.NextBool(45) && Main.tile[i, j + 1].LiquidType != LiquidID.Lava)
			TileExtensions.GrowVine(i, j + 1, ModContent.TileType<SavannaVine>());

		bool GrassAny()
		{
			int type = ModContent.TileType<ElephantGrass>();
			return Framing.GetTileSafely(i - 1, j - 1).TileType == type || Framing.GetTileSafely(i + 1, j - 1).TileType == type;
		}
	}

	public override void FloorVisuals(Player player)
	{
		if (player.flowerBoots) //Flower Boots functionality
		{
			var pos = ((player.Bottom - new Vector2(0, 8 * player.gravDir)) / 16).ToPoint16();

			if (!Main.tile[pos.X, pos.Y].HasTile)
			{
				WorldGen.PlaceTile(pos.X, pos.Y, ModContent.TileType<SavannaFoliage>(), true, style: Main.rand.Next(SavannaFoliage.StyleRange));
				NetMessage.SendTileSquare(-1, pos.X, pos.Y);
			}
		}
	}

	public bool Convert(IEntitySource source, ConversionType type, int i, int j)
	{
		var tile = Main.tile[i, j];

		tile.TileType = (ushort)(type switch
		{
			ConversionType.Hallow => ModContent.TileType<SavannaGrassHallow>(),
			ConversionType.Crimson => ModContent.TileType<SavannaGrassCrimson>(),
			ConversionType.Corrupt => ModContent.TileType<SavannaGrassCorrupt>(),
			_ => ModContent.TileType<SavannaGrass>(),
		});

		return true;
	}
}

public class SavannaGrassCorrupt : SavannaGrass
{
	protected override Color MapColor => new(109, 106, 174);

	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();

		TileID.Sets.Corrupt[Type] = true;
		TileID.Sets.AddCorruptionTile(Type, 20);
	}

	protected override void GrowTiles(int i, int j)
	{
		var above = Framing.GetTileSafely(i, j - 1);
		if (!above.HasTile && above.LiquidAmount < 80)
		{
			int grassChance = GrassAny() ? 6 : 50;

			if (Main.rand.NextBool(grassChance) && WorldGen.PlaceTile(i, j - 1, ModContent.TileType<ElephantGrassCorrupt>(), true, style: Main.rand.Next(5, 8)))
				NetMessage.SendTileSquare(-1, i, j - 2, 1, 2, TileChangeType.None);

			if (Main.rand.NextBool(80) && WorldGen.PlaceTile(i, j - 1, ModContent.TileType<SavannaFoliageCorrupt>(), true, style: Main.rand.Next(SavannaFoliage.StyleRange)))
				NetMessage.SendTileSquare(-1, i, j - 1);
		}

		bool GrassAny()
		{
			int type = ModContent.TileType<ElephantGrassCorrupt>();
			return Framing.GetTileSafely(i - 1, j - 1).TileType == type || Framing.GetTileSafely(i + 1, j - 1).TileType == type;
		}
	}
}

public class SavannaGrassCrimson : SavannaGrass
{
	protected override Color MapColor => new(183, 69, 68);

	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();

		TileID.Sets.AddCrimsonTile(Type, 20);
		TileID.Sets.Crimson[Type] = true;
	}

	protected override void GrowTiles(int i, int j)
	{
		var above = Framing.GetTileSafely(i, j - 1);
		if (!above.HasTile && above.LiquidAmount < 80)
		{
			int grassChance = GrassAny() ? 6 : 50;

			if (Main.rand.NextBool(grassChance) && WorldGen.PlaceTile(i, j - 1, ModContent.TileType<ElephantGrassCrimson>(), true, style: Main.rand.Next(5, 8)))
				NetMessage.SendTileSquare(-1, i, j - 2, 1, 2, TileChangeType.None);

			if (Main.rand.NextBool(80) && WorldGen.PlaceTile(i, j - 1, ModContent.TileType<SavannaFoliageCrimson>(), true, style: Main.rand.Next(SavannaFoliage.StyleRange)))
				NetMessage.SendTileSquare(-1, i, j - 1);
		}

		bool GrassAny()
		{
			int type = ModContent.TileType<ElephantGrassCrimson>();
			return Framing.GetTileSafely(i - 1, j - 1).TileType == type || Framing.GetTileSafely(i + 1, j - 1).TileType == type;
		}
	}
}

public class SavannaGrassHallow : SavannaGrass
{
	protected override Color MapColor => new(78, 193, 227);

	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();

		TileID.Sets.Hallow[Type] = true;
		TileID.Sets.HallowBiome[Type] = 20;
	}

	protected override void GrowTiles(int i, int j)
	{
		var above = Framing.GetTileSafely(i, j - 1);
		if (!above.HasTile && above.LiquidAmount < 80)
		{
			int grassChance = GrassAny() ? 6 : 40;

			if (Main.rand.NextBool(grassChance) && WorldGen.PlaceTile(i, j - 1, ModContent.TileType<ElephantGrassHallow>(), true, style: Main.rand.Next(5, 8)))
				NetMessage.SendTileSquare(-1, i, j - 2, 1, 2, TileChangeType.None);

			if (Main.rand.NextBool(15) && WorldGen.PlaceTile(i, j - 1, ModContent.TileType<SavannaFoliageHallow>(), true, style: Main.rand.Next(SavannaFoliage.StyleRange)))
				NetMessage.SendTileSquare(-1, i, j - 1);

			if (Main.rand.NextBool(1400) && WorldGen.PlaceTile(i, j - 1, TileID.DyePlants, true, style: 2))
				NetMessage.SendTileSquare(-1, i, j - 1, TileChangeType.None);
		}

		bool GrassAny()
		{
			int type = ModContent.TileType<ElephantGrassHallow>();
			return Framing.GetTileSafely(i - 1, j - 1).TileType == type || Framing.GetTileSafely(i + 1, j - 1).TileType == type;
		}
	}
}