using SpiritReforged.Content.Ocean.Items.Blunderbuss;

namespace SpiritReforged.Common.ModCompat;

internal class FablesCompat : ModSystem
{
	public static Mod Instance;
	public static bool Enabled => Instance != null;

	public override void Load()
	{
		Instance = null;
		if (!ModLoader.TryGetMod("CalamityFables", out Instance))
			return;
	}
}

internal class FablesRecipes : ModSystem
{
	public override void AddRecipes()
	{
		if (FablesCompat.Enabled)
		{
			if (FablesCompat.Instance.TryFind("WulfrumBlunderbuss", out ModItem wulfrumBlunderbuss) && FablesCompat.Instance.TryFind("WulfrumMetalScrap", out ModItem wulfrumScrap))
				Recipe.Create(wulfrumBlunderbuss.Type).AddIngredient(ModContent.ItemType<Blunderbuss>()).AddIngredient(wulfrumScrap.Type, 3)
					.AddTile(TileID.Anvils).Register();
		}
	}
}

