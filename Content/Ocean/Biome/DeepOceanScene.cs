namespace SpiritReforged.Content.Ocean.Biome;

internal class DeepOceanScene : ModSceneEffect
{
	public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/DeepOcean");
	public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
	public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<DeepOceanBackgroundStyle>();

	public override bool IsSceneEffectActive(Player player)
	{
		var coords = player.Center.ToTileCoordinates();
		return player.GetModPlayer<OceanPlayer>().ZoneDeepOcean && WorldGen.oceanDepths(coords.X, coords.Y);
	}
}
