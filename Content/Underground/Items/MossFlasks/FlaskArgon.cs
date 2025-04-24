namespace SpiritReforged.Content.Underground.Items.MossFlasks;

public class FlaskArgon : MossFlask { }

public class FlaskArgonProjectile : MossFlaskProjectile
{
	public override (ushort, ushort) Types => (TileID.ArgonMoss, TileID.ArgonMossBrick);
}