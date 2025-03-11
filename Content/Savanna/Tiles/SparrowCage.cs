using SpiritReforged.Common.TileCommon;

namespace SpiritReforged.Content.Savanna.Tiles;

public class SparrowCage : ModTile, IAutoloadTileItem
{
	private const int FrameHeight = 54;

	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;
		Main.tileSolidTop[Type] = true;
		Main.tileTable[Type] = true;

		TileID.Sets.CritterCageLidStyle[Type] = 0;

		TileObjectData.newTile.CopyFrom(TileObjectData.GetTileData(TileID.BirdCage, 0));
		TileObjectData.addTile(Type);

		DustType = DustID.Glass;
		AnimationFrameHeight = FrameHeight;
		AdjTiles = [TileID.BirdCage];

		RegisterItemDrop(Mod.Find<ModItem>("SparrowCageItem").Type);
	}

	public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) => offsetY = 2;
	public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

	public override void AnimateTile(ref int frame, ref int frameCounter)
	{
		const int numFrames = 35;

		if (++frameCounter >= 10)
		{
			frameCounter = 0;

			if (frame is 0 or 6 or 11 or 19 && !Main.rand.NextBool(12))
				return;

			frame = ++frame % numFrames;
		}
	}
}
