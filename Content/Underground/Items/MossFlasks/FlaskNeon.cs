namespace SpiritReforged.Content.Underground.Items.MossFlasks;

public class FlaskNeon : MossFlask { }

public class FlaskNeonProjectile : MossFlaskProjectile
{
	public override (ushort, ushort) Types => (TileID.VioletMoss, TileID.VioletMossBrick);
}