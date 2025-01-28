using SpiritReforged.Common.MathHelpers;
using SpiritReforged.Content.Ocean.Items.Reefhunter.Projectiles;
using Terraria.DataStructures;
using Terraria.Audio;

namespace SpiritReforged.Content.Ocean.Items.Reefhunter;

public class UrchinStaff : ModItem
{
	public override void SetDefaults()
	{
		Item.damage = 18;
		Item.width = 28;
		Item.height = 14;
		Item.useTime = Item.useAnimation = 24;
		Item.reuseDelay = 6;
		Item.knockBack = 2f;
		Item.shootSpeed = UrchinBall.MAX_SPEED;
		Item.noUseGraphic = true;
		Item.noMelee = true;
		Item.autoReuse = true;
		Item.DamageType = DamageClass.Magic;
		Item.mana = 10;
		Item.rare = ItemRarityID.Blue;
		Item.value = Item.sellPrice(gold: 2);
		Item.useStyle = ItemUseStyleID.Swing;
		Item.shoot = ModContent.ProjectileType<UrchinStaffProjectile>();
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		SoundEngine.PlaySound(new SoundStyle("SpiritReforged/Assets/SFX/Projectile/Impact_LightPop") with { PitchVariance = 0.4f, Pitch = -2f, Volume = .75f, MaxInstances = 3 }, player.Center);

		Vector2 targetPos = Main.MouseWorld;
		Vector2 shotTrajectory = player.GetArcVel(targetPos, 0.25f, velocity.Length());
		var proj = Projectile.NewProjectileDirect(source, player.MountedCenter, Vector2.Zero, type, damage, knockback, player.whoAmI);

		if (proj.ModProjectile is UrchinStaffProjectile staffProj)
		{
			staffProj.ShotTrajectory = shotTrajectory;
			staffProj.RelativeTargetPosition = Main.MouseWorld - player.MountedCenter;
			if (Main.netMode != NetmodeID.SinglePlayer) //sync extra ai as projectile is made
				NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj.whoAmI);
		}

		return false;
	}

	public override void AddRecipes() => CreateRecipe().AddIngredient(ModContent.ItemType<IridescentScale>(), 8)
		.AddIngredient(ModContent.ItemType<MineralSlag>(), 10).AddTile(TileID.Anvils).Register();
}