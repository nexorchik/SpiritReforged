using SpiritReforged.Content.Ocean.Items.Blunderbuss;
using static SpiritReforged.Common.ModCompat.CrossMod;

namespace SpiritReforged.Common.ModCompat;

internal class FablesRecipes : ModSystem
{
	public override bool IsLoadingEnabled(Mod mod) => Fables.Enabled;

	public override void AddRecipes()
	{
		if (Fables.TryFind("WulfrumBlunderbuss", out ModItem wulfrumBlunderbuss) && Fables.TryFind("WulfrumMetalScrap", out ModItem wulfrumScrap))
			Recipe.Create(wulfrumBlunderbuss.Type).AddIngredient(ModContent.ItemType<Blunderbuss>()).AddIngredient(wulfrumScrap.Type, 3).AddTile(TileID.Anvils).Register();
	}
}