using SpiritReforged.Common.TileCommon.Corruption;
using SpiritReforged.Common.TileCommon.TileSway;
using Terraria.DataStructures;
using static Terraria.GameContent.Drawing.TileDrawing;

namespace SpiritReforged.Content.Savanna.Tiles;

public class SavannaVine : ModTile, ISwayTile, IConvertibleTile
{
	public int Style => (int)TileCounterType.Vine;

	public override void SetStaticDefaults()
	{
		Main.tileBlockLight[Type] = true;
		Main.tileCut[Type] = true;
		Main.tileNoFail[Type] = true;
		Main.tileLavaDeath[Type] = true;

		TileID.Sets.IsVine[Type] = true;
		TileID.Sets.VineThreads[Type] = true;
		TileID.Sets.ReplaceTileBreakDown[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
		TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.AlternateTile, 1, 0);
		TileObjectData.newTile.AnchorValidTiles = [ModContent.TileType<SavannaGrass>(), ModContent.TileType<SavannaGrassCorrupt>(),
			ModContent.TileType<SavannaGrassCrimson>(), ModContent.TileType<SavannaGrassHallow>()];
		TileObjectData.newTile.AnchorAlternateTiles = [Type];
		TileObjectData.addTile(Type);

		if (Type == ModContent.TileType<SavannaVine>())
			AddMapEntry(new Color(24, 135, 28)); //Don't set on inheriting tiles

		DustType = DustID.JunglePlants;
		HitSound = SoundID.Grass;
	}

	public override void NumDust(int i, int j, bool fail, ref int num) => num = 3;

	public bool Convert(IEntitySource source, ConversionType type, int i, int j)
	{
		if (source is EntitySource_Parent { Entity: Projectile })
			return false;

		var tile = Main.tile[i, j];

		tile.TileType = (ushort)(type switch
		{
			ConversionType.Hallow => ModContent.TileType<SavannaVineHallow>(),
			ConversionType.Crimson => ModContent.TileType<SavannaVineCrimson>(),
			ConversionType.Corrupt => ModContent.TileType<SavannaVineCorrupt>(),
			_ => ModContent.TileType<SavannaVine>(),
		});

		return true;
	}
}

public class SavannaVineCorrupt : SavannaVine
{
	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();

		TileID.Sets.AddCorruptionTile(Type);
		TileID.Sets.Corrupt[Type] = true;

		AddMapEntry(new(109, 106, 174));
		DustType = DustID.Corruption;
	}
}

public class SavannaVineCrimson : SavannaVine
{
	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();

		TileID.Sets.AddCrimsonTile(Type);
		TileID.Sets.Crimson[Type] = true;

		AddMapEntry(new(183, 69, 68));
		DustType = DustID.CrimsonPlants;
	}
}

public class SavannaVineHallow : SavannaVine
{
	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();

		TileID.Sets.Hallow[Type] = true;
		TileID.Sets.HallowBiome[Type] = 1;

		AddMapEntry(new(78, 193, 227));
		DustType = DustID.HallowedPlants;
	}
}