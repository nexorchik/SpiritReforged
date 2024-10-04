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
		DustType = -1;
	}

	public override void RandomUpdate(int i, int j)
	{
		//Randomly grow hanging baobab fruit
		if (Main.rand.NextBool(50) && !Framing.GetTileSafely(i, j + 1).HasTile && !Framing.GetTileSafely(i, j + 2).HasTile)
			WorldGen.PlaceObject(i, j + 1, ModContent.TileType<Items.BaobabFruit.BaobabFruitTile>(), true);
	}
}
