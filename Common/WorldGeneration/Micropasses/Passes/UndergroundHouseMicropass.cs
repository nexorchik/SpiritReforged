using rail;
using SpiritReforged.Common.TileCommon;
using Terraria.GameContent.Biomes.CaveHouse;
using Terraria.WorldBuilding;

namespace SpiritReforged.Common.WorldGeneration.Micropasses.Passes;

internal class UndergroundHouseMicropass : Micropass
{
	public override string WorldGenName => "Underground House Modifications";

	public override void Load(Mod mod)
	{
		On_HouseBuilder.Place += PostBuildHouse;
	}

	private void PostBuildHouse(On_HouseBuilder.orig_Place orig, HouseBuilder self, HouseBuilderContext context, StructureMap structures)
	{
		orig(self, context, structures);

		if (self is not WoodHouseBuilder)
			return;

		bool hasPlaced = false;
		List<Rectangle> rooms = [];
		byte skipFlags = 0b_00;

		foreach (Rectangle room in self.Rooms)
		{
			int y = room.Height - 1 + room.Y;

			if ((skipFlags & 0b_01) != 0b_01 && PlaceDecorInRoom(room, y, TileID.Signs))
			{
				hasPlaced = true;
				skipFlags |= 0b_01;
			}

			if ((skipFlags & 0b_10) != 0b_10 && WorldGen.genRand.NextBool(3) && PlaceDecorInRoom(room, y, TileID.Mannequin))
			{
				hasPlaced = true;
				skipFlags |= 0b_10;
			}

			if (hasPlaced)
			{
				rooms.Add(room);
			}
		}

		if (hasPlaced)
		{
			foreach (Rectangle room in rooms)
			{
				for (int i = room.Left; i < room.Right; ++i)
				{
					for (int j = room.Top; j < room.Bottom; ++j)
					{
						Tile tile = Main.tile[i, j];

						if (!tile.HasTile || tile.TileType is not TileID.Signs and not TileID.Mannequin)
							continue;

						int x = i;
						int y = j;

						TileExtensions.GetTopLeft(ref x, ref y);
						tile = Main.tile[x, y];

						if (tile.TileType == TileID.Signs)
						{
							int sign = Sign.ReadSign(x, y);

							Main.sign[sign].text = WorldGen.genRand.NextBool(25)
								? Language.GetTextValue("Mods.SpiritReforged.Generation.Signs.Underground.Rare." + Main.rand.Next(3))
								: Language.GetTextValue("Mods.SpiritReforged.Generation.Signs.Underground.Common." + Main.rand.Next(11));
						}
					}
				}
			}
		}
	}

	private static bool PlaceDecorInRoom(Rectangle room, int y, int type)
	{
		bool hasPlaced = false;

		for (int i = 0; i < 10; i++)
		{
			if (hasPlaced = WorldGen.PlaceObject(WorldGen.genRand.Next(2, room.Width - 2) + room.X, y - 1, type, true))
				break;
		}

		if (hasPlaced)
			return true;

		for (int j = room.X + 2; j <= room.X + room.Width - 2; j++)
		{
			if (hasPlaced = WorldGen.PlaceObject(j, y, type, true))
				break;
		}

		return hasPlaced;
	}

	public override int GetWorldGenIndexInsert(List<GenPass> passes, ref bool afterIndex) => passes.FindIndex(genpass => genpass.Name.Equals("Sunflowers"));

	public override void Run(GenerationProgress progress, Terraria.IO.GameConfiguration config)
	{
	}
}
