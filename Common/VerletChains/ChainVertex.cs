namespace SpiritReforged.Common.VerletChains;

public class ChainVertex(Vector2 position, float scale, float drag = 0.9f, float groundBounce = 0.5f, float gravity = 0.2f)
{
	public Vector2 Position { get; set; } = position;
	public Vector2 LastPosition { get; set; } = position;
	public float Drag { get; set; } = drag;
	public float GroundBounce { get; set; } = groundBounce;
	public float Gravity { get; set; } = gravity;
	public float Scale { get; set; } = scale;
	public bool Static { get; set; }
	public Vector2 StaticPos { get; set; }

	public void Update()
	{
		Vector2 delta = (Position - LastPosition) * Drag;

		LastPosition = Position;
		Position += delta;
		Position += new Vector2(0, Gravity);
	}

	public void SetStatic()
	{
		if (Static)
			Position = StaticPos;
	}

	public void StandardConstrain()
	{
	}

	public void Draw(SpriteBatch sB, Color color = default)
	{
		if (color == default)
		{
			if (Static)
				color = new Color(0, 255, 0);
			else
				color = Color.White;
		}

		sB.Draw(TextureAssets.MagicPixel.Value, Position - Main.screenPosition, null, color, 0f, new Vector2(0.5f), Scale, SpriteEffects.None, 0);
	}
}
