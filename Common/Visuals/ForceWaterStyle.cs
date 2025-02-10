using MonoMod.Cil;

namespace SpiritReforged.Common.Visuals;

/// <summary> Manually overrides water style in specific scenarios. Doesn't exist on the server. </summary>
[Autoload(Side = ModSide.Client)]
internal class ForceWaterStyle : ModSystem
{
	//private static int DeepOceanWaterStyle;
	private static readonly Dictionary<int, int> StyleSets = []; //background, water

	public override void Load() => IL_Main.CalculateWaterStyle += SetWaterStyleDefault;

	public override void SetStaticDefaults()
	{
		if (Main.dedServ)
			return;

		foreach (var scene in ModContent.GetContent<ModSceneEffect>())
		{
			if (scene.SurfaceBackgroundStyle != null && scene.WaterStyle != null)
				StyleSets.Add(scene.SurfaceBackgroundStyle.Slot, scene.WaterStyle.Slot);
		}

		//DeepOceanWaterStyle = ModContent.GetInstance<DeepOceanBackgroundStyle>().Slot;
	}

	private static void SetWaterStyleDefault(ILContext il)
	{
		ILCursor c = new(il);

		c.Index = c.Instrs.Count;
		c.GotoPrev(x => x.MatchRet());

		c.EmitDelegate(Modify);
	}

	/// <summary> Modify the default case of modded water styles (purity).<br/>
	/// Used to prevent water from turning to purity before turning to the correct style during some scene transitions. </summary>
	/// <param name="style"> The current water style. </param>
	/// <returns> The replacement water style. </returns>
	private static int Modify(int style)
	{
		//if (Main.bgStyle == DeepOceanWaterStyle)
		//	return DeepOceanBackgroundStyle.ChooseWaterStyle();

		foreach (var set in StyleSets)
		{
			if (Main.bgStyle == set.Key)
				return set.Value;
		}

		return style;
	}
}
