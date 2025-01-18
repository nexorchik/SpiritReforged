using SpiritReforged.Common.Misc;
using Terraria.ModLoader.IO;

namespace SpiritReforged.Common.NPCCommon;

/// <summary> Handles world spawn data for world NPCs. </summary>
internal class WorldNPCFlags : ModSystem
{
	public bool cartographerSpawned, hikerSpawned;

	public override void Load() => TimeUtils.JustTurnedDay += ResetAll;
	private void ResetAll()
	{
		cartographerSpawned = false;
		hikerSpawned = false;
	}

	public override void SaveWorldData(TagCompound tag)
	{
		tag[nameof(cartographerSpawned)] = cartographerSpawned;
		tag[nameof(hikerSpawned)] = hikerSpawned;
	}

	public override void LoadWorldData(TagCompound tag)
	{
		cartographerSpawned = tag.GetBool(nameof(cartographerSpawned));
		hikerSpawned = tag.GetBool(nameof(hikerSpawned));
	}
}
