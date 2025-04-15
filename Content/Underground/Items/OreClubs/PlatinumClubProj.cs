using SpiritReforged.Common.Easing;
using SpiritReforged.Common.PrimitiveRendering;
using SpiritReforged.Common.PrimitiveRendering.CustomTrails;
using SpiritReforged.Common.ProjectileCommon.Abstract;
using static SpiritReforged.Common.Easing.EaseFunction;
using static Microsoft.Xna.Framework.MathHelper;

namespace SpiritReforged.Content.Underground.Items.OreClubs;

class PlatinumClubProj : BaseClubProj, ITrailProjectile
{
	public PlatinumClubProj() : base(new Vector2(84)) { }

	public override float HoldAngle_Intial => Pi * 2.4f;
	public override float HoldAngle_Final => -base.HoldAngle_Final * 2;
	public override float WindupTimeRatio => 1.6f;
	public override float PullbackWindupRatio => 0.3f;
	public override float LingerTimeRatio => 0.7f;

	public override bool? CanDamage() => (GetWindupProgress < 0.5f || CheckAiState(AiStates.SWINGING)) ? null : false;

	public void DoTrailCreation(TrailManager tM)
	{
		float trailDist = 76;
		float trailWidth = 50;
		static float windupSwingProgress(Projectile proj) => (proj.ModProjectile is BaseClubProj club) ? EaseQuadInOut.Ease(EaseQuadOut.Ease(club.GetWindupProgress)) : 0;

		Func<Projectile, float> uSwingFunc = CheckAiState(AiStates.SWINGING) ? GetSwingProgressStatic : windupSwingProgress;
		int swingDirection = CheckAiState(AiStates.SWINGING) ? 1 : -1;
		float uRange = AngleRange;

		if (CheckAiState(AiStates.CHARGING))
		{
			trailWidth *= 0.8f;
			trailDist *= 0.9f;
			uRange = Math.Abs(HoldAngle_Final - HoldAngle_Intial);
		}
		
		tM.CreateCustomTrail(new SwingTrail(Projectile, Color.Silver, 2, swingDirection * AngleRange, HoldAngle_Final, trailDist, trailWidth, uSwingFunc, SwingTrail.BasicSwingShaderParams));

		if (FullCharge)
			tM.CreateCustomTrail(new SwingTrail(Projectile, Color.Silver, 2, AngleRange, HoldAngle_Final, 1.2f * trailDist, trailWidth * 1.2f, GetSwingProgressStatic, SwingTrail.BasicSwingShaderParams));
	}

	public override void Swinging(Player owner) => base.Swinging(owner);

	internal override float ChargedRotationInterpolate(float progress) => Lerp(HoldAngle_Intial, HoldAngle_Final, EaseQuadInOut.Ease(EaseCircularOut.Ease(progress)));

	internal override float ChargedScaleInterpolate(float progress) => Lerp(0.25f, 1f, EaseCircularOut.Ease(progress));

	internal override bool AllowUseTurn => CheckAiState(AiStates.CHARGING) && GetWindupProgress >= 1;

	public override void OnSwingStart()
	{
		TrailManager.TryTrailKill(Projectile);
		Projectile.ResetLocalNPCHitImmunity();
		TrailManager.ManualTrailSpawn(Projectile);

		Player owner = Main.player[Projectile.owner];
		int tempDirection = owner.direction;
		if (owner == Main.LocalPlayer)
		{
			int newDir = Math.Sign(Main.MouseWorld.X - owner.Center.X);
			Projectile.velocity.X = newDir == 0 ? owner.direction : newDir;

			if (newDir != owner.direction)
				Projectile.netUpdate = true;
		}

		owner.ChangeDir((int)Projectile.velocity.X);

		if (tempDirection != owner.direction)
			for (int i = 0; i < Projectile.oldRot.Length; i++)
				Projectile.oldRot[i] = Projectile.oldRot[i] + PiOver2;
	}

	public override void OnSmash(Vector2 position)
	{
		TrailManager.TryTrailKill(Projectile);
		Collision.HitTiles(Projectile.position, Vector2.UnitY, Projectile.width, Projectile.height);

		DustClouds(10);

		if (FullCharge)
		{
			float angle = PiOver4 * 1.25f;
			if (Projectile.direction > 0)
				angle = -angle + Pi;

			DoShockwaveCircle(Vector2.Lerp(Projectile.Center, Main.player[Projectile.owner].Center, 0.5f), 380, angle, 0.4f);
		}

		DoShockwaveCircle(Projectile.Bottom - Vector2.UnitY * 8, 240, PiOver2, 0.4f);
	}

	public override bool PreDrawExtras()
	{
		if (CheckAiState(AiStates.CHARGING) && GetWindupProgress < 1)
			DrawAftertrail(Lighting.GetColor(Projectile.Center.ToTileCoordinates()));

		return true;
	}
}
