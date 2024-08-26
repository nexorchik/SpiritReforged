using SpiritReforged.Content.Cloudstalk.Buffs;

namespace SpiritReforged.Content.Cloudstalk.Items;

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

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient(ItemID.BottledWater);
		recipe.AddIngredient(ModContent.ItemType<Cloudstalk>());
		recipe.AddIngredient(ItemID.SoulofFlight, 5);
		recipe.AddIngredient(ItemID.Damselfish);
		recipe.AddTile(TileID.Bottles);
		recipe.Register();
	}
}
