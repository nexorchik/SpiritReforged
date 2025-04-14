using SpiritReforged.Common.ItemCommon;

namespace SpiritReforged.Content.Underground.Items.OreClubs;

public class GoldClub() : ClubItem()
{
	public override void SafeSetDefaults()
	{
		Item.damage = 18;
		Item.knockBack = 5;
		ChargeTime = 60;
		SwingTime = 24;
		Item.width = 60;
		Item.height = 60;
		Item.crit = 4;
		Item.value = Item.sellPrice(0, 0, 1, 0);
		Item.rare = ItemRarityID.White;
		Item.shoot = ModContent.ProjectileType<GoldClubProj>();
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient(ItemID.GoldBar, 20);
		recipe.AddTile(TileID.Anvils);
		recipe.Register();
	}
}