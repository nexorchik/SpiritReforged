using SpiritReforged.Common.ItemCommon;
using SpiritReforged.Content.Forest.Cloudstalk.Buffs;
using Terraria.GameContent.ItemDropRules;

namespace SpiritReforged.Content.Forest.Cloudstalk.Items;

public class DoubleJumpPotion : ModItem
{
	public override void SetStaticDefaults()
	{
		Item.ResearchUnlockCount = 20;

		CrateDatabase.AddCrateRule(ItemID.WoodenCrate, new CommonDrop(Type, 100, 1, 3, 35));
		CrateDatabase.AddCrateRule(ItemID.WoodenCrateHard, new CommonDrop(Type, 100, 1, 3, 35));
	}

	public override void SetDefaults()
	{
		Item.width = 16;
		Item.height = 32;
		Item.rare = ItemRarityID.Blue;
		Item.maxStack = Item.CommonMaxStack;
		Item.useStyle = ItemUseStyleID.DrinkLiquid;
		Item.useTime = Item.useAnimation = 20;
		Item.consumable = true;
		Item.autoReuse = false;
		Item.buffType = ModContent.BuffType<DoubleJumpPotionBuff>();
		Item.buffTime = 10800;
		Item.UseSound = SoundID.Item3;
	}

	public override void AddRecipes() => CreateRecipe().AddIngredient(ItemID.BottledWater).AddIngredient(ModContent.ItemType<Cloudstalk>())
		.AddIngredient(ItemID.Cloud, 5).AddIngredient(ItemID.Moonglow).AddTile(TileID.Bottles).Register();
}
