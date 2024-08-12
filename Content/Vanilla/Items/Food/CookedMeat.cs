using SpiritReforged.Common.ItemCommon;

namespace SpiritReforged.Content.Vanilla.Items.Food;

public class CookedMeat : FoodItem
{
	internal override Point Size => new(30, 22);

	public override bool CanUseItem(Player player)
	{
		return true;
	}
	public override void Defaults()
	{
		Item.buffTime = 6 * 60 * 60;
	}
	public override void AddRecipes()
	{
		Recipe recipe1 = CreateRecipe(1);
		recipe1.AddIngredient(ModContent.ItemType<RawMeat>(), 1);
		recipe1.AddTile(TileID.Campfire);
		recipe1.Register();
		Recipe recipe2 = CreateRecipe(2);
		recipe2.AddIngredient(ModContent.ItemType<RawMeat>(), 1);
		recipe2.AddTile(TileID.CookingPots);
		recipe2.Register();
	}
}

