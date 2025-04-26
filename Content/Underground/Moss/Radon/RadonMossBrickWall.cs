using SpiritReforged.Common.ItemCommon;
using SpiritReforged.Common.WallCommon;

namespace SpiritReforged.Content.Underground.Moss.Radon;

public class RadonMossBrickWall : ModWall, IAutoloadWallItem
{
	public void AddItemRecipes(ModItem item)
	{
		item.CreateRecipe(4).AddIngredient(ItemMethods.AutoItemType<RadonMossBrick>()).AddTile(TileID.WorkBenches).Register();

		//Allow wall items to be crafted back into base materials
		Recipe.Create(ItemMethods.AutoItemType<RadonMossBrick>()).AddIngredient(item.Type, 4).AddTile(TileID.WorkBenches).Register();
	}

	public override void SetStaticDefaults()
	{
		Main.wallHouse[Type] = true;
		AddMapEntry(new Color(126, 124, 1));
	}

	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) => (r, g, b) = (0.45f, 0.425f, 0.05f);
}