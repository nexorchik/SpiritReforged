
using SpiritReforged.Common.ItemCommon;

namespace SpiritReforged.Content.Vanilla;

public class VanillaRecipes : ModSystem
{
	public override void AddRecipeGroups() 
	{
		if (RecipeGroup.recipeGroups.ContainsKey(RecipeGroupID.Fruit))
		{
			foreach (int itemId in FoodItem.FruitItemsSet)
				RecipeGroup.recipeGroups[RecipeGroupID.Fruit].ValidItems.Add(itemId);
		}
	}
}
