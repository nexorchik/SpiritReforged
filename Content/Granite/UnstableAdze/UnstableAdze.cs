using SpiritReforged.Common.ItemCommon;
using SpiritReforged.Common.Visuals.Glowmasks;
using SpiritReforged.Content.Underground.Items.OreClubs;

namespace SpiritReforged.Content.Granite.UnstableAdze;

[AutoloadGlowmask("255, 255, 255")]
public class UnstableAdze : ClubItem
{
	internal override float DamageScaling => 1.5f;

	public override void SafeSetDefaults()
	{
		Item.damage = 32;
		Item.knockBack = 8;
		ChargeTime = 50;
		SwingTime = 30;
		Item.width = 60;
		Item.height = 60;
		Item.crit = 4;
		Item.value = Item.sellPrice(0, 0, 30, 0);
		Item.rare = ItemRarityID.Blue;
		Item.shoot = ModContent.ProjectileType<UnstableAdzeProj>();
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient(ItemID.PlatinumBar, 20);
		recipe.AddTile(TileID.Anvils);
		recipe.Register();
	}
}