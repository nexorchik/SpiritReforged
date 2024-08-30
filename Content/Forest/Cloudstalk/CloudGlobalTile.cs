using System.Linq;

namespace SpiritReforged.Content.Forest.Cloudstalk;

internal class CloudGlobalTile : GlobalTile
{
	public override void RandomUpdate(int i, int j, int type)
	{
		float chance = type switch
		{
			TileID.RainCloud => .001f,
			TileID.SnowCloud => .0001f,
			TileID.Cloud => 0.005f,
			_ => 0
		};

		if (Main.tile[i, j + 1].HasTile || Main.rand.NextFloat() >= chance)
			return;

		int y = j + 1;
		while (!Main.tile[i, y].HasTile)
			y++;

		Tile ground = Main.tile[i, y];
		int herbType = ModContent.TileType<Items.CloudstalkTile>();
		bool validTiles = TileObjectData.GetTileData(herbType, 0).AnchorValidTiles.Contains(Main.tile[i, y].TileType);

		if (validTiles && !ground.TopSlope)
			if (WorldGen.PlaceTile(i, y - 1, herbType, true, false))
				NetMessage.SendObjectPlacement(-1, i, y - 1, herbType, 0, 0, -1, -1);
	}
}
