using Terraria.GameContent.Biomes.CaveHouse;

namespace SpiritReforged.Content.Snow.Frostbite;

internal class SnowHouseGen : ILoadable
{
	private static bool Generated = false;

	public void Load(Mod mod) => On_HouseBuilder.FillRooms += AddBooks;

	private void AddBooks(On_HouseBuilder.orig_FillRooms orig, HouseBuilder self)
	{
		if (self.Type == HouseType.Ice)
		{
			if (!Generated)
			{
				Rectangle room = self.Rooms[WorldGen.genRand.Next(self.Rooms.Count)];

				int length = WorldGen.genRand.Next(3, 6);

				PlaceShelf(room.X + 1 + WorldGen.genRand.Next(room.Width - (length + 1)), room.Y + 2, length, WorldGen.genRand.Next(1, 3));
				Generated = true; //The first snow house generated always has Frostbite
			}
			else
			{
				foreach (var room in self.Rooms)
				{
					if (WorldGen.genRand.NextBool(4 + WorldGen.GetWorldSize()))
					{
						int length = WorldGen.genRand.Next(3, 6);
						PlaceShelf(room.X + 1 + WorldGen.genRand.Next(room.Width - (length + 1)), room.Y + 2, length, WorldGen.genRand.Next(1, 3));
					}
				}
			}
		}

		orig(self);

		void PlaceShelf(int x, int y, int length, int num)
		{
			int specialPos = Generated ? -1 : WorldGen.genRand.Next(length * num); //Which tile Frostbite should generate on

			for (int i = 0; i < length; i++)
			{
				for (int j = 0; j < num; j++)
				{
					WorldGen.PlaceTile(x + i, y + j * 2, TileID.Platforms, style: self.PlatformStyle);

					if (specialPos == 0)
						WorldGen.PlaceTile(x + i, y + j * 2 - 1, ModContent.TileType<FrostbiteTile>());
					else if (WorldGen.genRand.NextFloat() < .66f)
						WorldGen.PlaceTile(x + i, y + j * 2 - 1, TileID.Books, style: WorldGen.genRand.Next(6));

					specialPos--;
				}
			}
		}
	}

	public void Unload() { }
}
