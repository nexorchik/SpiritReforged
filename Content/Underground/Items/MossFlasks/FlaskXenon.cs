namespace SpiritReforged.Content.Underground.Items.MossFlasks;

public class FlaskXenon : MossFlask { }

public class FlaskXenonProjectile : MossFlaskProjectile
{
	public override (ushort, ushort) Types => (TileID.XenonMoss, TileID.XenonMossBrick);
}