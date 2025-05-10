using SpiritReforged.Content.Dusts;
using SpiritReforged.Content.Underground.Moss.Oganesson;

namespace SpiritReforged.Content.Underground.Items.MossFlasks;

public class FlaskOganesson : MossFlask
{
	public override void AddRecipes() => CreateRecipe().AddIngredient(ModContent.ItemType<OganessonMossItem>(), 3).AddIngredient(ItemID.Bottle).Register();
}

public class FlaskOganessonProjectile : MossFlaskProjectile
{
	public override MossConversion Conversion => new((ushort)ModContent.TileType<OganessonMoss>(), (ushort)ModContent.TileType<OganessonMossGrayBrick>(), (ushort)ModContent.TileType<OganessonPlants>());
	public override void CreateDust(int type) => base.CreateDust(ModContent.DustType<OganessonMossDust>());
}