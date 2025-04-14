using Microsoft.Xna.Framework.Graphics;
using SpiritReforged.Common.Particle;
using SpiritReforged.Common.PrimitiveRendering.CustomTrails;
using SpiritReforged.Content.Forest.WoodClub;
using SpiritReforged.Content.Particles;
using SpiritReforged.Content.Underground.Items.OreClubs;
using static Microsoft.Xna.Framework.MathHelper;
using static SpiritReforged.Common.Easing.EaseFunction;

namespace SpiritReforged.Common.ProjectileCommon.Abstract;

public abstract partial class BaseClubProj : ModProjectile
{
	public enum AiStates
	{
		CHARGING,
		SWINGING,
		POST_SMASH
	}

	public bool CheckAiState(AiStates checkState) => AiState == (float)checkState;

	public void SetAiState(AiStates setState)
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

	internal void DrawAftertrail(Color lightColor)
	{
		Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
		Player owner = Main.player[Projectile.owner];
		Vector2 handPos = owner.GetFrontHandPosition(owner.compositeFrontArm.stretch, owner.compositeFrontArm.rotation);
		Vector2 drawPos = handPos - Main.screenPosition + Vector2.UnitY * owner.gfxOffY;
		Color drawColor = Projectile.GetAlpha(lightColor);

		Rectangle topFrame = texture.Frame(1, Main.projFrames[Type]);

		for (int k = 0; k < Projectile.oldPos.Length; k++)
		{
			//Don't draw if rotation would be the default value
			if (CheckAiState(AiStates.CHARGING) && Projectile.oldRot[k] == 0)
				continue;

			float progress = (Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length;
			Color trailColor = drawColor * progress * .125f;
			Main.EntitySpriteDraw(texture, drawPos, topFrame, trailColor, Projectile.oldRot[k], HoldPoint, Projectile.scale, Effects, 0);
		}
	}

	public float GetSwingProgress => SwingSpeedMult * _swingTimer / SwingTime;

	public float GetWindupProgress => (ChargeTime > 0) ? _windupTimer / (float)WindupTime : 0;

	public static float GetSwingProgressStatic(Projectile Proj) => Proj.ModProjectile is BaseClubProj baseClub ? EaseQuadOut.Ease(baseClub.GetSwingProgress) : 0;

	public float AngleRange => SwingAngle_Max - HoldAngle_Final;

	public bool FullCharge => Charge == 1;
}