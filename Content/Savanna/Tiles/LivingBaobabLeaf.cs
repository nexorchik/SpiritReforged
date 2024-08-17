namespace SpiritReforged.Content.Savanna.Tiles;

internal class LivingBaobabLeaf : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileBlendAll[Type] = true;
		Main.tileMergeDirt[Type] = true;
		Main.tileBlockLight[Type] = true;

		AddMapEntry(new Color(140, 156, 55));
		HitSound = SoundID.Dig;
	}
}
