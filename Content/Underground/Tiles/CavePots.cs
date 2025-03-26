using Terraria.Audio;
using Terraria.DataStructures;

namespace SpiritReforged.Content.Underground.Tiles;

public class CavePots : ModTile
{
	private enum STYLE : int
	{
		PURITY, FANCY, ZENITH_BLUE, ZENITH_GREEN
	}

	public override void SetStaticDefaults()
	{
		const int row = 4; //Last space is empty for all rows except PURITY

		Main.tileSolid[Type] = false;
		Main.tileBlockLight[Type] = false;
		Main.tileCut[Type] = true;
		Main.tileFrameImportant[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
		TileObjectData.newTile.Origin = new(0, 1);
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop, TileObjectData.newTile.Width, 0);
		TileObjectData.newTile.StyleWrapLimit = row; //Would use RandomStyleRange if all rows had equal frames
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.UsesCustomCanPlace = true;
		TileObjectData.newTile.DrawYOffset = 2;
		PreAddObjectData();
		TileObjectData.addTile(Type);

		AddMapEntry(new Color(100, 90, 35), Language.GetText("MapObject.Pot"));
		DustType = -1;

		PotGlobalTile.PotTypes.Add(Type);
	}

	public virtual void PreAddObjectData() { }

	public override void KillMultiTile(int i, int j, int frameX, int frameY)
	{
		if (Main.netMode == NetmodeID.MultiplayerClient)
			return; //Drops should only occur on the server/singleplayer

		var style = (STYLE)(frameY / 36);

		switch (style)
		{
			case STYLE.PURITY:
				break;

			case STYLE.FANCY:
				break;

			case STYLE.ZENITH_BLUE:
				break;

			case STYLE.ZENITH_GREEN:
				break;
		}
	}

	public override bool KillSound(int i, int j, bool fail)
	{
		if (!fail)
		{
			SoundEngine.PlaySound(SoundID.Shatter, new Vector2(i, j).ToWorldCoordinates());
			return false;
		}

		return true;
	}
}