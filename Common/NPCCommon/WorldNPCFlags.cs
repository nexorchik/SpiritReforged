using SpiritReforged.Common.Misc;
using Terraria.ModLoader.IO;

namespace SpiritReforged.Common.NPCCommon;

/// <summary> Handles world spawn data for world NPCs. </summary>
internal class WorldNPCFlags : ModSystem
{
	public bool cartographerSpawned;

	public override void Load() => TimeUtils.JustTurnedDay += ResetAll;
	private void ResetAll() => cartographerSpawned = false;

	public override void SaveWorldData(TagCompound tag) => tag[nameof(cartographerSpawned)] = cartographerSpawned;
	public override void LoadWorldData(TagCompound tag) => cartographerSpawned = tag.GetBool(nameof(cartographerSpawned));
}
