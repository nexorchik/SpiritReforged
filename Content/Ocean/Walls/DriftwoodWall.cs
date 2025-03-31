using SpiritReforged.Common.ItemCommon;
using SpiritReforged.Common.WallCommon;
using SpiritReforged.Content.Ocean.Tiles;

namespace SpiritReforged.Content.Ocean.Walls;

public class DriftwoodWall : ModWall, IAutoloadWallItem
{
	public void AddItemRecipes(ModItem item)
	{
		item.CreateRecipe(4).AddIngredient(ItemMethods.AutoItemType<Driftwood>()).AddTile(TileID.WorkBenches).Register();

		//Allow wall items to be crafted back into base materials
		Recipe.Create(ItemMethods.AutoItemType<Driftwood>()).AddIngredient(item.Type, 4)
			.AddTile(TileID.WorkBenches).Register();
	}

	public override void SetStaticDefaults()
	{
		Main.wallHouse[Type] = true;
		AddMapEntry(new Color(87, 61, 44));
	}
}