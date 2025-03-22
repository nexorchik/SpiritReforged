using SpiritReforged.Common.WallCommon;

namespace SpiritReforged.Content.Savanna.Walls;

public class LivingBaobabLeafWall : ModWall, IAutoloadUnsafeWall, IAutoloadWallItem
{
	public override void SetStaticDefaults()
	{
		Main.wallHouse[Type] = true;
		DustType = DustID.WoodFurniture;

		var entryColor = new Color(71, 79, 24);
		AddMapEntry(entryColor);
		Mod.Find<ModWall>(Name + "Unsafe").AddMapEntry(entryColor); //Set the unsafe wall's map entry
	}

	public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
}