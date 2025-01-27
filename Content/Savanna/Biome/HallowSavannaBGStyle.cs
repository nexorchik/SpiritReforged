using MonoMod.Cil;

namespace SpiritReforged.Content.Savanna.Biome;

public class HallowSavannaBGStyle : SavannaBGStyle
{
	public override void Load() => IL_Main.GetPreferredBGStyleForPlayer += OverrideBackgroundStyle;

	/// <summary>
	/// Manually override the surface background without using a <see cref="ModSceneEffect"/> to avoid changing additional data (namely water style).
	/// </summary>
	/// <param name="il"></param>
	private static void OverrideBackgroundStyle(ILContext il)
	{
		ILCursor c = new(il);

		if (!c.TryGotoNext(x => x.MatchLdsfld("Terraria.ModLoader.GlobalBackgroundStyleLoader", "loaded")))
		{
			SpiritReforgedMod.Instance.Logger.Debug("Failed goto GlobalBackgroundStyleLoader.loaded");
			return;
		}

		c.GotoNext(x => x.MatchRet());
		c.EmitDelegate(ModifyStyle);
	}

	/// <summary> Change the surface background to <see cref="HallowSavannaBGStyle"/> when in the Hallow and Savanna biomes. </summary>
	/// <param name="style"> The background ID. </param>
	/// <returns> The background ID to use. </returns>
	private static int ModifyStyle(int style)
	{
		if (style is 5 or 6 && Main.LocalPlayer.ZoneHallow && SavannaTileCounts.InSavanna) //Only modify the background if a hallowed style is active
			style = ModContent.GetInstance<HallowSavannaBGStyle>().Slot;

		return style;
	}

	public override int ChooseMiddleTexture() => BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/HallowSavannaBackgroundMid");
	public override int ChooseFarTexture() => BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/HallowSavannaBackgroundFar");

	public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b)
	{
		b -= 400;
		return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/HallowSavannaBackgroundNear");
	}
}
