using SpiritReforged.Common.Visuals.Glowmasks;
using SpiritReforged.Content.Desert.Scarabeus.Items.Projectiles;
using SpiritReforged.Content.Ocean.Items.Reefhunter.Projectiles;
using Terraria.DataStructures;

namespace SpiritReforged.Content.Desert.Scarabeus.Items;

[AutoloadGlowmask("255,255,255")]
public class SunStaff : ModItem
{
	public override void SetStaticDefaults() => Item.staff[Type] = true;
	public override void SetDefaults()
	{
		Item.damage = 18;
		Item.width = Item.height = 46;
		Item.useTime = Item.useAnimation = 20;
		Item.knockBack = 2f;
		Item.shootSpeed = 6;
		Item.noMelee = true;
		Item.autoReuse = true;
		Item.DamageType = DamageClass.Magic;
		Item.mana = 40;
		Item.rare = ItemRarityID.Green;
		Item.value = Item.sellPrice(gold: 2);
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.shoot = ModContent.ProjectileType<SunOrb>();
		Item.channel = true;
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		velocity = -Vector2.UnitY * velocity.Length();
		Projectile.NewProjectile(source, player.Center, velocity, type, damage, knockback, player.whoAmI);

		return false;
	}

	public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;
}