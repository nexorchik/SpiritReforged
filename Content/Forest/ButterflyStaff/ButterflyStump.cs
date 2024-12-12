using SpiritReforged.Common.TileCommon;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;

namespace SpiritReforged.Content.Forest.ButterflyStaff;

public class ButterflyStump : ModTile
{
	private const int FrameHeight = 18 * 4;

	private static int ItemType => ModContent.ItemType<ButterflyStaff>();
	private static bool HasItem(int i, int j) => Framing.GetTileSafely(i, j).TileFrameY < FrameHeight;

	public override void SetStaticDefaults()
	{
		Main.tileLighted[Type] = true;
		Main.tileFrameImportant[Type] = true;
		Main.tileWaterDeath[Type] = true;
		Main.tileLavaDeath[Type] = true;

		TileID.Sets.HasOutlines[Type] = true;
		TileID.Sets.DisableSmartCursor[Type] = true;
		TileID.Sets.InteractibleByNPCs[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
		TileObjectData.newTile.Origin = new Point16(1, 3);
		TileObjectData.newTile.Height = 4;
		TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16];
		TileObjectData.newTile.DrawYOffset = 2;
		TileObjectData.addTile(Type);

		AddMapEntry(new Color(212, 125, 93));
		RegisterItemDrop(ItemType);

		AnimationFrameHeight = FrameHeight;
		DustType = DustID.WoodFurniture;
	}

	public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

	public override void MouseOver(int i, int j)
	{
		if (!HasItem(i, j))
			return;

		Player player = Main.LocalPlayer;
		player.noThrow = 2;
		player.cursorItemIconEnabled = true;
		player.cursorItemIconID = ItemType;
	}

	public override bool RightClick(int i, int j)
	{
		if (HasItem(i, j))
		{
			TileExtensions.GetTopLeft(ref i, ref j);

			for (int x = i; x < i + 2; x++)
				for (int y = j; y < j + 4; y++)
					Main.tile[x, y].TileFrameY += FrameHeight;

			if (Main.netMode != NetmodeID.SinglePlayer)
				NetMessage.SendTileSquare(-1, i, j, 2, 4);

			int item = Item.NewItem(null, new Vector2(i + 1, j) * 16, ItemType);
			if (Main.netMode != NetmodeID.SinglePlayer)
				NetMessage.SendData(MessageID.SyncItem, number: item);

			return true;
		}

		return false;
	}

	public override bool CanDrop(int i, int j) => HasItem(i, j);

	public override void AnimateTile(ref int frame, ref int frameCounter)
	{
		if (++frameCounter >= 4)
		{
			frameCounter = 0;
			frame = ++frame % 8;
		}
	}

	public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
	{
		if (HasItem(i, j))
			frameYOffset = Main.tileFrame[type] * FrameHeight;
		else
			frameYOffset = FrameHeight * 7; //Don't animate
	}

	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
	{
		if (!HasItem(i, j))
			return;

		var color = Color.Magenta * .005f;
		(r, g, b) = (color.R, color.G, color.B);
	}
}