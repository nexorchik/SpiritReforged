using SpiritReforged.Common.ItemCommon;
using SpiritReforged.Content.Ocean.Items;

namespace SpiritReforged.Content.Vanilla.Items.Food;
public class Nigiri : FoodItem
{
	internal override Point Size => new(44, 28);

	public override bool CanUseItem(Player player)
	{
		player.AddBuff(BuffID.Flipper, 3600);
		return true;
	}

	public override void AddRecipes()
	{
		Recipe recipe1 = CreateRecipe(1);
		recipe1.AddIngredient(ModContent.ItemType<Kelp>(), 7);
		recipe1.AddIngredient(ModContent.ItemType<RawFish>(), 1);
		recipe1.AddTile(TileID.CookingPots);
		recipe1.Register();
	}
}

