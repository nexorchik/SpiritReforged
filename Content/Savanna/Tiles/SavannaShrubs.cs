using SpiritReforged.Common.TileCommon.Corruption;
using SpiritReforged.Content.Savanna.Items;
using Terraria.DataStructures;

namespace SpiritReforged.Content.Savanna.Tiles;

public class SavannaShrubs : ModTile, IConvertibleTile
{
	protected virtual int[] Anchors => [ModContent.TileType<SavannaGrass>(), ModContent.TileType<SavannaDirt>(), TileID.Sand];

	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = false;
		Main.tileMergeDirt[Type] = false;
		Main.tileBlockLight[Type] = false;
		Main.tileFrameImportant[Type] = true;
		Main.tileNoFail[Type] = true;

		TileID.Sets.BreakableWhenPlacing[Type] = true;
		TileID.Sets.SwaysInWindBasic[Type] = true;

		const int height = 44;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.CoordinateWidth = 56;
		TileObjectData.newTile.CoordinateHeights = [height];
		TileObjectData.newTile.DrawYOffset = -(height - 18 - 4); //4 pixels are reserved for the tile space below
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 0);
		TileObjectData.newTile.AnchorValidTiles = Anchors;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.RandomStyleRange = 11;
		TileObjectData.addTile(Type);

		AddMapEntry(new Color(50, 92, 19));
		DustType = DustID.Grass;
		HitSound = SoundID.Grass;
	}

	public override void NumDust(int i, int j, bool fail, ref int num) => num = 3;

	public bool Convert(IEntitySource source, ConversionType type, int i, int j)
	{
		Tile tile = Main.tile[i, j];
		int oldId = tile.TileType;

		tile.TileType = (ushort)(type switch
		{
			ConversionType.Hallow => ModContent.TileType<SavannaShrubsHallow>(),
			ConversionType.Crimson => ModContent.TileType<SavannaShrubsCrimson>(),
			ConversionType.Corrupt => ModContent.TileType<SavannaShrubsCorrupt>(),
			_ => ModContent.TileType<SavannaShrubs>(),
		});

		return oldId != tile.TileType;
	}
}

public class SavannaShrubsCorrupt : SavannaShrubs
{
	protected override int[] Anchors => [ModContent.TileType<SavannaGrassCorrupt>(), TileID.Ebonsand];
}

public class SavannaShrubsCrimson : SavannaShrubs
{
	protected override int[] Anchors => [ModContent.TileType<SavannaGrassCrimson>(), TileID.Crimsand];
}

public class SavannaShrubsHallow : SavannaShrubs
{
	protected override int[] Anchors => [ModContent.TileType<SavannaGrassHallow>(), TileID.Pearlsand];
}

public class SavannaShrubsRubble : SavannaShrubs
{
	public override string Texture => base.Texture.Remove(base.Texture.Length - 6, 6); //Remove "Rubble"

	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();

		int styleRange = TileObjectData.GetTileData(Type, 0).RandomStyleRange;
		TileObjectData.GetTileData(Type, 0).RandomStyleRange = 0; //Random style breaks rubble placement

		for (int i = 0; i < styleRange; i++)
			FlexibleTileWand.RubblePlacementSmall.AddVariation(ModContent.ItemType<SavannaGrassSeeds>(), Type, i);

		RegisterItemDrop(ModContent.ItemType<SavannaGrassSeeds>());
	}
}
