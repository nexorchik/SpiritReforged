namespace SpiritReforged.Common.ModCompat;

internal class FablesCompat : ModSystem
{
	public static Mod Instance;
	public static bool Enabled => Instance != null;

	public override void Load()
	{
		Instance = null;
		if (!ModLoader.TryGetMod("CalamityFables", out Instance))
			return;
	}
}
