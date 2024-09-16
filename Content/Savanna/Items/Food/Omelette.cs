using SpiritReforged.Common.ItemCommon;

namespace SpiritReforged.Content.Savanna.Items.Food;

public class Omelette : FoodItem
{
	internal override Point Size => new(36, 24);
	public override void Defaults()
	{
		FruitItemsSet.Add(Type);
		Item.buffType = BuffID.WellFed2;
	}

	public override void AddRecipes() => CreateRecipe().AddIngredient(ModContent.ItemType<OstrichEgg>())
			.AddTile(TileID.CookingPots).Register();
}

