using Terraria.DataStructures;

namespace SpiritReforged.Content.Underground.Pots;

public class BiomePots : ModTile
{
	public enum STYLE : int
	{
		ICE, DESERT, JUNGLE, DUNGEON, CORRUPTION, CRIMSON, MARBLE, HELL
	}

	public override void SetStaticDefaults()
	{
		const int row = 3;

		Main.tileSolid[Type] = false;
		Main.tileBlockLight[Type] = false;
		Main.tileCut[Type] = true;
		Main.tileFrameImportant[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
		TileObjectData.newTile.Origin = new(0, 1);
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.AlternateTile, TileObjectData.newTile.Width, 0);
		TileObjectData.newTile.AnchorAlternateTiles = [Type];
		TileObjectData.newTile.StyleWrapLimit = row;
		TileObjectData.newTile.RandomStyleRange = row;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.UsesCustomCanPlace = true;
		TileObjectData.newTile.DrawYOffset = 2;
		TileObjectData.addTile(Type);

		AddMapEntry(new Color(100, 90, 35), Language.GetText("MapObject.Pot"));
		DustType = -1;

		PotGlobalTile.PotTypes.Add(Type);
	}

	public override void KillMultiTile(int i, int j, int frameX, int frameY)
	{
		if (Main.netMode == NetmodeID.MultiplayerClient)
			return; //Drops should only occur on the server/singleplayer

		var style = (STYLE)(frameY / 36);
		var source = new EntitySource_TileBreak(i, j);
		var position = new Vector2(i, j).ToWorldCoordinates(16, 16);

		switch (style)
		{
			case STYLE.ICE:
				break;

			case STYLE.DESERT:
				break;

			case STYLE.JUNGLE:
				break;

			case STYLE.DUNGEON:
				break;

			case STYLE.CORRUPTION:
				break;

			case STYLE.CRIMSON:
				break;

			case STYLE.MARBLE:
				break;

			case STYLE.HELL:
				break;
		}

		/*Gore.NewGore(source, position, Vector2.Zero, 51);
		Gore.NewGore(source, position, Vector2.Zero, 52);
		Gore.NewGore(source, position, Vector2.Zero, 53);*/
	}
}