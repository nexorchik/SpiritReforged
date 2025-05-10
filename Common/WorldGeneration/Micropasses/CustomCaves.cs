using SpiritReforged.Common.WorldGeneration.Micropasses.CaveEntrances;
using System.Linq;
using Terraria.DataStructures;
using Terraria.GameContent.Generation;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace SpiritReforged.Common.WorldGeneration.Micropasses;

/// <summary>
/// Handles replacing mountain caves with our custom caves.
/// </summary>
internal class CustomCaves : ILoadable
{
	private static readonly Dictionary<Point16, CaveEntranceType> _caveTypeByPositions = [];

	private static bool AddingMountainCaves = false;

	public void Load(Mod mod)
	{
		On_WorldGen.Mountinater += OverrideGenMound;
		On_WorldGen.Cavinator += ModifyCavinatorForCaveType;
		On_WorldGen.CaveOpenater += On_WorldGen_CaveOpenater;

		var pass = WorldGen.VanillaGenPasses.Values.FirstOrDefault(x => x.Name == "Mountain Caves");

		if (pass != null)
			WorldGen.DetourPass((PassLegacy)pass, AddFlagToMountainCavePass);
	}

	public void Unload()
	{
	}

	private void AddFlagToMountainCavePass(WorldGen.orig_GenPassDetour orig, object self, GenerationProgress progress, GameConfiguration configuration)
	{
		AddingMountainCaves = true;
		orig(self, progress, configuration);
		AddingMountainCaves = false;
	}

	private void On_WorldGen_CaveOpenater(On_WorldGen.orig_CaveOpenater orig, int i, int j)
	{
		if (!AddingMountainCaves)
		{
			orig(i, j);
			return;
		}

		if (!_caveTypeByPositions.TryGetValue(new Point16(i, j), out CaveEntranceType type) || type == CaveEntranceType.Vanilla)
			orig(i, j);
		else if (CaveEntrance.EntranceByType[type].ModifyOpening(ref i, ref j, true))
			orig(i, j);
	}

	private void ModifyCavinatorForCaveType(On_WorldGen.orig_Cavinator orig, int i, int j, int steps)
	{
		if (!AddingMountainCaves)
		{
			orig(i, j, steps);
			return;
		}

		if (!_caveTypeByPositions.TryGetValue(new Point16(i, j), out CaveEntranceType type) || type == CaveEntranceType.Vanilla)
			orig(i, j, steps);
		else if (CaveEntrance.EntranceByType[type].ModifyOpening(ref i, ref j, false))
			orig(i, j, steps);
	}

	private void OverrideGenMound(On_WorldGen.orig_Mountinater orig, int i, int j)
	{
		var type = (CaveEntranceType)WorldGen.genRand.Next(3);

		if (type == CaveEntranceType.Vanilla)
			orig(i, j);
		else
			CaveEntrance.EntranceByType[type].Generate(i, j);

		_caveTypeByPositions.Add(new(i, j), type);
	}
}
