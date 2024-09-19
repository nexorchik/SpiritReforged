namespace SpiritReforged.Content.Savanna.Items.Gar;

public class QuenchPotion : ModItem
{
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
public class QuenchPotion_Buff : ModBuff
{
	public override void Update(Player player, ref int buffIndex) => player.GetModPlayer<SavannaPlayer>().quenchPotion = true;
}