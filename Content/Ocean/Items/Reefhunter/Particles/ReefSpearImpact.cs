using SpiritReforged.Common.Easing;
using SpiritReforged.Content.Particles;

namespace SpiritReforged.Content.Ocean.Items.Reefhunter.Particles;

public class ReefSpearImpact(Entity entity, Vector2 basePosition, Vector2 velocity, float width, float length, float rotation, int maxTime, float taperExponent, int detatchTime = -1) : MotionNoiseCone(entity, basePosition, velocity, width, length, rotation, maxTime, detatchTime)
{
	internal override bool UseLightColor => true;
	private readonly float _taperExponent = taperExponent;
	internal override float GetScroll() => 2 * EaseFunction.EaseQuadOut.Ease(Progress);

	internal override Color BrightColor => new(251, 204, 62, 220);
	internal override Color DarkColor => new(230, 27, 112, 220);

	internal override void DissipationStyle(ref float dissipationProgress, ref float finalExponent, ref float xCoordExponent)
	{
		dissipationProgress = (float)Math.Max(1.33f * (Progress - 0.25f), 0);
		finalExponent = 2;
		xCoordExponent = 1.2f;
	}

	internal override float ColorLerpExponent => 1.5f;

	internal override int NumColors => 12;

	internal override float FinalIntensity => 1.4f;

	internal override void TaperStyle(ref float totalTapering, ref float taperExponent)
	{
		totalTapering = 1;
		taperExponent = _taperExponent;
	}

	internal override void TextureExponent(ref float minExponent, ref float maxExponent, ref float lerpExponent)
	{
		minExponent = 0.01f;
		maxExponent = 30f;
		lerpExponent = 2.25f;
	}

	internal override void XDistanceFade(ref float centeredPosition, ref float exponent)
	{
		centeredPosition = 0.15f;
		exponent = 2f;
	}
}
