using SpiritReforged.Common.ItemCommon;
using SpiritReforged.Common.ProjectileCommon.Abstract;
using Terraria.DataStructures;

namespace SpiritReforged.Content.Underground.Items.OreClubs;

public class GoldClub() : ClubItem()
{
	private int _combo;

	internal override float DamageScaling => 2f;
	internal override float KnockbackScaling => 2f;

	public override void SafeSetDefaults()
	{
		Item.damage = 35;
		Item.knockBack = 8;
		ChargeTime = 60;
		SwingTime = 24;
		Item.width = 60;
		Item.height = 60;
		Item.crit = 4;
		Item.value = Item.sellPrice(0, 0, 1, 0);
		Item.rare = ItemRarityID.White;
		Item.shoot = ModContent.ProjectileType<GoldClubProj>();
		_combo = 0;
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient(ItemID.GoldBar, 20);
		recipe.AddTile(TileID.Anvils);
		recipe.Register();
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		_combo++;
		_combo %= 2;

		Projectile proj = Projectile.NewProjectileDirect(source, player.Center, velocity, type, damage, knockback, player.whoAmI);

		if (proj.ModProjectile is BaseClubProj clubProj)
		{
			float speedMult = player.GetTotalAttackSpeed(DamageClass.Melee);
			float swingSpeedMult = MathHelper.Lerp(speedMult, 1, 0.5f);

			clubProj.SetStats(
				(int)(ChargeTime * MathHelper.Max(.15f, 2 - speedMult)),
				(int)(SwingTime * MathHelper.Max(.15f, 2 - swingSpeedMult)),
				DamageScaling,
				KnockbackScaling);
		}

		if (proj.ModProjectile is GoldClubProj goldClub)
		{
			goldClub.Direction = _combo == 0 ? 1 : -1;

			if(Main.netMode != NetmodeID.SinglePlayer)
				NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj.whoAmI);
		}

		return false;
	}
}