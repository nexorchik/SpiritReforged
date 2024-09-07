using SpiritReforged.Content.Savanna.Tiles;

namespace SpiritReforged.Content.Savanna;

public class SavannaGlobalTile : GlobalTile
{
	public override void RandomUpdate(int i, int j, int type)
	{
		if (type == ModContent.TileType<SavannaGrass>() && Main.rand.NextBool(90) && Framing.GetTileSafely(i, j - 1).LiquidAmount < 80 && !Framing.GetTileSafely(i, j).TopSlope && !Framing.GetTileSafely(i, j - 1).HasTile && !Framing.GetTileSafely(i, j - 2).HasTile)
			WorldGen.PlaceObject(i, j - 1, ModContent.TileType<ElephantGrassShort>(), true); //The majority of elephant grass generation happens in that class
	}
}
