using SpiritReforged.Common.ItemCommon;

namespace SpiritReforged.Content.Vanilla.Items.Food;

public class FishChips : FoodItem
{
	internal override Point Size => new(42, 30);

	public override bool CanUseItem(Player player) => true;

	public override void Defaults()
	{
		Item.buffTime = 7 * 60 * 60;
		Item.value = Item.sellPrice(0, 0, 2, 0);
	}

	public override void AddRecipes()
	{
		Recipe recipe1 = CreateRecipe(1);
		recipe1.AddIngredient(ModContent.ItemType<RawFish>(), 5);
		recipe1.AddTile(TileID.CookingPots);
		recipe1.Register();
	}
}

