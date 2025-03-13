namespace SpiritReforged.Common.Visuals;

/// <summary> Static helper class containing a method for quickly drawing godrays using spritebatch </summary>
public static class DrawGodray
{
	public static void DrawGodrays(SpriteBatch spriteBatch, Vector2 position, Color rayColor, float baseLength, float width, int numRays)
	{
		for (int i = 0; i < numRays; i++)
		{
			var ray = AssetLoader.LoadedTextures["Ray"].Value;
			float rotation = i * (MathHelper.TwoPi / numRays) + Main.GlobalTimeWrappedHourly * ((i % 3 + 1f) / 3) - MathHelper.PiOver2; //Half of rays rotate faster, so it looks less like a rotating static image

			float length = baseLength * (float)(Math.Sin((Main.GlobalTimeWrappedHourly + i) * 2) / 5 + 1); //Arbitrary sine function to fluctuate length between rays over time
			var rayscale = new Vector2(width / ray.Width, length / ray.Height);
			
			spriteBatch.Draw(ray, position, null, rayColor, rotation,
				new Vector2(ray.Width / 2, 0), rayscale, SpriteEffects.None, 0);
		}
	}

	public static void DrawGodrayStraight(SpriteBatch spriteBatch, Vector2 position, Color rayColor, float baseLength, float width, float rotation)
	{
		var ray = AssetLoader.LoadedTextures["Ray"].Value;
		float length = baseLength * (float)(Math.Sin((Main.GlobalTimeWrappedHourly) * 2) / 5 + 1); //Arbitrary sine function to fluctuate length between rays over time
		var rayscale = new Vector2(width / ray.Width, length / ray.Height);

		spriteBatch.Draw(ray, position, null, rayColor, rotation,
			new Vector2(ray.Width / 2, 0), rayscale, SpriteEffects.None, 0);
	}
}