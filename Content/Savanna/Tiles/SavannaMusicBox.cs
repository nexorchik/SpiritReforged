using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.Utilities;

namespace SpiritReforged.Content.Savanna.Tiles;

public class SavannaMusicBox : ModTile
{
	private static int ItemType => ModContent.ItemType<SavannaMusicBoxItem>();

	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileObsidianKill[Type] = true;

		TileID.Sets.DisableSmartCursor[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
		TileObjectData.newTile.CoordinateHeights = [16, 18];
		TileObjectData.newTile.Origin = new Point16(0, 1);
		TileObjectData.newTile.LavaDeath = false;
		TileObjectData.addTile(Type);

		RegisterItemDrop(ItemType); //Confirm an item drop for all styles
		AddMapEntry(new Color(200, 200, 200), Language.GetText("ItemName.MusicBox"));
		DustType = -1;
	}

	public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData) //Spawn music notes
	{
		bool chance = (int)Main.timeForVisualEffects % 7 == 0 && Main.rand.NextBool(3);
		if (Lighting.UpdateEveryFrame && new FastRandom(Main.TileFrameSeed).WithModifier(i, j).Next(4) != 0 || !chance)
			return;

		var tile = Framing.GetTileSafely(i, j);
		if (!TileDrawing.IsVisible(tile) || tile.TileFrameX != 36 || tile.TileFrameY % 36 != 0)
			return;

		int goreType = Main.rand.Next(570, 573);
		var position = new Vector2(i, j) * 16 + new Vector2(8, -8);

		static float Random() => Main.rand.NextFloat(.5f, 1.5f);
		var velocity = new Vector2(Main.WindForVisuals * 2f, -0.5f) * new Vector2(Random(), Random());

		var gore = Gore.NewGoreDirect(new EntitySource_TileUpdate(i, j), position, velocity, goreType, .8f);
		gore.position.X -= gore.Width / 2;
	}

	public override void MouseOver(int i, int j)
	{
		Player player = Main.LocalPlayer;
		player.noThrow = 2;
		player.cursorItemIconEnabled = true;
		player.cursorItemIconID = ItemType;
	}
}

public class SavannaMusicBoxItem : ModItem
{
	private static int TileType => ModContent.TileType<SavannaMusicBox>();

	public override void SetStaticDefaults()
	{
		MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Assets/Music/Savanna"), Type, TileType);

		ItemID.Sets.CanGetPrefixes[Type] = false;
		ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;
	}

	public override void SetDefaults() => Item.DefaultToMusicBox(TileType, 0);
}
