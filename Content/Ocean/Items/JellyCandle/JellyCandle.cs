namespace SpiritReforged.Content.Ocean.Items.JellyCandle;

public class JellyCandle : ModItem
{
	//TODO: Bring Back a glowmask system, implement it here
	public override void SetStaticDefaults() => ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.WaterCandle;

	public override void SetDefaults()
	{
		Item.CloneDefaults(ItemID.Fish);
		Item.shoot = ModContent.ProjectileType<JellyfishPet>();
		Item.buffType = ModContent.BuffType<JellyfishBuff>();
	}

	public override void UseStyle(Player player, Rectangle heldItemFrame)
	{
		if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
			player.AddBuff(Item.buffType, 3600, true);
	}

	public override bool CanUseItem(Player player) => player.miscEquips[0].IsAir;

	public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200, 100);

}