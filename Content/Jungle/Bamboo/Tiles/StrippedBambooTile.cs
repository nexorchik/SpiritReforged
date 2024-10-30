namespace SpiritReforged.Content.Jungle.Bamboo.Tiles;

public class StrippedBambooTile : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileMergeDirt[Type] = true;
		Main.tileBlockLight[Type] = true;

		Main.tileMerge[Type][TileID.WoodBlock] = true;
		Main.tileMerge[TileID.WoodBlock][Type] = true;

		Main.tileMerge[Type][TileID.Sand] = true;
		Main.tileMerge[TileID.Sand][Type] = true;

		DustType = DustID.PalmWood;
		AddMapEntry(new Color(145, 128, 109));
	}
}