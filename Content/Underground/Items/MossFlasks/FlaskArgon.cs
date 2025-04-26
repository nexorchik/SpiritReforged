namespace SpiritReforged.Content.Underground.Items.MossFlasks;

public class FlaskArgon : MossFlask
{
	public override void AddRecipes() => CreateRecipe().AddIngredient(ItemID.ArgonMoss, 3).AddIngredient(ItemID.Bottle).Register();
}

public class FlaskArgonProjectile : MossFlaskProjectile
{
	public override (ushort, ushort) Types => (TileID.ArgonMoss, TileID.ArgonMossBrick);
}