using SpiritReforged.Common.ItemCommon;

namespace SpiritReforged.Content.Vanilla.Items.Food;

public class CoconutMilk : FoodItem
{
	internal override Point Size => new(24, 30);

	public override bool CanUseItem(Player player)
	{
		return true;
	}
	public override void Defaults()
	{
		Item.buffTime = 10 * 60 * 60;
		Item.useStyle = ItemUseStyleID.DrinkLiquid;
		Item.UseSound = SoundID.Item3;

	}
	public override void AddRecipes()
	{
		Recipe recipe1 = CreateRecipe(1);
		recipe1.AddIngredient(ItemID.Coconut, 1);
		recipe1.AddTile(TileID.CookingPots);
		recipe1.Register();
	}
}

