using Terraria.Utilities;

namespace SpiritReforged.Common.Misc;

internal static class MiscExtensions
{
	public static Vector2 NextVec2CircularEven(this UnifiedRandom rand, float halfWidth, float halfHeight)
	{
		double x = rand.NextDouble();
		double y = rand.NextDouble();

		if (x + y > 1)
		{
			x = 1 - x;
			y = 1 - y;
		}

		double s = 1 / (x + y);

		if (double.IsNaN(s))
			return Vector2.Zero;

		s *= s;
		s = Math.Sqrt(x * x * s + y * y * s);
		s = 1 / s;
		x *= s;
		y *= s;

		double angle = rand.NextDouble() * (2 * Math.PI);
		double cos = Math.Cos(angle);
		double sin = Math.Sin(angle);

		return new Vector2((float)(x * cos - y * sin) * halfWidth, (float)(x * sin + y * cos) * halfHeight);
	}

	public static Vector2 TurnRight(this Vector2 vec) => new(-vec.Y, vec.X);

	public static Vector2 TurnLeft(this Vector2 vec) => new(vec.Y, -vec.X);

	/// <summary>
	/// Ends & restarts the spriteBatch with default vanilla parameters.
	/// </summary>
	public static void RestartToDefault(this SpriteBatch batch)
	{
		batch.End();
		batch.BeginDefault();
	}

	/// <summary>
	/// Begins the spriteBatch with default vanilla parameters.
	/// </summary>
	/// <param name="batch"></param>
	public static void BeginDefault(this SpriteBatch batch) => batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
}
