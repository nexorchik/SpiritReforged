using SpiritReforged.Common.Easing;
using SpiritReforged.Common.Particle;

namespace SpiritReforged.Content.Ocean.Items.Reefhunter.Particles;

public class BubbleParticle : Particle
{
	private readonly float _maxScale;
	private readonly Vector2 _initialVel;

	public BubbleParticle(Vector2 position, Vector2 velocity, float scale, int lifetime)
	{
		Position = position;
		Scale = scale;
		_maxScale = scale;
		MaxTime = lifetime;
		Velocity = velocity;
		_initialVel = velocity;
		Color = Lighting.GetColor(Position.ToTileCoordinates().X, Position.ToTileCoordinates().Y) * 0.8f;
	}

	public override ParticleDrawType DrawType => ParticleDrawType.BatchedAdditiveBlend;

	public override void Update()
	{
		Scale = MathHelper.Lerp(_maxScale, 0, EaseFunction.EaseCircularIn.Ease(Progress));
		Velocity = Vector2.Lerp(_initialVel, new Vector2(Main.windSpeedCurrent, -1), EaseFunction.EaseQuadOut.Ease(Progress) / 2) * (1 - EaseFunction.EaseCubicOut.Ease(Progress) / 4);
	}
}
