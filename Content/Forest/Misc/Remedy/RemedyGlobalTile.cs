using Terraria.DataStructures;

namespace SpiritReforged.Content.Forest.Misc.Remedy;

/// <summary> Drops <see cref="RemedyPotion"/>s from all pots in addition to normal items. Odds vary by depth. </summary>
internal class RemedyGlobalTile : GlobalTile
{
	public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
	{
		if (!fail && !effectOnly && type == TileID.Pots && IsTopLeft() && Main.netMode != NetmodeID.MultiplayerClient)
		{
			int chance = (i >= Main.UnderworldLayer) ? 33 : ((i >= Main.rockLayer) ? 38 : ((i >= Main.worldSurface) ? 31 : 0));

			if (chance > 0 && Main.rand.NextBool(chance))
				Item.NewItem(new EntitySource_TileBreak(i, j), new Rectangle(i * 16, j * 16, 32, 32), ModContent.ItemType<RemedyPotion>());
		}

		bool IsTopLeft()
		{
			var tile = Main.tile[i, j];
			return tile.TileFrameX % 36 == 0 && tile.TileFrameY % 36 == 0;
		}
	}
}
