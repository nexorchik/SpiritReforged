using Mono.Cecil;
using RubbleAutoloader;
using SpiritReforged.Content.Underground.Items;
using Terraria.Audio;
using Terraria.DataStructures;

namespace SpiritReforged.Content.Underground.Tiles;

public class CavePots : ModTile, IAutoloadRubble
{
	public virtual IAutoloadRubble.RubbleData Data => new(ModContent.ItemType<CeramicShard>(), IAutoloadRubble.RubbleSize.Medium, [0, 1, 2, 3, 4, 5, 6, 8, 9, 10, 12, 13, 14]);

	public enum STYLE : int
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
		if (Main.netMode == NetmodeID.MultiplayerClient || Autoloader.IsRubble(Type))
			return; //Drops should only occur on the server/singleplayer

		var style = (STYLE)(frameY / 36);
		var source = new EntitySource_TileBreak(i, j);
		var position = new Vector2(i, j).ToWorldCoordinates(16, 16);

		switch (style)
		{
			case STYLE.PURITY:

				Gore.NewGore(source, position, Vector2.Zero, 51);
				Gore.NewGore(source, position, Vector2.Zero, 52);
				Gore.NewGore(source, position, Vector2.Zero, 53);

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
		if (!fail && !Autoloader.IsRubble(Type))
		{
			SoundEngine.PlaySound(SoundID.Shatter, new Vector2(i, j).ToWorldCoordinates());
			return false;
		}

		return true;
	}
}