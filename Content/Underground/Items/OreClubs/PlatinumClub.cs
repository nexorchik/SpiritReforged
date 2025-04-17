using SpiritReforged.Common.ItemCommon;

namespace SpiritReforged.Content.Underground.Items.OreClubs;

public class PlatinumClub() : ClubItem()
{
	internal override float DamageScaling => 2f;

	public override void SafeSetDefaults()
	{
		Item.damage = 32;
		Item.knockBack = 8;
		ChargeTime = 60;
		SwingTime = 30;
		Item.width = 60;
		Item.height = 60;
		Item.crit = 4;
		Item.value = Item.sellPrice(0, 0, 1, 0);
		Item.rare = ItemRarityID.White;
		Item.shoot = ModContent.ProjectileType<PlatinumClubProj>();
		Item.UseSound = SoundID.DD2_MonkStaffSwing.WithPitchOffset(-0.5f);
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient(ItemID.PlatinumBar, 20);
		recipe.AddTile(TileID.Anvils);
		recipe.Register();
	}
}