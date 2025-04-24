namespace SpiritReforged.Content.Underground.Items.MossFlasks;

public class FlaskKrypton : MossFlask
{
	public override void AddRecipes() => CreateRecipe().AddIngredient(ItemID.KryptonMoss, 3).AddIngredient(ItemID.Bottle).Register();
}

public class FlaskKryptonProjectile : MossFlaskProjectile
{
	public override (ushort, ushort) Types => (TileID.KryptonMoss, TileID.KryptonMossBrick);
}