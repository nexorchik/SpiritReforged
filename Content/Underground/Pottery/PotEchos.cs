using SpiritReforged.Content.Underground.Tiles;

namespace SpiritReforged.Content.Underground.Pottery;

public class BiomePotsEcho : BiomePots
{
	public override string Texture => (typeof(BiomePots).Namespace + "." + typeof(BiomePots).Name).Replace('.', '/');

	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();
		Main.tileCut[Type] = false;
	}

	public override void KillMultiTile(int i, int j, int frameX, int frameY) { }
	public override void NearbyEffects(int i, int j, bool closer) { }
	public override bool KillSound(int i, int j, bool fail) => true;
	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) => true;
}