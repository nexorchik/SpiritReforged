using System.Diagnostics;
using System.Linq;
using Terraria.WorldBuilding;

namespace SpiritReforged.Common.WorldGeneration.Micropasses.Discoveries;

internal class DiscoveryHandler : ILoadable
{
	internal static readonly HashSet<Discovery> valid = [];

	public void Load(Mod mod) => On_WorldGen.GenerateWorld_RunTasksAndFinish += SelectDiscoveries;

	private static void SelectDiscoveries(On_WorldGen.orig_GenerateWorld_RunTasksAndFinish orig, int seed, Stopwatch generationStopwatch, GenerationProgress customProgressObject)
	{
		const int numDiscoveries = 2; //Per world
		var discoveries = ModContent.GetContent<Discovery>().ToArray();

		for (int a = 0; a < numDiscoveries; a++)
			if (!valid.Add(discoveries[WorldGen.genRand.Next(discoveries.Length)]))
				a--;

		orig(seed, generationStopwatch, customProgressObject); //ModifyWorldGenTasks is called

		valid.Clear();
	}

	public void Unload() { }
}
