using SpiritReforged.Common.NPCCommon;
using Terraria.DataStructures;

namespace SpiritReforged.Content.Forest.Botanist.Items;

public class WheatgrassSeedPouch : ModItem
{
	public override void SetStaticDefaults() => NPCShopHelper.AddEntry(new NPCShopHelper.ConditionalEntry((shop) => shop.NpcType == NPCID.Merchant, 
		new NPCShop.Entry(ModContent.ItemType<WheatgrassSeedPouch>(), Condition.DownedEyeOfCthulhu)));

	public override void SetDefaults()
	{
		Item.width = Item.height = 26;
		Item.value = Item.sellPrice(0, 0, 0, 5);
		Item.rare = ItemRarityID.Blue;
		Item.shoot = ModContent.ProjectileType<WheatgrassSeedProjectile>();
		Item.useStyle = ItemUseStyleID.Swing;
		Item.shootSpeed = 8;
		Item.useTime = Item.useAnimation = 25;
		Item.consumable = true;
		Item.maxStack = Item.CommonMaxStack;
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		for (int i = 0; i < 6; ++i)
			Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.4f) * Main.rand.NextFloat(0.5f, 1f), type, 0, knockback, player.whoAmI);	

		return false;
	}
}
