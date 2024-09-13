using SpiritReforged.Common.TileCommon;

namespace SpiritReforged.Common.Visuals.Glowmasks;

internal class GlowmaskTile : GlobalTile
{
	public static Dictionary<int, GlowmaskInfo> TileIdToGlowmask = [];

	public override void PostDraw(int i, int j, int type, SpriteBatch spriteBatch)
	{
		if (TileIdToGlowmask.TryGetValue(type, out var glow) && glow.DrawAutomatically)
		{
			Vector2 pos = TileExtensions.DrawPosition(i, j);
			TileExtensions.DrawSloped(i, j, glow.Glowmask.Value, glow.GetDrawColor(new Point(i, j)), Vector2.Zero);
		}
	}
}
