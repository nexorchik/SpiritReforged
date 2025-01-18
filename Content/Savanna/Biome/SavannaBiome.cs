using Terraria.Graphics.Capture;

namespace SpiritReforged.Content.Savanna.Biome;

public class SavannaBiome : ModBiome
{
	private int SavannaMusic => Main.dayTime ? MusicLoader.GetMusicSlot(Mod, "Assets/Music/Savanna") : MusicLoader.GetMusicSlot(Mod, "Assets/Music/SavannaNight");

	public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
	public override int Music => (Main.LocalPlayer.townNPCs > 2f) ? -1 : SavannaMusic;
	public override ModWaterStyle WaterStyle => ModContent.GetInstance<SavannaWaterStyle>();
	public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Normal;

	public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle 
		=> Main.LocalPlayer.ZoneHallow ? ModContent.GetInstance<HallowSavannaBGStyle>() : ModContent.GetInstance<SavannaBGStyle>();
	public override string BestiaryIcon => base.BestiaryIcon;
	public override string BackgroundPath => MapBackground;
	public override string MapBackground => "SpiritReforged/Assets/Textures/Backgrounds/SavannaMapBG";

	public override bool IsBiomeActive(Player player)
	{
		bool surface = player.ZoneSkyHeight || player.ZoneOverworldHeight;
		return SavannaTileCounts.InSavanna && surface && !player.ZoneCorrupt && !player.ZoneCrimson;
	}
}

internal class SavannaTileCounts : ModSystem
{
	public int savannaCount;

	public static bool InSavanna => ModContent.GetInstance<SavannaTileCounts>().savannaCount >= 400;

	public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts) 
		=> savannaCount = tileCounts[ModContent.TileType<Tiles.SavannaGrass>()] + tileCounts[ModContent.TileType<Tiles.SavannaDirt>()];
}
