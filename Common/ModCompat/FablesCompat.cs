using SpiritReforged.Content.Ocean.Items.Blunderbuss;

namespace SpiritReforged.Common.ModCompat;

internal class FablesRecipes : ModSystem
{
	public override void AddRecipes()
	{
		if (CrossMod.Fables.Enabled)
		{
			if (CrossMod.Fables.Instance.TryFind("WulfrumBlunderbuss", out ModItem wulfrumBlunderbuss) && CrossMod.Fables.Instance.TryFind("WulfrumMetalScrap", out ModItem wulfrumScrap))
				Recipe.Create(wulfrumBlunderbuss.Type).AddIngredient(ModContent.ItemType<Blunderbuss>()).AddIngredient(wulfrumScrap.Type, 3).AddTile(TileID.Anvils).Register();
		}
	}
}