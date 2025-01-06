using SpiritReforged.Common.WallCommon;
using SpiritReforged.Content.Ocean.Items.Driftwood;

namespace SpiritReforged.Content.Ocean.Walls;

public class DriftwoodFence : ModWall, IAutoloadWallItem
{
	public void AddItemRecipes(ModItem item)
	{
		var mod = SpiritReforgedMod.Instance; //Mod is null here, so get the instance manually

		item.CreateRecipe(4)
			.AddIngredient(ModContent.ItemType<DriftwoodTileItem>())
			.AddTile(TileID.WorkBenches)
			.Register();

		//Allow fence items to be crafted back into base materials
		Recipe.Create(mod.Find<ModItem>("DriftwoodFenceItem").Type)
			.AddIngredient(item.Type, 4)
			.AddTile(TileID.WorkBenches)
			.Register();
	}

	public override void SetStaticDefaults()
	{
		WallID.Sets.AllowsPlantsToGrow[Type] = true;
		Main.wallHouse[Type] = true;
		DustType = DustID.BorealWood;
	}

	public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
}