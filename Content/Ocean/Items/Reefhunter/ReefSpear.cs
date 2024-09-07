using SpiritReforged.Common.MathHelpers;
using SpiritReforged.Content.Ocean.Items.Reefhunter.Projectiles;

namespace SpiritReforged.Content.Ocean.Items.Reefhunter;

public class ReefSpear : ModItem
{
	public override void SetDefaults()
	{
		Item.damage = 18;
		Item.width = 28;
		Item.height = 14;
		Item.useTime = Item.useAnimation = 30;
		Item.knockBack = 2f;
		Item.shootSpeed = 0f;
		Item.noUseGraphic = true;
		Item.noMelee = true;
		Item.DamageType = DamageClass.Melee;
		Item.channel = false;
		Item.autoReuse = true;
		Item.rare = ItemRarityID.Blue;
		Item.value = Item.sellPrice(gold: 2);
		Item.UseSound = SoundID.Item1;
		Item.shoot = ModContent.ProjectileType<ReefSpearProjectile>();
		Item.useStyle = ItemUseStyleID.Shoot;
	}

	public override Vector2? HoldoutOffset() => new Vector2(-6, 0);
	public override bool AltFunctionUse(Player player) => true;

	public override bool CanUseItem(Player player)
	{
		if (player.altFunctionUse == 2)
		{
			Item.shoot = ModContent.ProjectileType<ReefSpearThrown>();
			Item.damage = 24;
			Item.shootSpeed = 16f;
			Item.channel = false;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = Item.useAnimation = 35;

		}
		else
		{
			Item.shoot = ModContent.ProjectileType<ReefSpearProjectile>();
			Item.damage = 18;
			Item.shootSpeed = 0f;
			Item.channel = true;
			Item.useTime = Item.useAnimation = 40;
			Item.useStyle = ItemUseStyleID.Shoot;
		}

		return true;
	}

	public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
	{
		if (player.altFunctionUse == 2)
		{
			position -= new Vector2(20 * player.direction, 0);
			velocity = ArcVelocityHelper.GetArcVel(position, Main.MouseWorld, 0.4f, Item.shootSpeed) + player.velocity / 3;
		}
	}

	public override void AddRecipes()
	{
		var recipe = CreateRecipe();
		recipe.AddIngredient(ModContent.ItemType<IridescentScale>(), 10);
		recipe.AddIngredient(ModContent.ItemType<SulfurDeposit>(), 12);
		recipe.AddTile(TileID.Anvils);
		recipe.Register();
	}
}