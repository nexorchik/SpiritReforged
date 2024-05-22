namespace SpiritReforged.Content.Ocean.Items.BubbleMine;

public class BubbleMine : ModItem
{
	public override void SetDefaults()
	{
		Item.CloneDefaults(ItemID.Shuriken);
		Item.width = 37;
		Item.height = 26;
		Item.shoot = ModContent.ProjectileType<BubbleMineProj>();
		Item.useAnimation = 30;
		Item.useTime = 30;
		Item.shootSpeed = 11f;
		Item.damage = 18;
		Item.knockBack = 1.0f;
		Item.value = Item.sellPrice(0, 0, 0, 5);
		Item.crit = 8;
		Item.rare = ItemRarityID.Blue;
		Item.DamageType = DamageClass.Ranged;
		Item.autoReuse = false;
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe(70);
		recipe.AddIngredient(ModContent.ItemType<DeepCascadeShard>(), 5);
		recipe.AddTile(TileID.Anvils);
		recipe.Register();
	}
}
