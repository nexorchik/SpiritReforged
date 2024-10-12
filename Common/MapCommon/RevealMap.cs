using Terraria.Map;

namespace SpiritReforged.Common.MapCommon;

internal class RevealMap
{
	public static void DrawMap(int x, int y, int radius)
	{
		var tile = MapHelper.CreateMapTile(x, y, 255);

		for (int i = x - radius / 2; i <= x + radius / 2; ++i)
		{
			for (int j = y - radius / 2; j <= y + radius / 2; ++j)
			{
				float dist = 1 - Vector2.Distance(new Vector2(x, y), new Vector2(i, j)) / (radius * 0.5f);
				byte light = WorldGen.SolidTile(i, j) ? (byte)(255 * Math.Max(dist, 0)) : (byte)0;

				if (light <= 0)
				{
					continue;
				}

				if (Main.Map[i, j].Light > 0)
				{
					continue;
				}

				SetTileAt(i, j, (byte)(215 + Main.rand.Next(40)), tile);
			}
		}

		Main.refreshMap = true;
	}

	private static void SetTileAt(int x, int y, byte light, MapTile tile)
	{
		tile.Light = light;
		tile.Type = 34;
		tile.IsChanged = false;
		Main.Map.SetTile(x, y, ref tile);
	}
}
