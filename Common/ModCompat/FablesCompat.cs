namespace SpiritReforged.Common.ModCompat;

internal class FablesCompat : ILoadable
{
	private const string FablesName = "CalamityFables";

	internal static Mod Instance = null;
	public static bool Enabled => Instance != null || ModLoader.HasMod(FablesName);

	public void Load(Mod mod)
	{
		if (!ModLoader.TryGetMod(FablesName, out Instance))
			return;
	}

	public void Unload() { }
}
