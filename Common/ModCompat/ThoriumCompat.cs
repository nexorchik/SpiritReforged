namespace SpiritReforged.Common.ModCompat;

internal class ThoriumCompat : ModSystem
{
	public static Mod Instance;
	public static bool Enabled => Instance != null;

	public override void Load()
	{
		Instance = null;
		if (!ModLoader.TryGetMod("ThoriumMod", out Instance))
			return;
	}
}
