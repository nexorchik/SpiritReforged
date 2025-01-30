using SpiritReforged.Content.Savanna.Biome;

namespace SpiritReforged.Common.Visuals;

internal class SavannaMenuTheme : ModMenu
{
	public override ModSurfaceBackgroundStyle MenuBackgroundStyle => ModContent.GetInstance<SavannaBGStyle>();
	public override string DisplayName => "Savanna";

	public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor)
	{
		logoScale *= .84f; //Scale is slightly too large by default
		return true;
	}
}
