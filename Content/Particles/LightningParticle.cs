using SpiritReforged.Common.Easing;
using SpiritReforged.Common.Particle;
using SpiritReforged.Common.PrimitiveRendering.PrimitiveShape;
using SpiritReforged.Common.PrimitiveRendering;

namespace SpiritReforged.Content.Particles;

public class LightningParticle : Particle
{
	private readonly int _maxTime;
	private Vector2 _endPosition;
	private Vector2[] _deviationPoints;

	public override ParticleDrawType DrawType => ParticleDrawType.Custom;

	public LightningParticle(Vector2 positionStart, Vector2 positionEnd, Color color, int maxTime, float scale)
	{
		Position = positionStart;
		_endPosition = positionEnd;
		Color = color;
		_maxTime = maxTime;
		Scale = scale;
	}

	public override void Update()
	{
		if (TimeActive > _maxTime)
			Kill();
	}

	public override void CustomDraw(SpriteBatch spriteBatch)
	{
		float length = Vector2.Distance(Position, _endPosition);
		float rotation = (_endPosition - Position).ToRotation();
		float progress = TimeActive / (float)_maxTime;
		var drawColor = Color.Lerp(Color.White, Color, EaseFunction.EaseQuadOut.Ease(progress) * 0.75f);

		//Draw the beam and apply shader parameters
		Effect beamEffect = ModContent.Request<Effect>("SpiritReforged/Assets/Shaders/Lightning", AssetRequestMode.ImmediateLoad).Value;
		beamEffect.Parameters["uTexture"].SetValue(ModContent.Request<Texture2D>("SpiritReforged/Assets/Textures/Lightning").Value);
		beamEffect.Parameters["perlinNoise"].SetValue(ModContent.Request<Texture2D>("SpiritReforged/Assets/Textures/noise").Value);
		beamEffect.Parameters["Progress"].SetValue(progress);
		beamEffect.Parameters["uTime"].SetValue((float)Main.time / 30f);
		beamEffect.Parameters["xMod"].SetValue(length / 150f);
		beamEffect.Parameters["yMod"].SetValue(5f);
		beamEffect.CurrentTechnique.Passes[0].Apply();

		var beam = new SquarePrimitive
		{
			Height = Scale * (0.5f + EaseFunction.EaseCubicIn.Ease(1 - progress) / 2),
			Length = length,
			Rotation = rotation,
			Position = Vector2.Lerp(Position, _endPosition, 0.5f) - Main.screenPosition,
			Color = drawColor
		};

		PrimitiveRenderer.DrawPrimitiveShape(beam, beamEffect);
	}
}
