using Terraria.GameContent.Events;
using Terraria.GameContent.Skies;
using Terraria.Graphics.Effects;

namespace SpiritReforged.Content.Savanna.DustStorm;

public class DustStormPlayer : ModPlayer
{
	private const float SkyIntensity = .18f; //How intense the tint of the sky is during a dust storm

	/// <summary> Whether the player is present in a dust storm. </summary>
	public bool ZoneDustStorm => Math.Abs(Main.windSpeedCurrent) > .4f && Player.InModBiome<Biome.SavannaBiome>();

	public override void Load() => On_SandstormSky.Draw += HijackSandstormSky;

	private void HijackSandstormSky(On_SandstormSky.orig_Draw orig, SandstormSky self, SpriteBatch spriteBatch, float minDepth, float maxDepth)
	{
		//Temporarily hijack sandstorm intensity to control sky color
		float _severity = Sandstorm.Severity;

		if (Main.LocalPlayer.TryGetModPlayer(out DustStormPlayer player) && player.ZoneDustStorm)
			Sandstorm.Severity = SkyIntensity;

		orig(self, spriteBatch, minDepth, maxDepth);

		Sandstorm.Severity = _severity;
	}

	public override void PostUpdateMiscEffects()
	{
		string sandstorm = "Sandstorm";

		if (ZoneDustStorm && !Filters.Scene[sandstorm].IsActive()) //Initialize
		{
			var center = Player.Center;

			SkyManager.Instance.Activate(sandstorm, center);
			Filters.Scene.Activate(sandstorm, center);
			Overlays.Scene.Activate(sandstorm, center); //Might have no effect?
		}
		else if (Filters.Scene[sandstorm].IsActive()) //Ongoing
		{
			Filters.Scene[sandstorm].GetShader().UseOpacity(.35f);
			Filters.Scene[sandstorm].GetShader().UseIntensity(.28f);

			if (!ZoneDustStorm) //Deactivate
			{
				SkyManager.Instance.Deactivate(sandstorm);
				Filters.Scene.Deactivate(sandstorm);
				Overlays.Scene.Deactivate(sandstorm);
			}
		}
	}
}
