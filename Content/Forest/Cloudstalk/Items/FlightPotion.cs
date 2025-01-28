using SpiritReforged.Content.Forest.Cloudstalk.Buffs;

namespace SpiritReforged.Content.Forest.Cloudstalk.Items;

public class FlightPotion : ModItem
{
	public override void SetStaticDefaults() => Item.ResearchUnlockCount = 20;

	public override void SetDefaults()
	{
		Item.width = 32;
		Item.height = 26;
		Item.rare = ItemRarityID.Blue;
		Item.maxStack = Item.CommonMaxStack;
		Item.useStyle = ItemUseStyleID.DrinkLiquid;
		Item.useTime = Item.useAnimation = 20;
		Item.consumable = true;
		Item.autoReuse = false;
		Item.buffType = ModContent.BuffType<FlightPotionBuff>();
		Item.buffTime = 14400;
		Item.UseSound = SoundID.Item3;
	}

	public override void AddRecipes() => CreateRecipe().AddIngredient(ItemID.BottledWater).AddIngredient(ModContent.ItemType<Cloudstalk>())
		.AddIngredient(ItemID.SoulofFlight, 5).AddIngredient(ItemID.Damselfish).AddTile(TileID.Bottles).Register();
}
