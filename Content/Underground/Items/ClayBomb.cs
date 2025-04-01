using SpiritReforged.Common.ProjectileCommon.Abstract;

namespace SpiritReforged.Content.Underground.Items;

public class ClayBomb : ModItem
{
	public override void SetDefaults()
	{
		Item.CloneDefaults(ItemID.DirtBomb);
		Item.shoot = ModContent.ProjectileType<ClayBombProjectile>();
	}

	public override void AddRecipes() => CreateRecipe().AddIngredient(ItemID.Bomb).AddIngredient(ItemID.ClayBlock, 25).Register();
}

public class ClayBombProjectile : SpreadBomb
{
	public override string Texture => base.Texture.Replace("Projectile", string.Empty);
	public override LocalizedText DisplayName => Language.GetText("Mods.SpiritReforged.Items.ClayBomb.DisplayName");

	public override void SetDefaults()
	{
		base.SetDefaults();

		area = 4;
		dustType = DustID.Clay;
		tileType = TileID.ClayBlock;
	}
}