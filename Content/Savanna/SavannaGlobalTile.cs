using SpiritReforged.Content.Savanna.Tiles;

namespace SpiritReforged.Content.Savanna;

public class SavannaGlobalTile : GlobalTile
{
	public override void RandomUpdate(int i, int j, int type)
	{
		if (type == ModContent.TileType<SavannaGrass>())
		{
			var above = Framing.GetTileSafely(i, --j); //Target the tile above

			if (Main.rand.NextBool(90) && !above.HasTile && above.LiquidAmount < 80) //The majority of elephant grass generation happens in that class
				WorldGen.PlaceObject(i, j, ModContent.TileType<ElephantGrassShort>(), true, style: Main.rand.Next(3));

			if (Main.rand.NextBool(120) && !above.HasTile && above.LiquidAmount < 80 && !WorldGen.PlayerLOS(i, j)) //Place small termite nests
				WorldGen.PlaceObject(i, j, ModContent.TileType<TermiteMoundSmall>(), true, style: Main.rand.Next(3));
		}
	}
}
