using SpiritReforged.Common.TileCommon.PresetTiles;
using SpiritReforged.Common.TileCommon.Tree;

namespace SpiritReforged.Content.Savanna.Tiles.AcaciaTree;

public class AcaciaSapling : SaplingTile<AcaciaTree>
{
	public override int[] AnchorTypes => [ModContent.TileType<SavannaGrass>()];
}

public class AcaciaSaplingCorrupt : AcaciaSapling
{
	public override int[] AnchorTypes => [ModContent.TileType<SavannaGrassCorrupt>()];

	public override void RandomUpdate(int i, int j)
	{
		if (WorldGen.genRand.NextBool(20))
			CustomTree.GrowTree<AcaciaTreeCorrupt>(i, j);
	}
}

public class AcaciaSaplingCrimson : AcaciaSapling
{
	public override int[] AnchorTypes => [ModContent.TileType<SavannaGrassCrimson>()];

	public override void RandomUpdate(int i, int j)
	{
		if (WorldGen.genRand.NextBool(20))
			CustomTree.GrowTree<AcaciaTreeCrimson>(i, j);
	}
}

public class AcaciaSaplingHallow : AcaciaSapling
{
	public override int[] AnchorTypes => [ModContent.TileType<SavannaGrassHallow>()];

	public override void RandomUpdate(int i, int j)
	{
		if (WorldGen.genRand.NextBool(20))
			CustomTree.GrowTree<AcaciaTreeHallow>(i, j);
	}
}