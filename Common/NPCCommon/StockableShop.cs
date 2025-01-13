using SpiritReforged.Common.Misc;
using System.Linq;

namespace SpiritReforged.Common.NPCCommon;

internal class StockableShopPlayer : ModPlayer
{
	public override void Load() => TimeUtils.JustTurnedDay += Reset;
	
	private void Reset() => StockableShop.ResetAllStock();

	public override void PostBuyItem(NPC vendor, Item[] shopInventory, Item item)
	{
		if (item.TryGetGlobalItem(out StockableItem sItem) && sItem.stockable)
			item.TryReduceStock();
	}

	public override bool CanBuyItem(NPC vendor, Item[] shopInventory, Item item)
	{
		if (item.TryGetGlobalItem(out StockableItem sItem) && sItem.stockable)
			return item.GetStock() != 0;

		return true;
	}
}

internal class StockableItem : GlobalItem
{
	public override bool InstancePerEntity => true;

	public bool stockable;

	public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
	{
		if (item.isAShopItem && stockable && item.GetStock() == 0)
		{
			spriteBatch.Draw(TextureAssets.Item[item.type].Value, position, frame, drawColor * .5f, 0, origin, scale, default, 0);
			return false;
		}

		return true;
	}

	public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
	{
		if (item.isAShopItem && stockable)
		{
			int value = item.GetStock();

			if (value > 0)
				Utils.DrawBorderString(spriteBatch, value.ToString(), position - frame.Size() / 2, Main.MouseTextColorReal, Main.inventoryScale);
		}
	}

	public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
	{
		if (item.isAShopItem && stockable && item.GetStock() == 0)
		{
			var shopTip = tooltips.Where(x => x.Name == "Price").FirstOrDefault();

			if (shopTip != default)
			{
				shopTip.Text = Language.GetTextValue("Mods.SpiritReforged.Misc.NoStock");
				shopTip.OverrideColor = Color.Gray with { A = Main.mouseTextColor };
			}
		}
	}
}

internal static class StockableShop
{
	public class StockData
	{
		public StockData(int value) => this.value = capacity = value;

		public int value;
		public int capacity;
	}

	private static readonly Dictionary<int, StockData> stockLookup = []; //item type, stock

	/// <summary> Attempts to reduce this stockable item's stock by one. </summary>
	public static void TryReduceStock(this Item item)
	{
		int key = item.type;

		if (stockLookup.TryGetValue(key, out StockData stock))
			stockLookup[key].value = Math.Max(stock.value - 1, 0);
	}

	/// <summary> Resets all item stocks to capacity. </summary>
	public static void ResetAllStock()
	{
		foreach (int key in stockLookup.Keys)
			stockLookup[key].value = stockLookup[key].capacity;
	}

	/// <summary> Attempts to get this item's stock value. </summary>
	public static int GetStock(this Item item)
	{
		int key = item.type;

		if (stockLookup.TryGetValue(key, out StockData stock))
			return stock.value;

		return 0;
	}

	public static NPCShop AddLimited(this NPCShop shop, int itemType, int stock, params Condition[] condition)
	{
		var item = new Item(itemType);

		if (item.TryGetGlobalItem(out StockableItem sItem))
		{
			sItem.stockable = true;
			stockLookup.Add(itemType, new StockData(stock));
		}

		return shop.Add(item, condition);
	}

	public static NPCShop AddLimited<T>(this NPCShop shop, int stock, params Condition[] condition) where T : ModItem
	{
		int itemType = ModContent.ItemType<T>();
		var item = new Item(itemType);

		if (item.TryGetGlobalItem(out StockableItem sItem))
		{
			sItem.stockable = true;
			stockLookup.Add(itemType, new StockData(stock));
		}

		return shop.Add(item, condition);
	}
}
