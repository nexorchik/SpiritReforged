namespace SpiritReforged.Content.Ocean.Items.MantarayHuntingHarpoon;

public class MantarayHuntingHarpoon : ModItem
{
	public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 30;
		Item.useTime = 20;
		Item.useAnimation = 20;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.rare = ItemRarityID.Blue;
		Item.UseSound = SoundID.Item3;
		Item.noMelee = true;
		Item.mountType = ModContent.MountType<MantarayMount>();
		Item.value = Item.sellPrice(gold: 5);
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient(ModContent.ItemType<DeepCascadeShard>(), 14);
		recipe.AddIngredient(ItemID.SharkFin, 1);
		recipe.AddTile(TileID.Anvils);
		recipe.Register();
	}
}