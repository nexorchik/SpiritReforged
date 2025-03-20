using System.Linq;
using Terraria.Audio;
using Terraria.ModLoader.IO;

namespace SpiritReforged.Common.ItemCommon;

internal class DiscoveryHelper : ModPlayer
{
	private const string nameKey = "discNames";

	/// <summary> Saved and loaded with the player. </summary>
	private static HashSet<string> CollectedNames = [];
	/// <summary> Populated with items in the mod. </summary>
	private static readonly Dictionary<int, SoundStyle> TypeToSound = [];
	public static void RegisterType(int type, SoundStyle sound) => TypeToSound.Add(type, sound);

	public override bool OnPickup(Item item)
	{
		if (!Main.dedServ && Player.whoAmI == Main.myPlayer && TypeToSound.TryGetValue(item.type, out var sound))
		{
			string name = item.ModItem?.Name;

			if (name != null && !CollectedNames.Contains(name))
			{
				SoundEngine.PlaySound(sound);
				CollectedNames.Add(name);
			}
		}

		return true;
	}

	public override void SaveData(TagCompound tag) => tag[nameKey] = CollectedNames.ToList();
	public override void LoadData(TagCompound tag) => CollectedNames = tag.GetList<string>(nameKey).ToHashSet();
}