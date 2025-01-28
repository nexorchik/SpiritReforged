using SpiritReforged.Common.ItemCommon;
using Terraria.DataStructures;

namespace SpiritReforged.Content.Ocean.Items.PoolNoodle;

public class PoolNoodle : ModItem
{
	public override void SetStaticDefaults() => VariantGlobalItem.AddVariants(Type, 3, true);
	public override void SetDefaults()
	{
		Item.DefaultToWhip(ModContent.ProjectileType<PoolNoodleProj>(), 14, 0, 4);
		Item.width = Item.height = 38;
		Item.rare = ItemRarityID.Blue;
		Item.value = Item.sellPrice(silver: 30);
	}

	public override bool MeleePrefix() => true;
	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		int variant = VariantGlobalItem.GetVariant(Item);
		Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, ai1: variant);

		return false;
	}
}