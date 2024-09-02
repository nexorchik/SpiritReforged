using SpiritReforged.Common.PlayerCommon.FlowerBootEffects;

namespace SpiritReforged.Common.PlayerCommon;

internal class FlowerBootsPlayer : ModPlayer
{
	public override void PostUpdateEquips()
	{
		if (Player.flowerBoots && Player.whoAmI == Main.myPlayer)
		{
			Player.DoBootsEffect(CheckAllEffects);
		}
	}

	private bool CheckAllEffects(int x, int y)
	{
		foreach (var item in ModContent.GetContent<FlowerBootEffect>())
		{
			Tile tile = Main.tile[x, y + 1];
			Tile current = Main.tile[x, y];
			bool? overrideCheck = item.RunManually(x, y, Player);

			if (overrideCheck is true or null)
			{
				if (overrideCheck is true)
					return false;

				if (overrideCheck is null)
					continue;
			}

			if (tile.HasTile && item.CanPlaceOn(x, y + 1, Player) && !current.HasTile && current.LiquidAmount == 0)
			{
				if (item.PlaceOn(x, y, Player))
				{
					tile.CopyPaintAndCoating(Main.tile[x, y + 1]);

					if (Main.netMode == NetmodeID.MultiplayerClient)
						NetMessage.SendTileSquare(-1, x, y);

					return false;
				}
			}
		}

		return false;
	}
}
