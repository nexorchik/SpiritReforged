namespace SpiritReforged.Common.PlayerCommon;

internal class CollisionPlayer : ModPlayer
{
	public bool fallThrough;
	private bool noReset;

	public bool FallThrough()
	{
		noReset = true;
		return fallThrough;
	}

	public override void ResetEffects()
	{
		if (!noReset)
			fallThrough = false;

		noReset = false;
	}
}
