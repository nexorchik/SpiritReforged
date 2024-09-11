using Terraria.DataStructures;

namespace SpiritReforged.Content.Ocean.Tiles;

public class BigBuoy : Buoy
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileSolid[Type] = false;
		Main.tileNoAttach[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
		TileObjectData.newTile.Height = 3;
		TileObjectData.newTile.CoordinateHeights = [16, 16, 18];
		TileObjectData.newTile.Origin = new Point16(0, 2);
		TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
		TileObjectData.addTile(Type);

		DustType = DustID.Iron;
		HitSound = SoundID.Dig;

		AddMapEntry(new Color(250, 67, 74));
	}
}