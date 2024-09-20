using SpiritReforged.Common.WallCommon;

namespace SpiritReforged.Content.Savanna.Walls;

public class LivingBaobabWall : ModWall, IAutoloadUnsafeWall, IAutoloadWallItem
{
	public override void SetStaticDefaults()
	{
		Main.wallHouse[Type] = true;
		DustType = DustID.WoodFurniture;
		AddMapEntry(new Color(107, 70, 50));
	}

	public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
}