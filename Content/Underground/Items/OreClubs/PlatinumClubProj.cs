using SpiritReforged.Common.Easing;
using SpiritReforged.Common.PrimitiveRendering;
using SpiritReforged.Common.PrimitiveRendering.CustomTrails;
using SpiritReforged.Common.ProjectileCommon.Abstract;
using static SpiritReforged.Common.Easing.EaseFunction;
using static Microsoft.Xna.Framework.MathHelper;
using System.IO;

namespace SpiritReforged.Content.Underground.Items.OreClubs;

class PlatinumClubProj : BaseClubProj, ITrailProjectile
{
	private bool _inputHeld = true;

	public PlatinumClubProj() : base(new Vector2(84)) { }

	public override float HoldAngle_Intial => Pi * 2.4f;
	public override float HoldAngle_Final => -base.HoldAngle_Final / 2;
	public override float WindupTimeRatio => 0.6f;
	public override float PullbackWindupRatio => 0.7f;
	public override float LingerTimeRatio => 0.7f;

	public override bool? CanDamage() => (GetWindupProgress < 0.5f || CheckAiState(AiStates.SWINGING)) ? null : false;

	public void DoTrailCreation(TrailManager tM)
	{
		float trailDist = 76;
		float trailWidth = 50;
		static float windupSwingProgress(Projectile proj) => (proj.ModProjectile is BaseClubProj club) ? EaseCubicInOut.Ease(EaseCircularOut.Ease(Lerp(club.GetWindupProgress, 0, club.PullbackWindupRatio))) : 0;

		Func<Projectile, float> uSwingFunc = CheckAiState(AiStates.SWINGING) ? GetSwingProgressStatic : windupSwingProgress;
		int swingDirection = CheckAiState(AiStates.SWINGING) ? 1 : -1;
		float uRange = AngleRange;
		float uRot = HoldAngle_Final;
		float dissolveThreshold = 0.9f;

		if (CheckAiState(AiStates.CHARGING))
		{
			trailWidth *= 0.8f;
			trailDist *= 0.7f;
			uRot = HoldAngle_Intial - PiOver4;
			uRange = Math.Abs(HoldAngle_Final - HoldAngle_Intial);
			dissolveThreshold *= EaseCubicInOut.Ease(EaseCircularOut.Ease(1 - PullbackWindupRatio));
		}

		tM.CreateCustomTrail(new SwingTrail(Projectile, Color.White, 2, swingDirection * uRange, uRot, trailDist, trailWidth, uSwingFunc, SwingTrail.BasicSwingShaderParams, TrailLayer.UnderProjectile, dissolveThreshold));

		if (FullCharge)
			tM.CreateCustomTrail(new SwingTrail(Projectile, Color.White, 2, AngleRange, HoldAngle_Final, 1.2f * trailDist, trailWidth * 1.2f, GetSwingProgressStatic, SwingTrail.BasicSwingShaderParams));
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		if (CheckAiState(AiStates.CHARGING))
		{
			if (target.knockBackResist > 0 && target.gravity != 0)
				target.velocity.Y = -Projectile.knockBack * 0.75f;

			target.velocity.Y -= Projectile.knockBack * target.knockBackResist * 0.25f;
			target.velocity.X -= Projectile.knockBack * Projectile.direction * target.knockBackResist * 0.8f;
		}
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo info)
	{
		if(CheckAiState(AiStates.CHARGING) && !target.noKnockback)
			target.velocity.Y -= Projectile.knockBack;
	}

	public override void Charging(Player owner)
	{
		if(!_inputHeld)
		{
			TrailManager.TryTrailKill(Projectile);
			_lingerTimer -= 2;
			float lingerProgress = _lingerTimer / (float)LingerTime;

			Projectile.scale = Lerp(Projectile.scale, 0, EaseCircularOut.Ease(lingerProgress) / 4);

			if (_lingerTimer <= 0)
				Projectile.Kill();

			BaseRotation -= 0.1f * EaseCircularOut.Ease(lingerProgress);
		}

		else
			base.Charging(owner);
	}

	internal override void WindupComplete(Player owner)
	{
		_inputHeld = owner.controlUseItem;
		Projectile.netUpdate = true;
	}

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

	internal override float ChargedRotationInterpolate(float progress) => Lerp(HoldAngle_Intial, HoldAngle_Final, EaseCubicInOut.Ease(EaseCircularOut.Ease(progress)));

	internal override float ChargedScaleInterpolate(float progress) => Lerp(0.2f, 1f, EaseCircularOut.Ease(progress));

	public override void Swinging(Player owner) => base.Swinging(owner);

	internal override bool AllowUseTurn => CheckAiState(AiStates.CHARGING) && GetWindupProgress >= 1 && _inputHeld;
	internal override bool AllowRelease => _inputHeld;

	public override bool PreDrawExtras()
	{
		if (CheckAiState(AiStates.CHARGING) && GetWindupProgress < 1)
			DrawAftertrail(Lighting.GetColor(Projectile.Center.ToTileCoordinates()) * EaseCubicOut.Ease(1 - GetWindupProgress));

		return true;
	}

	internal override void SendExtraDataSafe(BinaryWriter writer) => writer.Write(_inputHeld);

	internal override void ReceiveExtraDataSafe(BinaryReader reader) => _inputHeld = reader.ReadBoolean();
}
