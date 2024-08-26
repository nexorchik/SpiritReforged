using SpiritReforged.Common.ItemCommon;

namespace SpiritReforged.Content.Vanilla.Items.Food;
public class GoldenCaviar : FoodItem
{
	internal override Point Size => new(30, 34);

	public override void Defaults()
	{
		Item.buffType = BuffID.WellFed3;
		Item.buffTime = 50 * 60 * 60;
		Item.value = Item.sellPrice(0, 2, 0, 0);
	}

	public override bool CanUseItem(Player player)
	{
		return true;
	}
	public override void AddRecipes()
	{
		Recipe recipe1 = CreateRecipe(1);
		recipe1.AddIngredient(Mod.Find<ModItem>("GoldGarItem").Type, 1);
		recipe1.AddTile(TileID.CookingPots);
		recipe1.Register();

		Recipe recipe2 = CreateRecipe(1);
		recipe2.AddIngredient(Mod.Find<ModItem>("KillifishItem").Type, 1);
		recipe2.AddTile(TileID.CookingPots);
		recipe2.Register();
	}
}

