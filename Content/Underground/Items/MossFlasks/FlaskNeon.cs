namespace SpiritReforged.Content.Underground.Items.MossFlasks;

public class FlaskNeon : MossFlask
{
	public override void AddRecipes() => CreateRecipe().AddIngredient(ItemID.VioletMoss, 3).AddIngredient(ItemID.Bottle).Register();
}

public class FlaskNeonProjectile : MossFlaskProjectile
{
	public override (ushort, ushort) Types => (TileID.VioletMoss, TileID.VioletMossBrick);
}