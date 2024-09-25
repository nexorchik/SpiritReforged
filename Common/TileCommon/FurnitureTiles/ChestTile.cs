using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;

namespace SpiritReforged.Common.TileCommon.FurnitureTiles;

public abstract class ChestTile : ModTile
{
	private LocalizedText MapEntry => Language.GetText($"Mods.SpiritReforged.Items.{Name}Item.DisplayName");

	private static void GetTopLeft(ref int i, ref int j)
	{
		Tile tile = Framing.GetTileSafely(i, j);
		(i, j) = (i - tile.TileFrameX % (18 * 2) / 18, j - tile.TileFrameY / 18);
	}

	public override void SetStaticDefaults()
	{
		Main.tileSpelunker[Type] = true;
		Main.tileContainer[Type] = true;
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileOreFinderPriority[Type] = 500;

		TileID.Sets.HasOutlines[Type] = true;
		TileID.Sets.BasicChest[Type] = true;
		TileID.Sets.DisableSmartCursor[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
		TileObjectData.newTile.Origin = new Point16(0, 1);
		TileObjectData.newTile.Height = 2;
		TileObjectData.newTile.CoordinateHeights = [16, 18];
		TileObjectData.newTile.HookCheckIfCanPlace = new PlacementHook(Chest.FindEmptyChest, -1, 0, true);
		TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(Chest.AfterPlacement_Hook, -1, 0, false);
		TileObjectData.newTile.AnchorInvalidTiles = [127];
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.LavaDeath = false;
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
		TileObjectData.addTile(Type);

		RegisterItemDrop(Mod.Find<ModItem>(Name + "Item").Type);
		AddMapEntry(new Color(100, 100, 60), MapEntry);
		AdjTiles = [TileID.Containers];
		DustType = -1;
	}

	public static string MapChestName(string name, int i, int j)
	{
		GetTopLeft(ref i, ref j);

		int chest = Chest.FindChest(i, j);
		if (chest < 0)
			return Language.GetTextValue("LegacyChestType.0");

		if (Main.chest[chest].name == string.Empty)
			return name;

		return name + ": " + Main.chest[chest].name;
	}

	public override LocalizedText DefaultContainerName(int frameX, int frameY) => MapEntry;

	public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

	public override void KillMultiTile(int i, int j, int frameX, int frameY) => Chest.DestroyChest(i, j);

	public override bool RightClick(int i, int j)
	{
		GetTopLeft(ref i, ref j);

		Player player = Main.LocalPlayer;
		Main.mouseRightRelease = false;

		player.CloseSign();
		player.SetTalkNPC(-1);
		Main.npcChatCornerItem = 0;
		Main.npcChatText = string.Empty;

		if (Main.editChest)
		{
			SoundEngine.PlaySound(SoundID.MenuTick);
			Main.editChest = false;
			Main.npcChatText = string.Empty;
		}

		if (player.editedChestName)
		{
			NetMessage.SendData(MessageID.SyncPlayerChest, -1, -1, NetworkText.FromLiteral(Main.chest[player.chest].name), player.chest, 1f);
			player.editedChestName = false;
		}

		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			if (i == player.chestX && j == player.chestY && player.chest >= 0)
			{
				player.chest = -1;
				Recipe.FindRecipes();
				SoundEngine.PlaySound(SoundID.MenuClose);
			}
			else
			{
				NetMessage.SendData(MessageID.RequestChestOpen, -1, -1, null, i, j);
				Main.stackSplit = 600;
			}
		}
		else
		{
			int chest = Chest.FindChest(i, j);
			if (chest >= 0)
			{
				Main.stackSplit = 600;
				if (chest == player.chest)
				{
					player.chest = -1;
					SoundEngine.PlaySound(SoundID.MenuClose);
				}
				else
				{
					SoundEngine.PlaySound(player.chest < 0 ? SoundID.MenuOpen : SoundID.MenuTick);
					player.OpenChest(i, j, chest);
				}

				Recipe.FindRecipes();
			}
		}

		return true;
	}

	public override void MouseOver(int i, int j)
	{
		GetTopLeft(ref i, ref j);

		Player player = Main.LocalPlayer;
		int chest = Chest.FindChest(i, j);
		player.cursorItemIconID = -1;

		if (chest < 0)
		{
			player.cursorItemIconText = Language.GetTextValue("LegacyChestType.0");
		}
		else
		{
			Tile tile = Framing.GetTileSafely(i, j);
			string defaultName = TileLoader.DefaultContainerName(tile.TileType, tile.TileFrameX, tile.TileFrameY);

			player.cursorItemIconText = Main.chest[chest].name.Length > 0 ? Main.chest[chest].name : defaultName;
			if (player.cursorItemIconText == defaultName)
			{
				player.cursorItemIconID = Mod.Find<ModItem>(Name + "Item").Type;
				player.cursorItemIconText = string.Empty;
			}
		}

		player.noThrow = 2;
		player.cursorItemIconEnabled = true;
	}

	public override void MouseOverFar(int i, int j)
	{
		MouseOver(i, j);

		Player player = Main.LocalPlayer;
		if (player.cursorItemIconText == string.Empty)
		{
			player.cursorItemIconEnabled = false;
			player.cursorItemIconID = 0;
		}
	}
}
