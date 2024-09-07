using SpiritReforged.Common.Easing;
using SpiritReforged.Common.Particle;

namespace SpiritReforged.Content.Ocean.Items.Reefhunter.Particles;

public class BubbleParticle : Particle
{
	private readonly int _maxTime;
	private readonly float _maxScale;
	private readonly Vector2 _initialVel;

	public BubbleParticle(Vector2 position, Vector2 velocity, float scale, int lifetime)
	{
		Position = position;
		Scale = scale;
		_maxScale = scale;
		_maxTime = lifetime;
		Velocity = velocity;
		_initialVel = velocity;
		Color = Lighting.GetColor(Position.ToTileCoordinates().X, Position.ToTileCoordinates().Y) * 0.8f;
		Origin = ParticleHandler.GetTexture(Type).Size() / 2;
	}

	public override ParticleDrawType DrawType => ParticleDrawType.NonPremultiplied;

	public override void Update()
	{
		float progress = TimeActive / (float)_maxTime;
		Scale = MathHelper.Lerp(_maxScale, 0, EaseFunction.EaseCircularIn.Ease(progress));
		Velocity = Vector2.Lerp(_initialVel, new Vector2(Main.windSpeedCurrent, -1), EaseFunction.EaseQuadOut.Ease(progress) / 2) * (1 - EaseFunction.EaseCubicOut.Ease(progress) / 4);
		if (TimeActive > _maxTime)
			Kill();
	}
}
