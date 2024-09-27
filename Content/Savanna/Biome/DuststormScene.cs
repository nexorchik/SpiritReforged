using SpiritReforged.Content.Savanna.DustStorm;

namespace SpiritReforged.Content.Savanna.Biome;

public class DuststormScene : ModSceneEffect
{
	public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
	public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/Duststorm");

	public override bool IsSceneEffectActive(Player player) => player.InModBiome<SavannaBiome>() && player.GetModPlayer<DustStormPlayer>().ZoneDustStorm;
}
