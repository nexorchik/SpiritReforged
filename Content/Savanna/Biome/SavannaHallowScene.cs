/*namespace SpiritReforged.Content.Savanna.Biome;

public class SavannaHallowScene : ModSceneEffect
{
	public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
	public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<HallowSavannaBGStyle>();
	public override ModWaterStyle WaterStyle => default;

	public override bool IsSceneEffectActive(Player player) => SavannaTileCounts.InSavanna && player.ZoneHallow;
}

public class HallowSavannaBGStyle : SavannaBGStyle
{
	public override int ChooseMiddleTexture() => BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/HallowSavannaBackgroundMid");
	public override int ChooseFarTexture() => BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/HallowSavannaBackgroundFar");

	public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b)
	{
		b -= 400;
		return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/HallowSavannaBackgroundNear");
	}
}*/
