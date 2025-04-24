using SpiritReforged.Content.Underground.Moss.Radon;

namespace SpiritReforged.Content.Underground.Items.MossFlasks;

public class FlaskRadon : MossFlask { }

public class FlaskRadonProjectile : MossFlaskProjectile
{
	public override (ushort, ushort) Types => ((ushort)ModContent.TileType<RadonMoss>(), (ushort)ModContent.TileType<RadonMossGrayBrick>());
}