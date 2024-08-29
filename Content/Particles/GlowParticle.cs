using SpiritReforged.Common.Particle;

namespace SpiritReforged.Content.Particles;

public class GlowParticle : Particle
{
	private readonly Color _startColor;
	private readonly Color _endColor;
	private readonly int _maxTime;

	private const float FADETIME = 0.3f;

	public delegate void UpdateAction(Particle particle);

	private readonly UpdateAction _action;

	public override ParticleDrawType DrawType => ParticleDrawType.Custom;

	public GlowParticle(Vector2 position, Vector2 velocity, Color startColor, Color endColor, float scale, int maxTime, UpdateAction action = null)
	{
		Position = position;
		Velocity = velocity;
		_startColor = startColor;
		_endColor = endColor;
		Scale = scale;
		_maxTime = maxTime;
		_action = action;
	}
	public GlowParticle(Vector2 position, Vector2 velocity, Color color, float scale, int maxTime, UpdateAction action = null)
	{
		Position = position;
		Velocity = velocity;
		_startColor = color;
		_endColor = color;
		Scale = scale;
		_maxTime = maxTime;
		_action = action;
	}

	public override void Update()
	{
		float fadeintime = _maxTime * FADETIME;
		Color = Color.Lerp(_startColor, _endColor, TimeActive / (float)_maxTime);
		if (TimeActive < fadeintime)
			Color *= TimeActive / fadeintime;

		else if(TimeActive > _maxTime - fadeintime)
			Color *= (_maxTime - TimeActive) / fadeintime;

		Lighting.AddLight(Position, Color.ToVector3() * Scale * 0.5f);

		_action?.Invoke(this);

		if (TimeActive > _maxTime)
			Kill();
	}

	public override void CustomDraw(SpriteBatch spriteBatch)
	{
		Texture2D tex = ParticleHandler.GetTexture(Type);
		Texture2D bloom = ModContent.Request<Texture2D>("SpiritReforged/Assets/Textures/Bloom", AssetRequestMode.ImmediateLoad).Value;
		Color additiveFix = Color;
		additiveFix.A = 0;

		spriteBatch.Draw(bloom, Position - Main.screenPosition, null, additiveFix * 0.6f, 0, bloom.Size() / 2, Scale / 5f, SpriteEffects.None, 0);
		spriteBatch.Draw(tex, Position - Main.screenPosition, null, additiveFix, 0, tex.Size() / 2, Scale, SpriteEffects.None, 0);
	}
}
