using System.Linq;

namespace SpiritReforged.Content.Forest.Misc.TornMap;

public class TornMapWall : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;

		TileID.Sets.FramesOnKillWall[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
		TileObjectData.newTile.Height = 2;
		TileObjectData.newTile.CoordinateHeights = Enumerable.Repeat(16, 2).ToArray();
		TileObjectData.newTile.RandomStyleRange = 3;
		TileObjectData.addTile(Type);

		AddMapEntry(new Color(23, 23, 23), Language.GetText("MapObject.Painting"));
		RegisterItemDrop(ModContent.ItemType<TornMapPiece>());
		DustType = -1;
	}
}

public class TornMapWallSmall : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;

		TileID.Sets.FramesOnKillWall[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
		TileObjectData.newTile.Width = 2;
		TileObjectData.newTile.Height = 2;
		TileObjectData.newTile.CoordinateHeights = Enumerable.Repeat(16, 2).ToArray();
		TileObjectData.addTile(Type);

		AddMapEntry(new Color(23, 23, 23), Language.GetText("MapObject.Painting"));
		RegisterItemDrop(ModContent.ItemType<TornMapPiece>());
		DustType = -1;
	}
}