using SpiritReforged.Common.ItemCommon;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritReforged.Content.Forest.WoodClub;

public class WoodenClub() : ClubItem(50, 10, 35, 5, 8)
{
	public override void Defaults()
	{
		Item.width = 60;
		Item.height = 60;
		Item.crit = 4;
		Item.value = Item.sellPrice(0, 0, 1, 0);
		Item.rare = ItemRarityID.White;
		Item.shoot = ModContent.ProjectileType<WoodClubProj>();
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient(ItemID.Wood, 30);
		recipe.AddTile(TileID.WorkBenches);
		recipe.Register();
	}
}