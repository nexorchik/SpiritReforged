using SpiritReforged.Common.TileCommon;
using Terraria.DataStructures;

namespace SpiritReforged.Content.Savanna.Tiles;

public class SparrowCage : ModTile, IAutoloadTileItem
{
	private const int FrameHeight = 54;

	void IAutoloadTileItem.SetItemDefaults(ModItem item) => item.Item.SetShopValues(ItemRarityColor.Blue1, Item.buyPrice(0, 4, 0, 0));

	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;
		Main.tileSolidTop[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
		TileObjectData.newTile.Origin = new Point16(3, 2);
		TileObjectData.newTile.Width = 6;
		TileObjectData.newTile.CoordinateHeights = [16, 16, 16];
		TileObjectData.newTile.DrawYOffset = 2;
		TileObjectData.addTile(Type);

		DustType = DustID.Glass;
		AnimationFrameHeight = FrameHeight;
		AdjTiles = [TileID.BirdCage];

		RegisterItemDrop(Mod.Find<ModItem>("SparrowCageItem").Type);
	}

	public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

	public override void AnimateTile(ref int frame, ref int frameCounter)
	{
		const int numFrames = 35;

		if (++frameCounter >= 10)
		{
			frameCounter = 0;

			if (frame is 0 or 6 or 11 or 19 && !Main.rand.NextBool(5))
				return;

			frame = ++frame % numFrames;
		}
	}

	public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
	{
		/*var tile = Main.tile[i, j];
		if (tile.TileFrameY >= FrameHeight)
			frameYOffset = Main.tileFrame[type] * FrameHeight;
		else
			frameYOffset = 0; //Don't animate when turned off*/
	}
}
