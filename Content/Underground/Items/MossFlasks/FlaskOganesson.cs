using SpiritReforged.Content.Underground.Moss.Oganesson;

namespace SpiritReforged.Content.Underground.Items.MossFlasks;

public class FlaskOganesson : MossFlask { }

public class FlaskOganessonProjectile : MossFlaskProjectile
{
	public override (ushort, ushort) Types => ((ushort)ModContent.TileType<OganessonMoss>(), (ushort)ModContent.TileType<OganessonMossGrayBrick>());
}