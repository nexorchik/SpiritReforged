using Terraria.Graphics.CameraModifiers;

namespace SpiritReforged.Common.PlayerCommon;

internal static class QuickCameraModifiers
{
	public static void SimpleShakeScreen(this Player player, float strength, float vibrationCycles, int frames, float distanceFalloff, string uniqueIdentity = null)
	{
		var direction = (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2();
		PunchCameraModifier modifier = new(player.Center, direction, strength, vibrationCycles, frames, distanceFalloff, uniqueIdentity);
		Main.instance.CameraModifiers.Add(modifier);
	}
}
