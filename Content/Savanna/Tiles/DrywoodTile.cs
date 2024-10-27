namespace SpiritReforged.Content.Savanna.Tiles;

public class DrywoodTile : ModTile
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

		Main.tileMerge[Type][ModContent.TileType<SavannaDirt>()] = true;
		Main.tileMerge[ModContent.TileType<SavannaDirt>()][Type] = true;

		AddMapEntry(new Color(145, 128, 109));
	}
}