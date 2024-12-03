using SpiritReforged.Common.NPCCommon;
using SpiritReforged.Common.PlayerCommon;
using SpiritReforged.Common.TileCommon;
using SpiritReforged.Content.Savanna.Biome;
using Terraria.GameContent.ObjectInteractions;

namespace SpiritReforged.Content.Savanna.Tiles;

public class SavannaFountain : ModTile, IAutoloadTileItem
{
	void IAutoloadTileItem.SetItemDefaults(ModItem item) => item.Item.SetShopValues(ItemRarityColor.Blue1, Item.buyPrice(0, 4, 0, 0));

	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;

		TileID.Sets.InteractibleByNPCs[Type] = true;
		TileID.Sets.HasOutlines[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
		TileObjectData.newTile.Height = 4;
		TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16];
		TileObjectData.newTile.DrawYOffset = 2;
		TileObjectData.addTile(Type);

		DustType = DustID.Stone;

		int itemId = Mod.Find<ModItem>("SavannaFountainItem").Type;
		RegisterItemDrop(itemId);
		NPCShopHelper.AddEntry(new NPCShopHelper.ConditionalEntry(shop => shop.NpcType == NPCID.WitchDoctor, new NPCShop.Entry(itemId)));
	}

	public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
	public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;
	public override void AnimateTile(ref int frame, ref int frameCounter) => frame = Main.tileFrame[TileID.WaterFountain];

	public override void NearbyEffects(int i, int j, bool closer)
	{
		if (Main.tile[i, j].TileFrameY >= 72)
			Main.LocalPlayer.GetModPlayer<FountainPlayer>().SetFountain<SavannaWaterStyle>();
	}

	public override bool RightClick(int i, int j)
	{
		ToggleTile(i, j);
		return true;
	}

	public override void HitWire(int i, int j) => ToggleTile(i, j);

	private void ToggleTile(int i, int j)
	{
		Tile anchor = Framing.GetTileSafely(i, j);
		int x = i - anchor.TileFrameX / 18 % 2;
		int y = j - anchor.TileFrameY / 18 % 4;

		for (int l = x; l < x + 2; l++)
		{
			for (int m = y; m < y + 4; m++)
			{
				Tile tile = Framing.GetTileSafely(l, m);

				if (tile.HasTile && tile.TileType == Type)
				{
					if (tile.TileFrameY < 72)
						tile.TileFrameY += 72;
					else
						tile.TileFrameY -= 72;
				}

				Wiring.SkipWire(l, m);
			}
		}

		if (Main.netMode != NetmodeID.SinglePlayer)
			NetMessage.SendTileSquare(-1, x, y, 2, 4);
	}
}
