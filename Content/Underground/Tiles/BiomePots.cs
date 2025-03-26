using RubbleAutoloader;
using SpiritReforged.Content.Underground.Items;
using Terraria.DataStructures;

namespace SpiritReforged.Content.Underground.Tiles;

public class BiomePots : CavePots
{
	public override IAutoloadRubble.RubbleData Data => new(ModContent.ItemType<CeramicShard>(), IAutoloadRubble.RubbleSize.Medium);

	public new enum STYLE : int
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
		if (Main.netMode == NetmodeID.MultiplayerClient || Autoloader.IsRubble(Type))
			return; //Drops should only occur on the server/singleplayer

		var style = (STYLE)(frameY / 36);
		var source = new EntitySource_TileBreak(i, j);
		var position = new Vector2(i, j).ToWorldCoordinates(16, 16);

		switch (style)
		{
			case STYLE.ICE:

				Gore.NewGore(source, position, Vector2.Zero, 51);
				Gore.NewGore(source, position, Vector2.Zero, 52);
				Gore.NewGore(source, position, Vector2.Zero, 53);

				break;

			case STYLE.DESERT:

				Gore.NewGore(source, position, Vector2.Zero, GoreID.DesertPot1);
				Gore.NewGore(source, position, Vector2.Zero, GoreID.DesertPot2);
				Gore.NewGore(source, position, Vector2.Zero, GoreID.DesertPot3);

				break;

			case STYLE.JUNGLE:

				Gore.NewGore(source, position, Vector2.Zero, 166);
				Gore.NewGore(source, position, Vector2.Zero, 167);
				Gore.NewGore(source, position, Vector2.Zero, 168);

				break;

			case STYLE.DUNGEON:

				Gore.NewGore(source, position, Vector2.Zero, 169);
				Gore.NewGore(source, position, Vector2.Zero, 170);
				Gore.NewGore(source, position, Vector2.Zero, 171);

				break;

			case STYLE.CORRUPTION:

				Gore.NewGore(source, position, Vector2.Zero, 172);
				Gore.NewGore(source, position, Vector2.Zero, 173);
				Gore.NewGore(source, position, Vector2.Zero, 174);

				break;

			case STYLE.CRIMSON:

				Gore.NewGore(source, position, Vector2.Zero, 175);
				Gore.NewGore(source, position, Vector2.Zero, 176);
				Gore.NewGore(source, position, Vector2.Zero, 177);

				break;

			case STYLE.MARBLE:

				Gore.NewGore(source, position, Vector2.Zero, GoreID.GreekPot1);
				Gore.NewGore(source, position, Vector2.Zero, GoreID.GreekPot2);
				Gore.NewGore(source, position, Vector2.Zero, GoreID.GreekPot3);

				break;

			case STYLE.HELL:

				Gore.NewGore(source, position, Vector2.Zero, 178);
				Gore.NewGore(source, position, Vector2.Zero, 179);
				Gore.NewGore(source, position, Vector2.Zero, 180);

				break;
		}

		/*Gore.NewGore(source, position, Vector2.Zero, 51);
		Gore.NewGore(source, position, Vector2.Zero, 52);
		Gore.NewGore(source, position, Vector2.Zero, 53);*/
	}
}