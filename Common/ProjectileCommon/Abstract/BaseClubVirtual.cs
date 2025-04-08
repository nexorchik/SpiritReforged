using SpiritReforged.Common.Easing;
using Terraria.Audio;
using Terraria.Graphics.CameraModifiers;
using static Microsoft.Xna.Framework.MathHelper;
using static SpiritReforged.Common.Easing.EaseFunction;

namespace SpiritReforged.Common.ProjectileCommon.Abstract;

public abstract partial class BaseClubProj : ModProjectile
{
	public virtual float HoldAngle_Intial => PiOver2;
	public virtual float HoldAngle_Final => PiOver4 / 4;
	public virtual float SwingAngle_Max => Pi * 1.75f;

	public virtual float WindupTimeRatio => 0.5f;
	public virtual float LingerTimeRatio => 0.75f;
	public virtual float HoldPointRatio => 0.1f;

	public virtual float SwingSpeedMult => Charge == 1 ? 1.2f : 1f;

	public virtual void Charging(Player owner)
	{
		const float PULLBACK_WINDUP_RATIO = 0.33f;

		//Useturn logic only during charge
		if (owner == Main.LocalPlayer)
		{
			int newDir = Math.Sign(Main.MouseWorld.X - owner.Center.X);
			Projectile.velocity.X = newDir == 0 ? owner.direction : newDir;

			if (newDir != owner.direction)
				Projectile.netUpdate = true;
		}

		owner.ChangeDir((int)Projectile.velocity.X);

		if (_windupTimer < WindupTime)
			_windupTimer++;
		else
		{
			Charge += 1f / ChargeTime;
			Charge = Min(Charge, 1);
		}

		if (Charge == 1 && !_hasFlickered)
		{
			SoundEngine.PlaySound(SoundID.NPCDeath7, Projectile.Center);
			_flickerTime = MAX_FLICKERTIME;
			_hasFlickered = true;
			Projectile.netUpdate = true;
		}

		float windupAnimProgress = _windupTimer / (float)WindupTime;
		windupAnimProgress = Lerp(windupAnimProgress, Charge, PULLBACK_WINDUP_RATIO);

		BaseRotation = Lerp(HoldAngle_Intial, HoldAngle_Final, EaseCircularOut.Ease(windupAnimProgress));
		Projectile.scale = Lerp(0f, 1f, EaseCubicOut.Ease(EaseCircularOut.Ease(windupAnimProgress)));

		--_flickerTime;
	}

	public virtual void Swinging(Player owner)
	{
		const float PHASE_THRESHOLD = 0.25f;
		const float SHRINK_THRESHOLD = 0.5f;

		float getSwingProgress() => SwingSpeedMult * _swingTimer / SwingTime;
		float swingProgress = getSwingProgress();

		bool validTile = Collision.SolidTiles(Projectile.position, Projectile.width, Projectile.height, true);
		Projectile.scale = 1;

		_swingTimer++;

		EaseFunction swingEase = EaseQuadOut;
		BaseRotation = Lerp(HoldAngle_Final, SwingAngle_Max, swingEase.Ease(swingProgress));

		//If the club is touching a tile and isn't currently meant to phase through tiles, do the smash
		if (validTile && swingProgress > PHASE_THRESHOLD)
		{
			SetAiState(AiStates.POST_SMASH);
			OnSmash(Projectile.Center);
			if (!Main.dedServ)
			{
				float volume = Clamp(EaseQuadOut.Ease(Charge), 0.75f, 1f);
				SoundEngine.PlaySound(SoundID.Item70.WithVolumeScale(volume), Projectile.Center);
				SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact.WithVolumeScale(volume).WithPitchOffset(-0.5f), Projectile.Center);

				if (Main.LocalPlayer == owner)
					Main.instance.CameraModifiers.Add(new PunchCameraModifier(Main.screenPosition, Vector2.Normalize(Projectile.oldPosition - Projectile.position), 1 + Charge * 2, 6, (int)(20 * (0.5f + Charge / 2))));
			}
		}

		if (swingProgress >= SHRINK_THRESHOLD)
		{
			float shrinkProgress = (swingProgress - SHRINK_THRESHOLD) / (1 - SHRINK_THRESHOLD);
			shrinkProgress = Clamp(shrinkProgress, 0, 1);

			Projectile.scale = Lerp(1, 0, EaseCubicIn.Ease(shrinkProgress));

			if (swingProgress > 1)
				Projectile.Kill();
		}
	}

	public virtual void AfterCollision()
	{
		const float shrinkThreshold = 0.6f;

		_lingerTimer--;
		float lingerProgress = _lingerTimer / (float)LingerTime;
		lingerProgress = 1 - lingerProgress;

		float shrinkProgress = (lingerProgress - shrinkThreshold) / (1 - shrinkThreshold);
		shrinkProgress = Clamp(shrinkProgress, 0, 1);

		Projectile.scale = Lerp(1, 0, EaseCubicOut.Ease(EaseCircularIn.Ease(shrinkProgress)));

		if (_lingerTimer <= 0)
			Projectile.Kill();

		BaseRotation += Lerp(-0.05f, 0.05f, EaseQuadIn.Ease(lingerProgress)) * (1 + Charge / 2);
	}

	public virtual void SafeSetStaticDefaults() { }
	public virtual void SafeSetDefaults() { }
	public virtual void SafeAI() { }
	public virtual void OnSmash(Vector2 position) { }
	public virtual void SafeDraw(SpriteBatch spriteBatch, Color lightColor) { }

	public virtual SpriteEffects Effects => Main.player[Projectile.owner].direction * (int)Main.player[Projectile.owner].gravDir < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
	public virtual Vector2 HoldPoint => Effects == SpriteEffects.FlipHorizontally ? Size * (1 - HoldPointRatio) : new Vector2(Size.X * HoldPointRatio, Size.Y * (1 - HoldPointRatio));
}