using Microsoft.Build.Utilities;
using SpiritReforged.Common.ItemCommon;
using SpiritReforged.Common.TileCommon;
using SpiritReforged.Common.TileCommon.PresetTiles;
using SpiritReforged.Content.Vanilla.Food;
using Terraria.Audio;
using Terraria.DataStructures;

namespace SpiritReforged.Content.Savanna.Items;

public class CampfireSpit : ModItem
{
	public override void SetStaticDefaults() => Item.ResearchUnlockCount = 25;

	public override void SetDefaults()
	{
		Item.width = Item.height = 14;
		Item.useAnimation = 15;
		Item.useTime = 10;
		Item.maxStack = Item.CommonMaxStack;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.useTurn = true;
		Item.autoReuse = true;
		Item.consumable = true;
		Item.value = Item.sellPrice(copper: 4);
	}

	public override void HoldItem(Player player)
	{
		if (CanPlace(player))
		{
			player.cursorItemIconEnabled = true;
			player.cursorItemIconID = Type;
		}
	}

	public override bool CanUseItem(Player player) => CanPlace(player);

	public override bool? UseItem(Player player)
	{
		if (Main.myPlayer == player.whoAmI && player.ItemAnimationJustStarted)
		{
			if (CanPlace(player))
			{
				int i = Player.tileTargetX;
				int j = Player.tileTargetY;

				TileExtensions.GetTopLeft(ref i, ref j);
				TileEntity.PlaceEntityNet(i, j, ModContent.TileEntityType<CampfireSlot>());

				return true;
			}
		}

		return null;
	}

	private bool CanPlace(Player player)
	{
		int i = Player.tileTargetX;
		int j = Player.tileTargetY;

		var target = Framing.GetTileSafely(i, j);

		if (target.HasTile && target.TileType == TileID.Campfire && player.IsTargetTileInItemRange(Item))
		{
			TileExtensions.GetTopLeft(ref i, ref j);
			return ModContent.GetInstance<CampfireSlot>().Find(i, j) == -1; //Invalid if this entity already exists
		}

		return false;
	}
}

public class CampfireSlot : SingleSlotEntity
{
	private const short cookCounterMax = 60 * 10;
	private short cookCounter;

	public override bool IsTileValidForEntity(int x, int y)
	{
		var t = Main.tile[x, y];
		return t.TileType == TileID.Campfire && TileObjectData.IsTopLeft(x, y);
	}

	public override bool CanAddItem(Item item) => RoastGlobalTile.AllowedTypes.Contains(item.type);

	public override void Update()
	{
		if (item.type == ModContent.ItemType<RawMeat>())
		{
			if (++cookCounter >= cookCounterMax)
			{
				item = new Item(ModContent.ItemType<CookedMeat>());
				cookCounter = 0;

				if (Main.netMode == NetmodeID.Server)
					new SingleSlotData((short)ID, item).Send();
			}
		}

		if (!IsTileValidForEntity(Position.X, Position.Y))
			Kill(Position.X, Position.Y);
	}
}

public class RoastGlobalTile : GlobalTile
{
	private static Asset<Texture2D> TileTexture;
	internal static HashSet<int> AllowedTypes;

	private static TileEntity Entity(int i, int j)
	{
		if (Main.tile[i, j].TileType != TileID.Campfire)
			return null;

		TileExtensions.GetTopLeft(ref i, ref j);
		int id = ModContent.GetInstance<CampfireSlot>().Find(i, j);

		return (id == -1) ? null : TileEntity.ByID[id];
	}

	private static bool IsTopHalf(int i, int j) => Main.tile[i, j].TileFrameY % (18 * 2) == 0;

	public override void Load() => TileTexture = Mod.Assets.Request<Texture2D>("Content/Savanna/Items/CampfireSpit_Tile");

	public override void SetStaticDefaults()
	{
		TileTexture = Mod.Assets.Request<Texture2D>("Content/Savanna/Items/CampfireSpit_Tile");
		AllowedTypes = [ModContent.ItemType<RawMeat>(), ModContent.ItemType<CookedMeat>()];
	}

	public override void MouseOver(int i, int j, int type)
	{
		if (Entity(i, j) is CampfireSlot slot && IsTopHalf(i, j))
		{
			int iconType = slot.item.IsAir ? ModContent.ItemType<CampfireSpit>() : slot.item.type;
			Main.LocalPlayer.cursorItemIconEnabled = true;
			Main.LocalPlayer.cursorItemIconID = iconType;
			Main.LocalPlayer.noThrow = 2;
		}
	}

	public override void RightClick(int i, int j, int type)
	{
		if (Entity(i, j) is CampfireSlot slot && IsTopHalf(i, j) && slot.OnInteract(Main.LocalPlayer))
		{
			Wiring.ToggleCampFire(i, j, Main.tile[i, j], null, true);
			SoundEngine.PlaySound(SoundID.Item1 with { Pitch = .25f }, new Vector2(i, j).ToWorldCoordinates());
		}
	}

	public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
	{
		if (!effectOnly && Entity(i, j) is CampfireSlot slot)
		{
			fail = true;
			TileExtensions.GetTopLeft(ref i, ref j);

			var pos = new Vector2(i, j).ToWorldCoordinates() + new Vector2(16);
			ItemMethods.NewItemSynced(new EntitySource_TileBreak(i, j), ModContent.ItemType<CampfireSpit>(), pos);

			if (!slot.item.IsAir)
				ItemMethods.NewItemSynced(new EntitySource_TileBreak(i, j), slot.item, pos);

			slot.Kill(i, j);
		}
	}

	public override bool PreDraw(int i, int j, int type, SpriteBatch spriteBatch)
	{
		if (TileObjectData.IsTopLeft(i, j) && Entity(i, j) is CampfireSlot slot)
		{
			var position = new Vector2(i, j) * 16 + new Vector2(Main.offScreenRange) - Main.screenPosition - new Vector2(0, 16);
			spriteBatch.Draw(TileTexture.Value, position, Lighting.GetColor(i + 1, j + 1));

			if (!slot.item.IsAir)
			{
				var itemTexture = TextureAssets.Item[slot.item.type];
				var source = itemTexture.Frame(1, 3, 0, 2);

				spriteBatch.Draw(itemTexture.Value, position + new Vector2(25, 12), source, Lighting.GetColor(i + 1, j + 1), 0, source.Size() / 2, 1, default, 0);
			}
		}

		return true;
	}
}