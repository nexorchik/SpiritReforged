using SpiritReforged.Content.Underground.Moss.Radon;

namespace SpiritReforged.Content.Underground.Items.MossFlasks;

public class FlaskRadon : MossFlask
{
	public override void AddRecipes() => CreateRecipe().AddIngredient(ModContent.ItemType<RadonMossItem>(), 3).AddIngredient(ItemID.Bottle).Register();
}

public class FlaskRadonProjectile : MossFlaskProjectile
{
	public override (ushort, ushort) Types => ((ushort)ModContent.TileType<RadonMoss>(), (ushort)ModContent.TileType<RadonMossGrayBrick>());
}