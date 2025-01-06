using System.Linq;

namespace SpiritReforged.Common.NPCCommon;

internal class StockableShopPlayer : ModPlayer
{
	private bool _wasDayTime;

	public override void PostBuyItem(NPC vendor, Item[] shopInventory, Item item) => item.TryReduceStock();
	public override bool CanBuyItem(NPC vendor, Item[] shopInventory, Item item) => item.HasStock(out _);

	public override void PostUpdate()
	{
		if (Main.dayTime && !_wasDayTime)
			StockableShop.ResetAllStock();

		_wasDayTime = Main.dayTime;
	}
}

internal class StockableItem : GlobalItem
{
	public override bool InstancePerEntity => true;

	public bool stockable;

	public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
	{
		if (item.isAShopItem && !item.HasStock(out _))
		{
			spriteBatch.Draw(TextureAssets.Item[item.type].Value, position, frame, drawColor * .5f, 0, origin, scale, default, 0);
			return false;
		}

		return true;
	}

	public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
	{
		if (item.isAShopItem && item.HasStock(out int value) && value > 0)
			Utils.DrawBorderString(spriteBatch, value.ToString(), position - frame.Size() / 2, Main.MouseTextColorReal, Main.inventoryScale);
	}

	public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
	{
		if (item.isAShopItem && !item.HasStock(out _))
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

	public static void TryReduceStock(this Item item)
	{
		if (item.TryGetGlobalItem(out StockableItem sItem) && sItem.stockable)
		{
			int key = item.type;

			if (stockLookup.TryGetValue(key, out StockData stock))
				stockLookup[key].value = Math.Max(stock.value - 1, 0);
		}
	}

	public static void ResetAllStock()
	{
		foreach (int key in stockLookup.Keys)
			stockLookup[key].value = stockLookup[key].capacity;
	}

	public static bool HasStock(this Item item, out int value)
	{
		if (item.TryGetGlobalItem(out StockableItem sItem) && sItem.stockable)
		{
			int key = item.type;

			if (stockLookup.TryGetValue(key, out StockData stock))
			{
				value = stock.value;
				return stock.value > 0;
			}
		}

		value = 0;
		return true;
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
}
