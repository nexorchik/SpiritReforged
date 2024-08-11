using SpiritReforged.Common.WorldGeneration.Ecotones;
using SpiritReforged.Content.Savanna.Tiles;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace SpiritReforged.Content.Savanna;

internal class SavannaEcotone : EcotoneBase
{
	public override void Generate(GenerationProgress progress, GameConfiguration config, List<EcotoneSurfaceMapping.EcotoneEntry> entries)
	{
		var entry = entries.Find(x => x.SurroundedBy("Desert", "Jungle"));

		if (entry is null)
			return;

		foreach (Point position in entry.SurfacePoints)
		{
			for (int i = -5; i < 10; ++i)
			{
				Tile tile = Main.tile[position.X, position.Y + i];

				if (WorldGen.SolidOrSlopedTile(tile) && tile.TileType == TileID.Dirt)
					WorldGen.PlaceTile(position.X, position.Y + i, ModContent.TileType<SavannaDirt>(), true, true);
			}
		}
	}
}
