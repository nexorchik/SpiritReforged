using SpiritReforged.Common.Easing;
using System.IO;
using Terraria.Audio;
using Terraria.Graphics.CameraModifiers;
using static Microsoft.Xna.Framework.MathHelper;
using static SpiritReforged.Common.Easing.EaseFunction;

namespace SpiritReforged.Common.ProjectileCommon;

public abstract class BaseClubProj(Vector2 size) : ModProjectile
{
	//Todo: make some of these changable per club rather than hardcoded constants
	private const int MAX_FLICKERTIME = 20;
	private const int MAX_LINGERTIME = 30;
	private const int MAX_SWINGTIME = 30;
	private const int WINDUP_TIME = 25;

	private const float INITIAL_HOLD_ANGLE = PiOver2;
	private const float FINAL_HOLD_ANGLE = PiOver4 / 2;
	private const float MAX_SWING_ANGLE = Pi * 1.33f;

	private static Vector2 SWING_PHASE_THRESHOLD = new(0.15f, 0.66f);

	internal readonly Vector2 Size = size;

	public int ChargeTime { get; private set; }

	public int MinDamage { get; private set; }
	public int MaxDamage { get; private set; }

	public float MinKnockback { get; private set; }
	public float MaxKnockback { get; private set; }

	public bool released = false;

	public float Charge { get => Projectile.ai[0]; set => Projectile.ai[0] = value; }
	public float BaseRotation { get => Projectile.ai[2]; set => Projectile.ai[2] = value; }
	public bool HasFlickered { get => Projectile.ai[1] == 1; set => Projectile.ai[1] = value ? 1 : 0; }

	protected int _lingerTimer = 0;
	protected int _flickerTime = 0;
	protected int _swingTimer = 0;
	protected int _windupTimer = 0;
	private bool _collided = false;

	public void SetStats(int chargeTime, int minDamage, int maxDamage, float minKnockback, float maxKnockback)
	{
		ChargeTime = chargeTime;
		MinDamage = minDamage;
		MaxDamage = maxDamage;
		MinKnockback = minKnockback;
		MaxKnockback = maxKnockback;
	}

	public sealed override void SetStaticDefaults()
	{
		ProjectileID.Sets.TrailCacheLength[Type] = 4;
		ProjectileID.Sets.TrailingMode[Type] = 2;

		SafeSetStaticDefaults();
	}

	public sealed override void SetDefaults()
	{
		Projectile.hostile = false;
		Projectile.DamageType = DamageClass.Melee;
		Projectile.width = Projectile.height = 16;
		Projectile.aiStyle = -1;
		Projectile.friendly = true;
		Projectile.penetrate = -1;
		Projectile.tileCollide = false;
		Projectile.ownerHitCheck = true;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = -1;

		SafeSetDefaults();
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Main.player[Projectile.owner].Center, Projectile.Center) ? true : base.Colliding(projHitbox, targetHitbox);

	public override bool? CanDamage() => _lingerTimer == 0 && released ? null : false;

	public sealed override void AI()
	{
		SafeAI();

		Player owner = Main.player[Projectile.owner];

		if (owner.dead)
			Projectile.Kill();

		Projectile.scale = 1;
		owner.heldProj = Projectile.whoAmI;

		if (!released)
			Charging(owner);
		else
			Swinging(owner);

		if (!owner.channel && _windupTimer >= WINDUP_TIME)
		{
			released = true;
			HasFlickered = false;
		}

		--_flickerTime;

		TranslateRotation(owner, out float clubRotation, out float armRotation);
		Projectile.rotation = clubRotation;

		owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.ThreeQuarters, armRotation);
		owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.ThreeQuarters, armRotation);
		Projectile.position.X = owner.Center.X - (int)(Math.Cos(armRotation - PiOver2) * Size.X) - Projectile.width / 2;
		Projectile.position.Y = owner.Center.Y - (int)(Math.Sin(armRotation - PiOver2) * Size.Y) - Projectile.height / 2 - owner.gfxOffY;

		owner.itemAnimation = owner.itemTime = 2;
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

	private void Charging(Player owner)
	{
		if (owner == Main.LocalPlayer)
		{
			int newDir = Math.Sign(Main.MouseWorld.X - owner.Center.X);
			Projectile.velocity.X = newDir == 0 ? owner.direction : newDir;

			if (newDir != owner.direction)
				Projectile.netUpdate = true;
		}

		owner.ChangeDir((int)Projectile.velocity.X);

		if (_windupTimer < WINDUP_TIME)
			_windupTimer++;
		else
		{
			Charge += 1f / ChargeTime;
			Charge = Min(Charge, 1);
		}

		if(Charge == 1 && !HasFlickered)
		{
			SoundEngine.PlaySound(SoundID.NPCDeath7, Projectile.Center);
			_flickerTime = MAX_FLICKERTIME;
			HasFlickered = true;
		}

		Projectile.damage = (int)Lerp(MinDamage, MaxDamage, Charge);
		Projectile.knockBack = Lerp(MinKnockback, MaxKnockback, Charge);

		float windupAnimProgress = _windupTimer / (float)WINDUP_TIME;
		windupAnimProgress = Lerp(windupAnimProgress, Charge, 0.4f);

		BaseRotation = Lerp(INITIAL_HOLD_ANGLE, FINAL_HOLD_ANGLE, EaseCircularOut.Ease(windupAnimProgress));
		Projectile.scale = Lerp(0f, 1f, EaseCubicOut.Ease(EaseCircularOut.Ease(windupAnimProgress)));
		owner.headRotation = Lerp(0f, FINAL_HOLD_ANGLE, windupAnimProgress);
	}

	private void Swinging(Player owner)
	{
		owner.direction = Math.Sign(Projectile.velocity.X);
		float swingProgress() => _swingTimer / (float)MAX_SWINGTIME;

		bool validTile = Collision.SolidTiles(Projectile.position, Projectile.width, Projectile.height, true);
		Projectile.scale = 1;
		if (swingProgress() >= SWING_PHASE_THRESHOLD.Y && !_collided)
		{
			float shrinkProgress = (swingProgress() - SWING_PHASE_THRESHOLD.Y) / (1 - SWING_PHASE_THRESHOLD.Y);
			shrinkProgress = Clamp(shrinkProgress, 0, 1);
			Projectile.scale = Lerp(1, 0, EaseQuadIn.Ease(shrinkProgress));
		}

		_swingTimer++;
		if (swingProgress() >= 1)
			Projectile.Kill();

		if (_collided)
		{
			_lingerTimer--;
			float lingerProgress = _lingerTimer / (float)MAX_LINGERTIME;
			BaseRotation += Lerp(0f, 0.01f, EaseQuadIn.Ease(lingerProgress)) * (1 + Charge * 2);
			if (_lingerTimer <= 0)
				Projectile.Kill();
		}
		else
			BaseRotation = Lerp(FINAL_HOLD_ANGLE, MAX_SWING_ANGLE, EaseCircularOut.Ease(swingProgress()));

		if (validTile && !_collided && swingProgress() > SWING_PHASE_THRESHOLD.X && swingProgress() < SWING_PHASE_THRESHOLD.Y)
		{
			_collided = true;
			_lingerTimer = MAX_LINGERTIME;
			Smash(Projectile.Center);

			float volume = Clamp(EaseQuadOut.Ease(Charge), 0.75f, 1f);
			SoundEngine.PlaySound(SoundID.Item70.WithVolumeScale(volume), Projectile.Center);
			SoundEngine.PlaySound(SoundID.NPCHit42.WithVolumeScale(volume), Projectile.Center);

			Main.instance.CameraModifiers.Add(new PunchCameraModifier(Main.screenPosition, Vector2.Normalize(Projectile.oldPosition - Projectile.position), 1 + Charge * 2, 6, (int)(20 * (0.5f + Charge/2))));
		}
	}

	public virtual void SafeAI() { }
	public virtual void SafeDraw(SpriteBatch spriteBatch, Color lightColor) { }
	public virtual void SafeSetDefaults() { }
	public virtual void SafeSetStaticDefaults() { }
	public virtual void Smash(Vector2 position) { }

	public virtual SpriteEffects Effects => Main.player[Projectile.owner].direction * (int)Main.player[Projectile.owner].gravDir < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
	public virtual Vector2 Origin => Effects == SpriteEffects.FlipHorizontally ? Size : new Vector2(0, Size.Y);

	public sealed override bool PreDraw(ref Color lightColor)
	{
		DrawTrail(lightColor);

		Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
		Player owner = Main.player[Projectile.owner];
		Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, owner.Center - Main.screenPosition + Vector2.UnitY * owner.gfxOffY, texture.Frame(1, Main.projFrames[Type]), Projectile.GetAlpha(lightColor), Projectile.rotation, Origin, Projectile.scale, Effects, 0);

		SafeDraw(Main.spriteBatch, lightColor);

		if (Projectile.ai[0] >= 1 && !released && _flickerTime > 0)
		{
			float alpha = EaseQuadIn.Ease(EaseSine.Ease(_flickerTime / (float)MAX_FLICKERTIME));

			Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, owner.Center - Main.screenPosition + Vector2.UnitY * owner.gfxOffY, texture.Frame(1, Main.projFrames[Type], 0, 1, 0, 0), Projectile.GetAlpha(Color.White * alpha), Projectile.rotation, Origin, Projectile.scale, Effects, 0);
		}

		return false;
	}

	public virtual void DrawTrail(Color lightColor)
	{
		if (released && _lingerTimer == 0)
		{
			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = Main.player[Projectile.owner].Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
				Color trailColor = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length) * .5f;
				Main.EntitySpriteDraw(texture, drawPos, texture.Frame(1, Main.projFrames[Type]), trailColor, Projectile.oldRot[k], Origin, Projectile.scale, Effects, 0);
			}
		}
	}

	public override void SendExtraAI(BinaryWriter writer)
	{
		writer.Write(ChargeTime);

		writer.Write(_lingerTimer);
		writer.Write(released);
	}

	public override void ReceiveExtraAI(BinaryReader reader)
	{
		ChargeTime = reader.ReadInt32();

		_lingerTimer = reader.ReadInt32();
		released = reader.ReadBoolean();
	}
}