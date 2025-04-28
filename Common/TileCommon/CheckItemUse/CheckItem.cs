using Terraria.DataStructures;

namespace SpiritReforged.Common.TileCommon.CheckItemUse;

internal class CheckItem : GlobalItem
{
	public delegate bool? ItemUseDelegate(int itemType, int i, int j);
	private static readonly Dictionary<int, ItemUseDelegate> TileToAction = [];

	public static void RegisterTileCheck(int tileType, ItemUseDelegate dele) => TileToAction.Add(tileType, dele);

	public override void SetStaticDefaults()
	{
		foreach (var tile in Mod.GetContent<ModTile>())
		{
			if (tile is ICheckItemUse check)
				RegisterTileCheck(tile.Type, check.CheckItemUse);
		}
	}

	public override bool? UseItem(Item item, Player player)
	{
		var target = new Point(Player.tileTargetX, Player.tileTargetY);
		var tile = Main.tile[target.X, target.Y];

		if (tile.HasTile && player.InInteractionRange(target.X, target.Y, TileReachCheckSettings.Simple) && TileToAction.TryGetValue(tile.TileType, out var check))
			return check.Invoke(item.type, target.X, target.Y);

		return null;
	}
}