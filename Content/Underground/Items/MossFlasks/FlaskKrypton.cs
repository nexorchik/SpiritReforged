namespace SpiritReforged.Content.Underground.Items.MossFlasks;

public class FlaskKrypton : MossFlask { }

public class FlaskKryptonProjectile : MossFlaskProjectile
{
	public override (ushort, ushort) Types => (TileID.KryptonMoss, TileID.KryptonMossBrick);
}