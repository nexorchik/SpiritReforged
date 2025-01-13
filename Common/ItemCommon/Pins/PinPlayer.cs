using System.Linq;
using Terraria.ModLoader.IO;

namespace SpiritReforged.Common.ItemCommon.Pins;

internal class PinPlayer : ModPlayer
{
	public HashSet<string> unlockedPins = []; //Remember that this data isn't synced
	public readonly HashSet<string> newPins = [];

	public override void SaveData(TagCompound tag) => tag["unlockedPins"] = unlockedPins.ToList();
	public override void LoadData(TagCompound tag) => unlockedPins = tag.GetList<string>("unlockedPins").ToHashSet();
}

internal static class PinPlayerHelper
{
	/// <returns> Whether the pin of the given name is unlocked. </returns>
	public static bool PinUnlocked(this Player player, string pinName) => player.GetModPlayer<PinPlayer>().unlockedPins.Contains(pinName);

	/// <summary> Unlocks the pin of the given name and enables notification logic. </summary>
	/// <returns> False if the pin was already unlocked. </returns>
	public static bool UnlockPin(this Player player, string pinName)
	{
		if (player.GetModPlayer<PinPlayer>().unlockedPins.Add(pinName))
		{
			player.GetModPlayer<PinPlayer>().newPins.Add(pinName);
			return true;
		}

		return false;
	}
}
