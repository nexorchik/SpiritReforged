using SpiritReforged.Common.WallCommon;
using SpiritReforged.Content.Savanna.Items.Drywood;

namespace SpiritReforged.Content.Savanna.Walls;

public class DrywoodWall : ModWall, IAutoloadWallItem
{
	public void AddItemRecipes(ModItem item)
	{
		item.CreateRecipe(4)
			.AddIngredient(ModContent.ItemType<Drywood>())
			.AddTile(TileID.WorkBenches)
			.Register();

		//Allow wall items to be crafted back into base materials
		Recipe.Create(ModContent.ItemType<Drywood>())
			.AddIngredient(item.Type, 4)
			.AddTile(TileID.WorkBenches)
			.Register();
	}

	public override void SetStaticDefaults()
	{
		Main.wallHouse[Type] = true;
		DustType = DustID.WoodFurniture;
		AddMapEntry(new Color(107, 70, 50));
	}

	public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
}