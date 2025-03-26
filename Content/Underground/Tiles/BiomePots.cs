using Terraria.DataStructures;

namespace SpiritReforged.Content.Underground.Tiles;

public class BiomePots : CavePots
{
	private enum STYLE : int
	{
		ICE, DESERT, JUNGLE, DUNGEON, CORRUPTION, CRIMSON, MARBLE, HELL
	}

	public override void PreAddObjectData()
	{
		const int row = 3;

		TileObjectData.newTile.StyleWrapLimit = row;
		TileObjectData.newTile.RandomStyleRange = row;
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