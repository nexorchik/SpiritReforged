namespace SpiritReforged.Common.Visuals;

internal class VFX : ILoadable
{
	public static Asset<Texture2D> Bloom = null;

	public void Load(Mod mod)
	{
		Bloom = ModContent.Request<Texture2D>("SpiritReforged/Assets/Textures/Bloom");
	}

	public void Unload()
	{
	}

	/// <summary>
	/// Draws bloom at the given position.
	/// </summary>
	/// <param name="worldPosition"></param>
	/// <param name="color"></param>
	/// <param name="scaleInPixels"></param>
	/// <param name="origin"></param>
	public static void DrawBloom(Vector2 worldPosition, Color color, float scaleInPixels, Vector2? origin = null)
	{
		origin ??= new(0.5f);
		Main.spriteBatch.Draw(Bloom.Value, worldPosition, null, color, 0f, origin.Value, Bloom.Value.Width / scaleInPixels, SpriteEffects.None, 0f);
	}
}
