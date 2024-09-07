using SpiritReforged.Common.TileCommon;
using System.Linq;
using Terraria.DataStructures;

internal class StaffOfRegrowthItem : GlobalItem
{
	public override bool? UseItem(Item item, Player player)
	{
		if (item.type == ItemID.StaffofRegrowth)
		{
			var target = new Point(Player.tileTargetX, Player.tileTargetY);
			Tile tile = Main.tile[target.X, target.Y];

			if (tile.HasTile && player.InInteractionRange(target.X, target.Y, TileReachCheckSettings.Simple))
			{
				int type = TagGlobalTile.HarvestableHerbs.FirstOrDefault(x => x == tile.TileType);

				if (type != default)
				{
					var herb = ModContent.GetModTile(type) as HerbTile;

					if (herb.CanBeHarvested(target.X, target.Y))
					{
						WorldGen.KillTile(target.X, target.Y);
						return true;
					}
				}
			}
		}

		return null;
	}
}
