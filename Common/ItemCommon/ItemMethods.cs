using Terraria.DataStructures;

namespace SpiritReforged.Common.ItemCommon;

internal static class ItemMethods
{
	/// <summary> Spawns an item and automatically syncs it for the multiplayer client. </summary>
	public static void NewItemSynced(IEntitySource source, Item item, Vector2 position, bool noGrabDelay = false)
	{
		int id = Item.NewItem(source, position, item, noGrabDelay: noGrabDelay);

		if (Main.netMode == NetmodeID.MultiplayerClient)
			NetMessage.SendData(MessageID.SyncItem, number: id, number2: noGrabDelay ? 1 : 0);
	}

	/// <inheritdoc cref="NewItemSynced(IEntitySource, Item, Vector2, bool)"/>
	public static void NewItemSynced(IEntitySource source, int itemType, Vector2 position, bool noGrabDelay = false) => NewItemSynced(source, new Item(itemType), position, noGrabDelay);

	public delegate void CoinAction(int type, int stack);

	/// <summary> Splits the given coin value randomly into item stacks. </summary>
	public static void SplitCoins(int fullValue, CoinAction action, int stacks = 0)
	{
		int[] split = Utils.CoinsSplit(fullValue);

		for (int c = 0; c < split.Length; c++)
		{
			if (split[c] == 0)
				continue;

			int type = c switch
			{
				3 => ItemID.PlatinumCoin,
				2 => ItemID.GoldCoin,
				1 => ItemID.SilverCoin,
				_ => ItemID.CopperCoin,
			};

			int numStacks = (stacks < 1) ? Math.Min(Main.rand.Next(3) + 1, split[c]) : stacks;
			for (int r = 0; r < numStacks; r++)
				action?.Invoke(type, split[c] / numStacks);
		}
	}

	/// <summary> Attempts to find the autoloaded item associated with this type. Throws an exception on failure.<para/>
	/// Should be used in tandem with <see cref="TileCommon.IAutoloadTileItem"/>, <see cref="WallCommon.IAutoloadWallItem"/> and other autoloader utils. </summary>
	public static ModItem AutoModItem<T>(this T mt) where T : ModType => SpiritReforgedMod.Instance.Find<ModItem>(mt.Name + "Item");

	/// <inheritdoc cref="AutoModItem{T}(T)"/> //Improves readability slightly
	public static Item AutoItem<T>(this T mt) where T : ModType => mt.AutoModItem().Item;

	/// <summary> Attempts to find the autoloaded item type associated with the given Type. Throws an exception on failure. </summary>
	public static int AutoItemType<T>() where T : ModType
	{
		var mod = SpiritReforgedMod.Instance;
		return mod.Find<ModItem>(ModContent.GetInstance<T>().Name + "Item").Type;
	}
}
