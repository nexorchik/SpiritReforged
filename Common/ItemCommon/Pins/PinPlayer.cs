using System.Linq;
using Terraria.ModLoader.IO;

namespace SpiritReforged.Common.ItemCommon.Pins;

internal class PinPlayer : ModPlayer
{
	public HashSet<string> unlockedPins = [];

	public static bool Obtained(Player player, string pinName) => player.GetModPlayer<PinPlayer>().unlockedPins.Contains(pinName);
	public override void SaveData(TagCompound tag) => tag["unlockedPins"] = unlockedPins.ToList();
	public override void LoadData(TagCompound tag) => unlockedPins = tag.GetList<string>("unlockedPins").ToHashSet();
}
