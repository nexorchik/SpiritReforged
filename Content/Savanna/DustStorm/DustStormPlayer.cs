using Terraria.GameContent.Events;
using Terraria.Graphics.Effects;

namespace SpiritReforged.Content.Savanna.DustStorm;

public class DustStormPlayer : ModPlayer
{
	private bool wasDustStorm;

	/// <summary> Whether the player is present in a dust storm. </summary>
	public bool ZoneDustStorm => Math.Abs(Main.windSpeedCurrent) > .4f && Player.InModBiome<Biome.SavannaBiome>();

	public override void PostUpdateMiscEffects()
	{
		string sandstorm = "Sandstorm";

		if (ZoneDustStorm)
		{
			float intensity = .35f;

			if (!Filters.Scene[sandstorm].IsActive()) //Initialize
			{
				//Severity has a value even if it isn't being used. Doing this prevents snapping to high-severity sandstorm vfx upon entering a dust storm zone
				if (!Filters.Scene[sandstorm].IsInUse())
					Sandstorm.Severity = intensity;

				var center = Player.Center;

				SkyManager.Instance.Activate(sandstorm, center);
				Filters.Scene.Activate(sandstorm, center);
				Overlays.Scene.Activate(sandstorm, center); //Might have no effect?
			}

			if (!Player.ZoneSandstorm)
				Sandstorm.Severity = MathHelper.Max(Sandstorm.Severity - .01f, intensity); //Transition into a calmer severity if necessary
		}

		if (!ZoneDustStorm && wasDustStorm && Filters.Scene[sandstorm].IsActive() && !Player.ZoneSandstorm)
		{
			SkyManager.Instance.Deactivate(sandstorm);
			Filters.Scene.Deactivate(sandstorm);
			Overlays.Scene.Deactivate(sandstorm);
		}

		wasDustStorm = ZoneDustStorm;
	}
}
