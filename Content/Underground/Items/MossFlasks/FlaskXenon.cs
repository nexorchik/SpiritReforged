namespace SpiritReforged.Content.Underground.Items.MossFlasks;

public class FlaskXenon : MossFlask
{
	public override void AddRecipes() => CreateRecipe().AddIngredient(ItemID.XenonMoss, 3).AddIngredient(ItemID.Bottle).Register();
}

public class FlaskXenonProjectile : MossFlaskProjectile
{
	public override (ushort, ushort) Types => (TileID.XenonMoss, TileID.XenonMossBrick);
}