using static SpiritReforged.Common.ModCompat.CrossMod;

namespace SpiritReforged.Common.ModCompat;

internal class MusicDisplayCompat : ModSystem
{
	public override bool IsLoadingEnabled(Mod mod) => MusicDisplay.Enabled;
	public override void PostSetupContent()
	{
		var display = (Mod)MusicDisplay;
		LocalizedText modName = Language.GetText("Mods.SpiritReforged.MusicDisplay.ModName");

		void AddMusic(string name)
		{
			LocalizedText author = Language.GetText("Mods.SpiritReforged.MusicDisplay." + name + ".Author");
			LocalizedText displayName = Language.GetText("Mods.SpiritReforged.MusicDisplay." + name + ".DisplayName");
			display.Call("AddMusic", (short)MusicLoader.GetMusicSlot(SpiritReforgedMod.Instance, "Assets/Music/" + name), displayName, author, modName);
		}

		AddMusic("Duststorm");
		AddMusic("Savanna");
		AddMusic("SavannaNight");
		AddMusic("DeepOcean");
	}
}