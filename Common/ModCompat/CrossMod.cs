namespace SpiritReforged.Common.ModCompat;

/// <summary> Handles general cross mod compatibility. </summary>
internal static class CrossMod
{
	public readonly record struct ModEntry(string Name)
	{
		public readonly string name = Name;

		public readonly bool Enabled
		{
			get
			{
				if (LoadedMods.ContainsKey(name))
					return true;

				if (ModLoader.TryGetMod(name, out var mod))
				{
					LoadedMods.Add(name, mod);
					return true;
				}

				return false;
			}
		}

		/// <summary> Should not be used unless you know this mod is enabled (<see cref="Enabled"/>). </summary>
		public readonly Mod Instance
		{
			get
			{
				if (LoadedMods.TryGetValue(name, out var mod))
					return mod;

				var getMod = ModLoader.GetMod(name);
				LoadedMods.Add(name, getMod);

				return getMod;
			}
		}
	}

	public static readonly ModEntry Fables = new("CalamityFables");
	public static readonly ModEntry Thorium = new("ThoriumMod");
	public static readonly ModEntry NewBeginnings = new("NewBeginnings");
	public static readonly ModEntry MusicDisplay = new("MusicDisplay");
	//public static readonly ModEntry Classic = new("SpiritMod"); //Classic is handled entirely in its own series of classes

	/// <summary> The names and instances of loaded crossmod mods per <see cref="ModEntry"/>. </summary>
	private static readonly Dictionary<string, Mod> LoadedMods = [];
}