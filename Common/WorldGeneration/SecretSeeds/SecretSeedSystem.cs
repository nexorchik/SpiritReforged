using System.Linq;
using Terraria.GameContent.UI.States;
using Terraria.ModLoader.IO;

namespace SpiritReforged.Common.WorldGeneration.Seeds;

internal class SecretSeedSystem : ModSystem
{
	private const byte Default = byte.MaxValue;

	/// <summary> The Spirit secret seed currently in use. Returns null if none. </summary>
	public static SecretSeed WorldSecretSeed
	{
		get
		{
			if (SelectedSeed == Default)
				return null;

			return SecretSeeds[SelectedSeed];
		}
	}

	//[WorldBound]
	private static byte SelectedSeed = Default;

	private static byte NextIndex;
	private static readonly Dictionary<byte, SecretSeed> SecretSeeds = [];

	public static SecretSeed GetSeed<T>() where T : SecretSeed => SecretSeeds.Values.Where(x => x.GetType() == typeof(T)).FirstOrDefault();

	public static void RegisterSeed(SecretSeed seed)
	{
		SecretSeeds.Add(NextIndex, seed);
		NextIndex++;
	}

	public override void Load() => On_UIWorldCreation.ProcessSpecialWorldSeeds += ProcessCustom;

	private static void ProcessCustom(On_UIWorldCreation.orig_ProcessSpecialWorldSeeds orig, string processedSeed)
	{
		orig(processedSeed);

		for (byte i = 0; i < SecretSeeds.Count; i++)
		{
			var seed = SecretSeeds[i];
			if (processedSeed.Equals(seed.Name, StringComparison.CurrentCultureIgnoreCase))
			{
				SelectedSeed = i;
				break;
			}
		}
	}

	public override void SaveWorldData(TagCompound tag) => tag[nameof(SelectedSeed)] = SelectedSeed;
	public override void LoadWorldData(TagCompound tag) => SelectedSeed = tag.GetByte(nameof(SelectedSeed));

	public override void Unload()
	{
		SecretSeeds.Clear();
		NextIndex = 0;
	}
}