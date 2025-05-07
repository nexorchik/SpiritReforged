using SpiritReforged.Common.ItemCommon;

namespace SpiritReforged.Content.Underground.Items.OreClubs;

public class GoldClub : ClubItem
{
	internal override float DamageScaling => 2f;
	internal override float KnockbackScaling => 2f;

	public override void SafeSetDefaults()
	{
		Item.damage = 35;
		Item.knockBack = 8;
		ChargeTime = 45;
		SwingTime = 35;
		Item.width = 60;
		Item.height = 60;
		Item.crit = 4;
		Item.value = Item.sellPrice(0, 0, 1, 0);
		Item.rare = ItemRarityID.White;
		Item.shoot = ModContent.ProjectileType<GoldClubProj>();
	}

	public override void AddRecipes() => CreateRecipe().AddIngredient(ItemID.GoldBar, 20).AddTile(TileID.Anvils).Register();
}