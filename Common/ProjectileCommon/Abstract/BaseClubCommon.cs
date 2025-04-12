using SpiritReforged.Common.Particle;
using SpiritReforged.Common.PrimitiveRendering.CustomTrails;
using SpiritReforged.Content.Forest.WoodClub;
using SpiritReforged.Content.Particles;
using static Microsoft.Xna.Framework.MathHelper;
using static SpiritReforged.Common.Easing.EaseFunction;

namespace SpiritReforged.Common.ProjectileCommon.Abstract;

public abstract partial class BaseClubProj : ModProjectile
{
	private enum AiStates
	{
		CHARGING,
		SWINGING,
		POST_SMASH
	}

	private bool CheckAiState(AiStates checkState) => AiState == (float)checkState;

	private void SetAiState(AiStates setState)
	{
		AiState = (float)setState;
		Projectile.netUpdate = true;
	}

	public void SetStats(int chargeTime, int swingTime, float damageScaling, float knockbackScaling)
	{
		ChargeTime = chargeTime;
		SwingTime = swingTime;
		DamageScaling = damageScaling;
		KnockbackScaling = knockbackScaling;

		_lingerTimer = LingerTime;
		_swingTimer = 0;
		_windupTimer = 0;
		_flickerTime = 0;
	}

	private void TranslateRotation(Player owner, out float clubRotation, out float armRotation)
	{
		float output = BaseRotation * owner.gravDir + 1.7f;
		float clubOffset = Pi + PiOver4;
		if (owner.direction < 0)
		{
			output = TwoPi - output;
			clubOffset -= PiOver2;
		}

		clubRotation = output - clubOffset;
		armRotation = output;
	}

	internal static void DoShockwaveCircle(Vector2 pos, float size, float xyRotation, float opacity)
	{
		var easeFunction = EaseCubicOut;
		float ringWidth = 0.4f;
		int lifetime = 24;
		float zRotation = 0.85f;

		ParticleHandler.SpawnParticle(new TexturedPulseCircle(
			pos,
			Color.LightGray * opacity,
			Color.LightGray * opacity,
			ringWidth,
			size,
			lifetime,
			"supPerlin",
			new Vector2(2, 3),
			easeFunction).WithSkew(zRotation, xyRotation).UsesLightColor());
	}

	internal void DustClouds(int maxClouds)
	{
		float chargeFactor = Clamp(EaseQuadIn.Ease(Charge), 0.33f, 1f);
		float chargeFactorLerped = Lerp(chargeFactor, 1, 0.5f);

		for (int i = 0; i < maxClouds * chargeFactorLerped; i++)
		{
			Vector2 smokePos = Projectile.Bottom + Vector2.UnitX * Main.rand.NextFloat(-20, 20);

			float scale = Main.rand.NextFloat(0.05f, 0.07f);
			scale *= 1 + chargeFactor / 2;

			float speed = Main.rand.NextFloat(4);
			speed *= chargeFactorLerped;
			speed += 0.5f;

			int lifeTime = (int)(40 + Main.rand.Next(-15, 16) * (1 + chargeFactor));

			ParticleHandler.SpawnParticle(new SmokeCloud(smokePos, -Vector2.UnitY * speed, Color.LightGray * 0.75f, scale, EaseQuadOut, lifeTime));
		}
	}

	public float GetSwingProgress => SwingSpeedMult * _swingTimer / SwingTime;

	public static float GetSwingProgressStatic(Projectile Proj) => Proj.ModProjectile is WoodClubProj woodClub ? EaseQuadOut.Ease(woodClub.GetSwingProgress) : 0;

	public static void BasicSwingShaderParams(Effect effect, SwingTrail swingTrail)
	{
		effect = AssetLoader.LoadedShaders["SwingTrails"];
		effect.Parameters["baseTexture"].SetValue(AssetLoader.LoadedTextures["supPerlin"].Value);
		effect.Parameters["baseColorLight"].SetValue(swingTrail.Color.ToVector4());
		effect.Parameters["baseColorDark"].SetValue(swingTrail.Color.ToVector4());

		effect.Parameters["coordMods"].SetValue(new Vector2(0.7f, 1f));
		effect.Parameters["textureExponent"].SetValue(new Vector2(0.5f, 1.25f));

		effect.Parameters["timer"].SetValue(Main.GlobalTimeWrappedHourly);
		effect.Parameters["progress"].SetValue(swingTrail.GetSwingProgress());
		effect.Parameters["intensity"].SetValue(1.25f);
		effect.Parameters["opacity"].SetValue(1);
	}

	public float AngleRange => SwingAngle_Max - HoldAngle_Final;

	public bool FullCharge => Charge == 1;
}