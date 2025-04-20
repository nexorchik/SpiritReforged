using Terraria.WorldBuilding;

namespace SpiritReforged.Common.WorldGeneration.Micropasses.Passes;

internal class CustomCaveEntranceMicropass : Micropass
{
	public override string WorldGenName => "Cave Entrances";

	public override void Load(Mod mod) => On_WorldGen.Mountinater += OverrideGenMound;

	private void OverrideGenMound(On_WorldGen.orig_Mountinater orig, int i, int j)
	{
		//orig(i, j);
	}

	public override int GetWorldGenIndexInsert(List<GenPass> passes, ref bool afterIndex) => passes.FindIndex(genpass => genpass.Name.Equals("Sunflowers"));

	public override void Run(GenerationProgress progress, Terraria.IO.GameConfiguration config)
	{
	}
}
