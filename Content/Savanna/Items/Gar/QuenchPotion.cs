using SpiritReforged.Common.BuffCommon;

namespace SpiritReforged.Content.Savanna.Items.Gar;

public class QuenchPotion : ModItem
{
	public override void Load() => BuffHooks.ModifyBuffTime += QuenchifyBuff;

	private void QuenchifyBuff(int buffType, ref int buffTime, Player player, bool quickBuff)
	{
		bool Quenched() => player.HasBuff<QuenchPotion_Buff>() || quickBuff 
			&& player.HasItemInInventoryOrOpenVoidBag(Item.type) && player.CountBuffs() + 1 <= Player.MaxBuffs;

		if (Main.debuff[buffType] || buffType == ModContent.BuffType<QuenchPotion_Buff>() || !Quenched())
			return;

		buffTime = (int)(buffTime * 1.25f);
	}

	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 30;
		Item.rare = ItemRarityID.Blue;
		Item.maxStack = Item.CommonMaxStack;
		Item.useStyle = ItemUseStyleID.DrinkLiquid;
		Item.useTime = Item.useAnimation = 20;
		Item.consumable = true;
		Item.autoReuse = false;
		Item.buffType = ModContent.BuffType<QuenchPotion_Buff>();
		Item.buffTime = 60 * 45;
		Item.value = 200;
		Item.UseSound = SoundID.Item3;
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient(Mod.Find<ModItem>("GarItem").Type, 1);
		recipe.AddIngredient(ItemID.Blinkroot, 1);
		recipe.AddIngredient(ItemID.Moonglow, 1);
		recipe.AddIngredient(ItemID.Waterleaf, 1);
		recipe.AddIngredient(ItemID.BottledWater, 1);
		recipe.AddTile(TileID.Bottles);
		recipe.Register();
	}
}

public class QuenchPotion_Buff : ModBuff { }