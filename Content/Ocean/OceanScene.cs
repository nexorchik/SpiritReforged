namespace SpiritReforged.Content.Ocean;

internal class OceanScene : ModSceneEffect
{
	public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
	public override ModWaterStyle WaterStyle => ModContent.GetInstance<OceanWaterStyle>();

	public override bool IsSceneEffectActive(Player player) => player.ZoneBeach && (!player.GetModPlayer<OceanPlayer>().Submerged(30) || NotInDepths(player));
	private static bool NotInDepths(Player plr) => !ModLoader.TryGetMod("ThoriumMod", out Mod thor) || thor.Call("GetZoneAquaticDepths", plr) is bool depths && !depths;
}
