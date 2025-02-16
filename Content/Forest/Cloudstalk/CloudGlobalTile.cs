using SpiritReforged.Common.TileCommon;
using SpiritReforged.Content.Forest.Cloudstalk.Items;
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

		while (!Main.tile[i, j + 1].HasTile)
			j++;

		var ground = Main.tile[i, j + 1];
		var tile = Main.tile[i, j];

		int herbType = ModContent.TileType<CloudstalkTile>();
		bool validAnchor = TileObjectData.GetTileData(herbType, 0).AnchorValidTiles.Contains(ground.TileType);

		if (validAnchor && ground.Slope == SlopeType.Solid && !ground.IsHalfBlock && tile.LiquidAmount < 100 && TilePlaceHelper.IsReplaceable(i, j) && TilePlaceHelper.CanPlaceHerb(i, j, herbType))
		{
			tile.ClearTile();
			TilePlaceHelper.PlaceTile(i, j, herbType);
		}
	}
}