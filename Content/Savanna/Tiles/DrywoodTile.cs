namespace SpiritReforged.Content.Savanna.Tiles;

public class DrywoodTile : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileBlockLight[Type] = true;
		Main.tileBrick[Type] = true;
		Main.tileMergeDirt[Type] = true;

		DustType = DustID.t_PearlWood;
		AddMapEntry(new Color(145, 128, 109));
	}
}