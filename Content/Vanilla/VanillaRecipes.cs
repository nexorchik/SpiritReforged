using SpiritReforged.Common.ItemCommon;

namespace SpiritReforged.Content.Vanilla;

public class VanillaRecipes : ModSystem
{
	public override void AddRecipeGroups() 
	{
		if (RecipeGroup.recipeGroups.TryGetValue(RecipeGroupID.Fruit, out RecipeGroup value))
			foreach (int itemId in FoodItem.FruitItemsSet)
				value.ValidItems.Add(itemId);
	}
}
