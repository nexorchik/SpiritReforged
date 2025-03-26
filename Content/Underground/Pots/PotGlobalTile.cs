using SpiritReforged.Common.ItemCommon;
using Terraria.DataStructures;

namespace SpiritReforged.Content.Underground.Pots;

/// <summary> Register common pot types in <see cref="PotTypes"/>. </summary>
internal class PotGlobalTile : GlobalTile
{
	public static readonly HashSet<int> PotTypes = [TileID.Pots];

	/// <summary> Drops Ceramic Shards at the given position if the nearest player is holding <see cref="CeramicGuide"/>. </summary>
	public static void DropShards(IEntitySource source, Vector2 position)
	{
		var p = Main.player[Player.FindClosest(position, 16, 16)];

		if (!p.GetModPlayer<CeramicPreservationPlayer>().guideActive)
			return;

		int type = ItemID.DirtBlock; //TEMP
		int stack = Main.rand.Next(1, 3);

		if (Main.netMode == NetmodeID.MultiplayerClient)
			ItemMethods.NewItemSynced(source, new Item(type, stack), position, true);
		else
			Item.NewItem(source, position, type, stack, noGrabDelay: true);
	}

	public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
	{
		if (effectOnly || fail || noItem || Main.netMode == NetmodeID.MultiplayerClient || !PotTypes.Contains(type) || !IsTopLeft())
			return;

		DropShards(new EntitySource_TileBreak(i, j), new Vector2(i, j).ToWorldCoordinates(16, 16));

		bool IsTopLeft()
		{
			var tile = Main.tile[i, j];
			return tile.TileFrameX % 36 == 0 && tile.TileFrameY % 36 == 0;
		}
	}
}