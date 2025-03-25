using SpiritReforged.Common.TileCommon.Corruption;
using Terraria.DataStructures;
using Terraria.GameContent.Metadata;

namespace SpiritReforged.Content.Savanna.Tiles;

public class SavannaFoliage : ModTile, IConvertibleTile
{
	public const int StyleRange = 15;

	protected virtual int AnchorTile => ModContent.TileType<SavannaGrass>();
	protected virtual Color MapColor => new(104, 156, 7);
	protected virtual int Dust => DustID.JunglePlants;

	public override void SetStaticDefaults()
	{
		const int TileHeight = 30;

		Main.tileSolid[Type] = false;
		Main.tileFrameImportant[Type] = true;
		Main.tileNoFail[Type] = true;
		Main.tileCut[Type] = true;
		Main.tileBlockLight[Type] = false;

		TileID.Sets.SwaysInWindBasic[Type] = true;
		TileMaterials.SetForTileId(Type, TileMaterials._materialsByName["Plant"]);

		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.CoordinateWidth = 16;
		TileObjectData.newTile.CoordinateHeights = [TileHeight];
		TileObjectData.newTile.DrawYOffset = -(TileHeight - 18);
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 0);
		TileObjectData.newTile.AnchorValidTiles = [AnchorTile];
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.RandomStyleRange = StyleRange;
		TileObjectData.addTile(Type);

		AddMapEntry(MapColor);
		DustType = Dust;
		HitSound = SoundID.Grass;
	}

	public override void NumDust(int i, int j, bool fail, ref int num) => num = 3;

	public bool Convert(IEntitySource source, ConversionType type, int i, int j)
	{
		Tile tile = Main.tile[i, j];
		int oldId = tile.TileType;

		tile.TileType = (ushort)(type switch
		{
			ConversionType.Hallow => ModContent.TileType<SavannaFoliageHallow>(),
			ConversionType.Crimson => ModContent.TileType<SavannaFoliageCrimson>(),
			ConversionType.Corrupt => ModContent.TileType<SavannaFoliageCorrupt>(),
			_ => ModContent.TileType<SavannaFoliage>(),
		});

		return oldId != tile.TileType;
	}

	public override IEnumerable<Item> GetItemDrops(int i, int j)
	{
		if (Main.player[Player.FindClosest(new Vector2(i, j).ToWorldCoordinates(0, 0), 16, 16)].HeldItem.type == ItemID.Sickle)
			yield return new Item(ItemID.Hay, Main.rand.Next(1, 3));

		if (Main.player[Player.FindClosest(new Vector2(i, j).ToWorldCoordinates(0, 0), 16, 16)].HasItem(ItemID.Blowpipe))
			yield return new Item(ItemID.Seed, Main.rand.Next(1, 3));
	}
}

public class SavannaFoliageCorrupt : SavannaFoliage
{
	protected override int AnchorTile => ModContent.TileType<SavannaGrassCorrupt>();
	protected override Color MapColor => new(109, 106, 174);
	protected override int Dust => DustID.Corruption;
}

public class SavannaFoliageCrimson : SavannaFoliage
{
	protected override int AnchorTile => ModContent.TileType<SavannaGrassCrimson>();
	protected override Color MapColor => new(183, 69, 68);
	protected override int Dust => DustID.CrimsonPlants;
}

public class SavannaFoliageHallow : SavannaFoliage
{
	protected override int AnchorTile => ModContent.TileType<SavannaGrassHallow>();
	protected override Color MapColor => new(78, 193, 227);
	protected override int Dust => DustID.HallowedPlants;
}
