using SpiritReforged.Common.Easing;
using SpiritReforged.Common.Particle;
using SpiritReforged.Common.PrimitiveRendering.PrimitiveShape;
using SpiritReforged.Common.PrimitiveRendering;
using SpiritReforged.Common.Misc;

namespace SpiritReforged.Content.Particles;

public class LightningParticle : Particle
{
	private Vector2 _endPosition;

	public override ParticleDrawType DrawType => ParticleDrawType.Custom;

	public LightningParticle(Vector2 positionStart, Vector2 positionEnd, Color color, int maxTime, float scale)
	{
		Position = positionStart;
		_endPosition = positionEnd;
		Color = color;
		MaxTime = maxTime;
		Scale = scale;
	}

	public override void CustomDraw(SpriteBatch spriteBatch)
	{
		float length = Vector2.Distance(Position, _endPosition);
		float rotation = (_endPosition - Position).ToRotation();
		var drawColor = Color.Lerp(Color.White, Color, EaseFunction.EaseQuadOut.Ease(Progress)).Additive();

		Effect beamEffect = AssetLoader.LoadedShaders["Lightning"];
		beamEffect.Parameters["uTexture"].SetValue(AssetLoader.LoadedTextures["Lightning"]);
		beamEffect.Parameters["perlinNoise"].SetValue(AssetLoader.LoadedTextures["noise"]);
		beamEffect.Parameters["Progress"].SetValue(Progress);
		beamEffect.Parameters["uTime"].SetValue(EaseFunction.EaseQuadOut.Ease(Progress) / 3 + (float)Main.time / 90);
		beamEffect.Parameters["xMod"].SetValue(length / 120f);
		beamEffect.Parameters["yMod"].SetValue(MathHelper.Lerp(1f, 5f, EaseFunction.EaseQuadOut.Ease(Progress)));
		beamEffect.CurrentTechnique.Passes[0].Apply();

		var beam = new SquarePrimitive
		{
			Height = Scale * MathHelper.Lerp(1, 0.2f, EaseFunction.EaseCubicIn.Ease(Progress)),
			Length = length,
			Rotation = rotation,
			Position = Vector2.Lerp(Position, _endPosition, 0.5f) - Main.screenPosition,
			Color = drawColor
		};

		PrimitiveRenderer.DrawPrimitiveShape(beam, beamEffect);
	}
}
