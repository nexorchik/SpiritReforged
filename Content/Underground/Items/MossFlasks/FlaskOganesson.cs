using SpiritReforged.Content.Underground.Moss.Oganesson;

namespace SpiritReforged.Content.Underground.Items.MossFlasks;

public class FlaskOganesson : MossFlask
{
	public override void AddRecipes() => CreateRecipe().AddIngredient(ModContent.ItemType<OganessonMossItem>(), 3).AddIngredient(ItemID.Bottle).Register();
}

public class FlaskOganessonProjectile : MossFlaskProjectile
{
	public override (ushort, ushort) Types => ((ushort)ModContent.TileType<OganessonMoss>(), (ushort)ModContent.TileType<OganessonMossGrayBrick>());
}