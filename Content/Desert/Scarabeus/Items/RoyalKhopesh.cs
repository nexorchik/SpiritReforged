using SpiritReforged.Content.Desert.Scarabeus.Items.Projectiles;
using Terraria.DataStructures;

namespace SpiritReforged.Content.Desert.Scarabeus.Items;

public class RoyalKhopesh : ModItem
{
	private int _combo;
	public override void SetDefaults()
	{
		Item.damage = 24;
		Item.Size = new Vector2(48, 52);
		Item.useTime = Item.useAnimation = 36;
		Item.knockBack = 1f;
		Item.shootSpeed = 1;
		Item.noMelee = true;
		Item.channel = true;
		Item.noUseGraphic = true;
		Item.DamageType = DamageClass.Melee;
		Item.useTurn = false;
		Item.autoReuse = true;
		Item.rare = ItemRarityID.Green;
		Item.value = Item.sellPrice(gold: 2);
		Item.useStyle = ItemUseStyleID.Swing;
		Item.shoot = ModContent.ProjectileType<RoyalKhopeshHeld>();
		Item.UseSound = SoundID.DD2_MonkStaffSwing;
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		var p = Projectile.NewProjectileDirect(source, player.Center, Vector2.Zero, Item.shoot, Item.damage, Item.knockBack, player.whoAmI);
		if (p.ModProjectile != null)
		{
			if (p.ModProjectile is RoyalKhopeshHeld khopesh)
			{
				khopesh.BaseDirection = velocity;
				khopesh.SwingDirection = (_combo % 2 == 0 ? 1 : -1) * player.direction;
				khopesh.SwingTime = Item.useTime;
				khopesh.SwingRadians = MathHelper.Pi * 1.75f;
				p.netUpdate = true;
			}
		}

		_combo++;
		_combo %= 4;

		return false;
	}

	public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] == 0;
}