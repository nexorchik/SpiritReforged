using Terraria.DataStructures;
using Terraria.WorldBuilding;

namespace SpiritReforged.Common.WorldGeneration.Micropasses.Passes;

internal class FishingAreaMicropass : Micropass
{
	public static Dictionary<int, Point16[]> OffsetsBySubId = new()
	{
		{ 0, [new Point16(9, 5), new Point16(54, 19), new Point16(6, 15)] }, 
		{ 1, [new Point16(30, 7), new Point16(1, 12)] }, 
		{ 2, [new Point16(28, 3), new Point16(2, 18), new Point16(42, 20)] },
		{ 3, [new Point16(18, 3), new Point16(6, 11), new Point16(2, 17), new Point16(34, 6), new Point16(46, 22)] },
		{ 4, [new Point16(23, 3), new Point16(4, 15), new Point16(51, 2), new Point16(49, 20)] }
	};

	[WorldBound]
	public static readonly HashSet<Rectangle> Coves = [];
	
	public override string WorldGenName => "Fishing Coves";

	public override int GetWorldGenIndexInsert(List<GenPass> passes, ref bool afterIndex) => passes.FindIndex(genpass => genpass.Name.Equals("Sunflowers"));

	public override void Run(GenerationProgress progress, Terraria.IO.GameConfiguration config)
	{
		progress.Message = Language.GetTextValue("Mods.SpiritReforged.Generation.FishingCoves");
		int repeats = (int)(Main.maxTilesX / 4200f * 8);

		for (int i = 0; i < repeats; ++i)
		{
			int subId = WorldGen.genRand.Next(5);
			string subChar = WorldGen.genRand.NextBool() ? "a" : "b";
			string structureName = "Assets/Structures/Coves/FishCove" + subId + subChar;

			Point16 size = StructureHelper.API.Generator.GetStructureDimensions(structureName, SpiritReforgedMod.Instance);
			Point16 position;

			do
			{
				position = new Point16(WorldGen.genRand.Next(200, Main.maxTilesX - 200), WorldGen.genRand.Next((int)Main.rockLayer, (int)(Main.rockLayer + Main.maxTilesY) / 2));
			} while (!Collision.SolidCollision(position.ToWorldCoordinates(), 32, 32));

			position -= WorldGen.genRand.Next(OffsetsBySubId[subId]);

			if (GenVars.structures.CanPlace(new Rectangle(position.X, position.Y, size.X, size.Y), 10))
			{
				if (!StructureTools.SpawnConvertedStructure(position, size, structureName, QuickConversion.BiomeType.Desert))
				{
					i--;
					continue;
				}

				var area = new Rectangle(position.X, position.Y, size.X, size.Y);
				StructureTools.ClearActuators(position.X, position.Y, size.X, size.Y);

				Coves.Add(area);
				GenVars.structures.AddProtectedStructure(area, 6);
			}
			else
				i--;
		}

		foreach (var area in Coves)
			ScanForChests(area);
	}

	/// <summary> Bandaid fix for barrel inventory slots sometimes being air. </summary>
	private static void ScanForChests(Rectangle area)
	{
		foreach (var c in Main.chest)
		{
			if (c != null && area.Contains(new Point(c.x, c.y)))
			{
				List<Item> cached = [];

				for (int i = 0; i < c.item.Length; i++) //Cache all non-air items
				{
					if (c.item[i]?.IsAir == false)
					{
						cached.Add(c.item[i].Clone());
						c.item[i].TurnToAir();
					}
				}

				int index = 0;
				foreach (var item in cached) //Place the cached items in the order they were added
				{
					c.item[index] = item;
					index++;
				}
			}
		}
	}
}
