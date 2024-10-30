using Terraria.Audio;

namespace SpiritReforged.Content.Ocean.Hydrothermal;

internal class BubbleSoundPlayer : ModPlayer
{
	private readonly SoundStyle sound = new("SpiritReforged/Assets/SFX/Ambient/Bubbling") { SoundLimitBehavior = SoundLimitBehavior.IgnoreNew, IsLooped = true };
	private bool stopped;

	public override void PostUpdateEquips()
	{
		if (Collision.WetCollision(Main.LocalPlayer.position, Main.LocalPlayer.width, Main.LocalPlayer.height)) //Ambient sound logic
		{
			if (stopped)
			{
				SoundEngine.FindActiveSound(in sound)?.Resume();
				stopped = false;
			}
		}
		else
		{
			SoundEngine.FindActiveSound(in sound)?.Stop(); //Stop the sound if the player isn't submerged
			stopped = true;
		}
	}

	public static void StartSound(Vector2 origin)
	{
		if (Main.LocalPlayer.TryGetModPlayer(out BubbleSoundPlayer asp) && !asp.stopped)
		{
			SoundEngine.PlaySound(asp.sound, origin);

			var activeSound = SoundEngine.FindActiveSound(in asp.sound);

			if (activeSound != null && activeSound.Position.HasValue && Main.LocalPlayer.Distance(activeSound.Position.Value) > Main.LocalPlayer.Distance(origin))
				activeSound.Position = origin; //Move the sound to the closest vent
		}
	}
}
