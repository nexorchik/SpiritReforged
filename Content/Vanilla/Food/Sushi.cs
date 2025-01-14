using SpiritReforged.Common.ItemCommon;
using SpiritReforged.Content.Ocean.Items;

namespace SpiritReforged.Content.Vanilla.Food;

public class Sushi : FoodItem
{
	internal override Point Size => new(44, 28);

	public override bool CanUseItem(Player player)
	{
		player.AddBuff(BuffID.Gills, 1800);
		return true;
	}

	public override void AddRecipes()
	{
		Recipe recipe1 = CreateRecipe(1);
		recipe1.AddIngredient(ModContent.ItemType<Kelp>(), 5);
		recipe1.AddIngredient(ModContent.ItemType<RawFish>(), 1);
		recipe1.AddTile(TileID.CookingPots);
		recipe1.Register();
	}
}
