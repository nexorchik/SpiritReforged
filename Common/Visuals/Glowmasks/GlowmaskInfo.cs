namespace SpiritReforged.Common.Visuals.Glowmasks;

public readonly struct GlowmaskInfo(Asset<Texture2D> glowmask, Func<object, Color> drawColor, bool drawAutomatically)
{
	public readonly Asset<Texture2D> Glowmask = glowmask;
	public readonly Func<object, Color> GetDrawColor = drawColor;
	public readonly bool DrawAutomatically = drawAutomatically;
}
