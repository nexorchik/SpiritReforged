namespace SpiritReforged.Content.Savanna.DustStorm;

public class DustStormPlayer : ModPlayer
{
	/// <summary> Whether the player is present in a dust storm. </summary>
	public bool ZoneDustStorm => Math.Abs(Main.windSpeedCurrent) > .4f && Player.InModBiome<Biome.SavannaBiome>();
}
