namespace SpiritReforged.Common.Misc;

internal class SpiritConditions
{
	// These InBiome properties use an anonymous method to wrap around the Zone checks.
	// VS will mark this as needless, but removing the wrapping delegate will cause issues.
	// This is because otherwise the condition used will capture an invalid LocalPlayer instance, causing a null ref.
	// Fun!
	// Ported Over From Spirit Classic
	public static Condition InSavanna => new("Mods.SpiritReforged.Conditions.InSavanna", () => Main.LocalPlayer.InModBiome<Content.Savanna.Biome.SavannaBiome>());
}
