using SpiritReforged.Content.Underground.Moss.Oganesson;
using SpiritReforged.Content.Underground.Moss.Radon;
using System.Reflection;
using Terraria.WorldBuilding;

namespace SpiritReforged.Common.WorldGeneration.Micropasses.Passes;

internal class NewNeonsMicropass : Micropass
{
	private static FieldInfo neonMossValue;

	public override string WorldGenName => "New Neon Mosses";

	public override void Load(Mod mod)
	{
		neonMossValue = typeof(WorldGen).GetField("neonMossType", BindingFlags.Static | BindingFlags.NonPublic);

		On_WorldGen.randMoss += ForceNewMoss;
	}

	private void ForceNewMoss(On_WorldGen.orig_randMoss orig, bool justNeon)
	{
		orig(justNeon);

		if (WorldGen.genRand.NextBool(1))
			neonMossValue.SetValue(null, (ushort)(WorldGen.genRand.NextBool() ? ModContent.TileType<RadonMoss>() : ModContent.TileType<OganessonMoss>()));
	}

	public override int GetWorldGenIndexInsert(List<GenPass> passes, ref bool afterIndex) => passes.FindIndex(genpass => genpass.Name.Equals("Sunflowers"));

	public override void Run(GenerationProgress progress, Terraria.IO.GameConfiguration config)
	{
		progress.Message = Language.GetTextValue("Mods.SpiritReforged.Generation.NeonMosses");

		for (int i = 40; i < Main.maxTilesX - 40; i++)
		{
			for (int j = (int)Main.worldSurface; j < Main.maxTilesY - 200; ++j)
			{
				Tile tile = Main.tile[i, j];

				if (tile.HasTile && tile.TileType == ModContent.TileType<RadonMoss>() || tile.TileType == ModContent.TileType<OganessonMoss>())
				{
					ModTile modTile = ModContent.GetModTile(tile.TileType);

					for (int k = 0; k < 20; ++k)
						modTile.RandomUpdate(i, j);
				}
			}
		}
	}
}
