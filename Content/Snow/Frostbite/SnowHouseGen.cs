using System.Linq;
using Terraria.GameContent.Biomes.CaveHouse;

namespace SpiritReforged.Content.Snow.Frostbite;

internal class SnowHouseGen : ModSystem
{
	/// <summary> The maximum number of <b>guaranteed</b> Frostbite tomes in this world. </summary>
	public static int GenCountMax => WorldGen.GetWorldSize() == WorldGen.WorldSize.Large ? 2 : 1;

	/// <summary> The number of Frostbite tomes generated. </summary>
	private static int GenCount;

	public override void Load() => On_HouseBuilder.FillRooms += AddBooks;

	private static void AddBooks(On_HouseBuilder.orig_FillRooms orig, HouseBuilder self)
	{
		bool genned = false; //Whether Frostbite has already generated in this particular house
		bool shouldGen = GenCount < GenCountMax || WorldGen.genRand.NextBool(5);

		if (self.Type == HouseType.Ice)
		{
			foreach (var room in self.Rooms)
			{
				if (shouldGen && !genned || WorldGen.genRand.NextBool(4 + WorldGen.GetWorldSize()))
				{
					int length = WorldGen.genRand.Next(3, 6);
					PlaceShelf(room.X + 1 + WorldGen.genRand.Next(room.Width - (length + 1)), room.Y + 2, new Point(length, WorldGen.genRand.Next(1, 3)));
				}
			}
		}

		orig(self);

		void PlaceShelf(int originX, int originY, Point size)
		{
			HashSet<Point> safe = []; //Keeps track of empty shelves for Frostbite gen

			for (int x = originX; x < originX + size.X; x++)
			{
				for (int j = 0; j < size.Y; j++)
				{
					int y = originY + j * 2;

					WorldGen.PlaceTile(x, y, TileID.Platforms, style: self.PlatformStyle);

					if (WorldGen.genRand.NextFloat() < .66f)
						WorldGen.PlaceTile(x, y - 1, TileID.Books, style: WorldGen.genRand.Next(6));
					else
						safe.Add(new Point(x, y - 1));
				}
			}

			if (shouldGen && !genned)
			{
				foreach (var pt in safe.OrderBy(x => WorldGen.genRand.Next(safe.Count)))
				{
					int type = ModContent.TileType<FrostbiteTile>();
					WorldGen.PlaceTile(pt.X, pt.Y, type);

					if (Framing.GetTileSafely(pt).TileType == type)
					{
						genned = true;
						GenCount++;
						break;
					}
				}
			}
		}
	}

	public override void PostWorldGen() => GenCount = 0; //Reset to default
}
