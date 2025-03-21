namespace SpiritReforged.Content.Savanna.Tiles;

internal class LivingBaobabLeaf : ModTile
{
	public override void SetStaticDefaults()
	{
		TileID.Sets.IsSkippedForNPCSpawningGroundTypeCheck[Type] = true;

		Main.tileSolid[Type] = true;
		Main.tileBlockLight[Type] = true;

		AddMapEntry(new Color(140, 156, 55));
		DustType = DustID.JunglePlants;
		HitSound = SoundID.Grass;
	}

	public override void RandomUpdate(int i, int j)
	{
		if (Main.rand.NextBool(50)) //Randomly grow hanging baobab fruit
			HangingBaobabFruit.GrowVine(i, ++j);
	}
}
