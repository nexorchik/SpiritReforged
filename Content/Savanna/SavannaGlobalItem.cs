using SpiritReforged.Content.Savanna.NPCs.Gar;

namespace SpiritReforged.Content.Savanna;

public class SavannaGlobalItem : GlobalItem
{
	public override void AddRecipes()
	{
		Recipe recipe = Recipe.Create(ItemID.HunterPotion, 1);
		recipe.AddIngredient(ItemID.BottledWater)
			.AddIngredient(ItemID.Blinkroot)
			.AddIngredient(Mod.Find<ModItem>("GarItem").Type)
			.AddTile(TileID.Bottles)
			.Register();
	}
}
