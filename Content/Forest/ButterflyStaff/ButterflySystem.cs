using Terraria.ModLoader.IO;

namespace SpiritReforged.Content.Forest.ButterflyStaff;

internal class ButterflySystem : ModSystem
{
	public readonly HashSet<Rectangle> butterflyZones = [];

	public override void SaveWorldData(TagCompound tag)
	{
		if (butterflyZones is null || butterflyZones.Count == 0)
			return;

		int count = 0;

		foreach (var zone in butterflyZones)
		{
			tag[$"butterflyZone{count}"] = zone;
			count++;
		}

		tag["numButterflyZones"] = count;
	}

	public override void LoadWorldData(TagCompound tag)
	{
		int count = tag.GetInt("numButterflyZones");

		for (int i = 0; i < count; i++)
		{
			var zone = tag.Get<Rectangle>($"butterflyZone{count}");

			if (zone != default)
				butterflyZones.Add(zone);
		}
	}
}
