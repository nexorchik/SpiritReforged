using SpiritReforged.Content.Ocean.Items;

namespace SpiritReforged.Content.Ocean;

public class OceanRecipes : ModSystem
{
	public override void AddRecipes()
	{
		Recipe.Create(Mod.Find<ModItem>("GravelItem").Type, 10)
			.AddIngredient(ModContent.ItemType<MineralSlag>())
			.AddTile(TileID.Bottles)
			.Register();
	}
}
