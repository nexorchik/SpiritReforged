using SpiritReforged.Content.Ocean.Items.Driftwood;
using SpiritReforged.Content.Ocean.NPCs.OceanSlime;
using SpiritReforged.Content.Ocean.Tiles;
using System.Linq;

namespace SpiritReforged.Content.Ocean;

public class OceanGlobalTile : GlobalTile
{
	public override void RandomUpdate(int i, int j, int type)
	{
		int[] sands = [TileID.Sand, TileID.Crimsand, TileID.Ebonsand]; //All valid sands
		int[] woods = [TileID.WoodBlock, TileID.BorealWood, TileID.Ebonwood, TileID.DynastyWood, TileID.RichMahogany, TileID.PalmWood, TileID.Shadewood, TileID.WoodenBeam,
			ModContent.TileType<DriftwoodTile>(), TileID.Pearlwood];

		bool inOcean = (i < Main.maxTilesX / 16 || i > Main.maxTilesX / 16 * 15) && j < (int)Main.worldSurface; //Might need adjustment; don't know if this will be exclusively in the ocean
		bool inWorldBounds = i > 40 && i < Main.maxTilesX - 40;

		if (sands.Contains(type) && inOcean && inWorldBounds && !Framing.GetTileSafely(i, j - 1).HasTile && !Framing.GetTileSafely(i, j).TopSlope) //woo
		{
			if (Framing.GetTileSafely(i, j - 1).LiquidAmount > 200) //water stuff
			{
				if (Main.rand.NextBool(35))
					WorldGen.PlaceTile(i, j - 1, ModContent.TileType<OceanKelp>(), true); //Kelp spawning

				bool openSpace = !Framing.GetTileSafely(i, j - 2).HasTile;
				if (openSpace && Main.rand.NextBool(40)) //1x2 kelp
					WorldGen.PlaceObject(i, j - 1, ModContent.TileType<Kelp1x2>(), true);

				openSpace = !Framing.GetTileSafely(i + 1, j - 1).HasTile && !Framing.GetTileSafely(i + 1, j - 2).HasTile && !Framing.GetTileSafely(i, j - 2).HasTile;
				if (openSpace && Framing.GetTileSafely(i + 1, j).HasTile && Main.tileSolid[Framing.GetTileSafely(i + 1, j).TileType] && Framing.GetTileSafely(i + 1, j).TopSlope && Main.rand.NextBool(80)) //2x2 kelp
					WorldGen.PlaceObject(i, j - 1, ModContent.TileType<Kelp2x2>(), true);

				openSpace = !Framing.GetTileSafely(i + 1, j - 1).HasTile && !Framing.GetTileSafely(i + 1, j - 2).HasTile && !Framing.GetTileSafely(i, j - 2).HasTile && !Framing.GetTileSafely(i + 1, j - 3).HasTile && !Framing.GetTileSafely(i, j - 3).HasTile;
				if (openSpace && Framing.GetTileSafely(i + 1, j).HasTile && Main.tileSolid[Framing.GetTileSafely(i + 1, j).TileType] && Framing.GetTileSafely(i + 1, j).TopSlope && Main.rand.NextBool(90)) //2x3 kelp
					WorldGen.PlaceObject(i, j - 1, ModContent.TileType<Kelp2x3>(), true);
			}
			else if (Main.rand.NextBool(6))
				SpawnSeagrass(i, j, 5);
		}

		if (inOcean && inWorldBounds && woods.Contains(Framing.GetTileSafely(i, j).TileType))
		{
			for (int k = i - 1; k < i + 2; ++k)
			{
				for (int l = j - 1; l < j + 2; ++l)
				{
					if (k == i && l == j)
						continue; //Dont check myself

					Tile cur = Framing.GetTileSafely(k, l);
					if (!cur.HasTile && cur.LiquidAmount > 155 && cur.LiquidType == LiquidID.Water && Main.rand.NextBool(6))
					{
						int musselType = ModContent.TileType<Mussel>();
						WorldGen.PlaceTile(k, l, musselType, style: Main.rand.Next(TileObjectData.GetTileData(musselType, 0).RandomStyleRange));
						
						return;
					}
				}
			}
		}
	}

	private static void SpawnSeagrass(int i, int j, int rangeFromGrass)
	{
		bool GrassInRange()
		{
			for (int x = 0; x < rangeFromGrass * 2; x++)
			{
				int scanX = i + x % rangeFromGrass * ((x >= rangeFromGrass - 1) ? -1 : 1); //Scan right then left

				if (Framing.GetTileSafely(scanX, j).TileType == TileID.Grass)
					return true;
			}

			return false;
		} //Checks whether grass is in range (left or right) of the tile at these coordinates

		if (GrassInRange())
			WorldGen.PlaceTile(i, j - 1, ModContent.TileType<Seagrass>(), true, true, -1, Main.rand.Next(16));
	}

	public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
	{
		if (type == TileID.PalmTree && Main.rand.NextBool(10) && (i < 300 || i > Main.maxTilesX - 300)) //palm trees at/near the beach
		{
			if (fail)
			{
				int x = i;
				int y = j;

				// Crawl upwards until an air tile is found
				while (y > 0 && Main.tile[x, y].HasTile)
				{
					y--;
				}

				if (NPC.CountNPCS(ModContent.NPCType<OceanSlime>()) < 1) //too many of these guys has to feel bad... right? feel free to remove if we want to troll players
					NPC.NewNPC(WorldGen.GetItemSource_FromTreeShake(i, j), x * 16, y * 16, ModContent.NPCType<OceanSlime>());
			}
		}
	}
}
