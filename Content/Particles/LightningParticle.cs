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
		var drawColor = Color.Lerp(Color.White, Color, EaseFunction.EaseQuadOut.Ease(progress));
		drawColor.A = 0;

		//Draw the beam and apply shader parameters
		Effect beamEffect = AssetLoader.LoadedShaders["Lightning"];
		beamEffect.Parameters["uTexture"].SetValue(ModContent.Request<Texture2D>("SpiritReforged/Assets/Textures/Lightning").Value);
		beamEffect.Parameters["perlinNoise"].SetValue(ModContent.Request<Texture2D>("SpiritReforged/Assets/Textures/noise").Value);
		beamEffect.Parameters["Progress"].SetValue(progress);
		beamEffect.Parameters["uTime"].SetValue(EaseFunction.EaseQuadOut.Ease(progress) / 3 + (float)Main.time / 90);
		beamEffect.Parameters["xMod"].SetValue(length / 120f);
		beamEffect.Parameters["yMod"].SetValue(MathHelper.Lerp(1f, 5f, EaseFunction.EaseQuadOut.Ease(progress)));
		beamEffect.CurrentTechnique.Passes[0].Apply();

		var beam = new SquarePrimitive
		{
			Height = Scale * MathHelper.Lerp(1, 0.2f, EaseFunction.EaseCubicIn.Ease(progress)),
			Length = length,
			Rotation = rotation,
			Position = Vector2.Lerp(Position, _endPosition, 0.5f) - Main.screenPosition,
			Color = drawColor
		};

		PrimitiveRenderer.DrawPrimitiveShape(beam, beamEffect);
	}
}
