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
}
