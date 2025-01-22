using SpiritReforged.Common.Visuals.Glowmasks;

namespace SpiritReforged.Content.Ocean.Items.JellyCandle;

[AutoloadGlowmask("255, 255, 255")]
public class JellyCandle : ModItem
{
	public override void SetStaticDefaults() => ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.WaterCandle;

	public override void SetDefaults()
	{
		Item.CloneDefaults(ItemID.Fish);
		Item.width = 30;
		Item.height = 46;
		Item.shoot = ModContent.ProjectileType<JellyfishPet>();
		Item.buffType = ModContent.BuffType<JellyfishBuff>();
	}

	public override void UseStyle(Player player, Rectangle heldItemFrame)
	{
		if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
			player.AddBuff(Item.buffType, 3600, true);
	}

	public override bool CanUseItem(Player player) => player.miscEquips[0].IsAir;

	public override void Update(ref float gravity, ref float maxFallSpeed) => Lighting.AddLight(Item.position, .224f, .133f, .255f);

}