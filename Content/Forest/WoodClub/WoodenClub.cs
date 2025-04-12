using SpiritReforged.Common.ItemCommon;

namespace SpiritReforged.Content.Forest.WoodClub;

public class WoodenClub() : ClubItem()
{
	public override void SafeSetDefaults()
	{
		Item.damage = 18;
		Item.knockBack = 5;
		ChargeTime = 40;
		SwingTime = 24;
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