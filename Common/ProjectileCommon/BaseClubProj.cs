using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritReforged.Common.Easing;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritReforged.Common.ProjectileCommon;

public abstract class BaseClubProj(Vector2 size) : ModProjectile
{
	private const int MAX_FLICKERTIME = 20;
	private const int MAX_LINGERTIME = 30;
	private const int MAX_SWINGTIME = 30;

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
		Projectile.width = Projectile.height = 48;
		Projectile.aiStyle = -1;
		Projectile.friendly = true;
		Projectile.penetrate = -1;
		Projectile.tileCollide = false;
		Projectile.ownerHitCheck = true;

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

		if (!owner.channel && Charge > 0.33f)
		{
			released = true;
			HasFlickered = false;
		}

		--_flickerTime;

		TranslateRotation(owner, out float clubRotation, out float armRotation);
		Projectile.rotation = clubRotation;

		owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.ThreeQuarters, armRotation);
		owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.ThreeQuarters, armRotation);
		Projectile.position.X = owner.Center.X - (int)(Math.Cos(armRotation - MathHelper.PiOver2) * Size.X) - Projectile.width / 2;
		Projectile.position.Y = owner.Center.Y - (int)(Math.Sin(armRotation - MathHelper.PiOver2) * Size.Y) - Projectile.height / 2 - owner.gfxOffY;

		owner.itemAnimation = owner.itemTime = 2;
	}

	private void TranslateRotation(Player owner, out float clubRotation, out float armRotation)
	{
		float output = BaseRotation * owner.gravDir + 1.7f;
		float clubOffset = MathHelper.Pi + MathHelper.PiOver4;
		if (owner.direction < 0)
		{
			output = MathHelper.TwoPi - output;
			clubOffset -= MathHelper.PiOver2;
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

		Charge += 1f / ChargeTime;
		Charge = MathHelper.Min(Charge, 1);

		if(Charge == 1 && !HasFlickered)
		{
			SoundEngine.PlaySound(SoundID.NPCDeath7, Projectile.Center);
			_flickerTime = MAX_FLICKERTIME;
			HasFlickered = true;
		}

		Projectile.damage = (int)MathHelper.Lerp(MinDamage, MaxDamage, Charge);
		Projectile.knockBack = MathHelper.Lerp(MinKnockback, MaxKnockback, Charge);

		BaseRotation = MathHelper.Lerp(MathHelper.PiOver2, MathHelper.PiOver4 / 2, EaseFunction.EaseCircularOut.Ease(Charge));
		Projectile.scale = MathHelper.Lerp(0.6f, 1f, EaseFunction.EaseQuadOut.Ease(EaseFunction.EaseCircularOut.Ease(Charge)));
	}

	private void Swinging(Player owner)
	{
		owner.direction = Math.Sign(Projectile.velocity.X);
		bool validTile = Collision.SolidTiles(Projectile.position, Projectile.width, Projectile.height, true);
		Projectile.scale = 1;

		_swingTimer++;
		if (_swingTimer > 20)
			Projectile.Kill();

		if (_collided)
		{
			_lingerTimer--;
			if (_lingerTimer <= 0)
				Projectile.Kill();
		}
		else
			BaseRotation = MathHelper.Lerp(MathHelper.PiOver4 / 2, MathHelper.Pi * 1.25f, EaseFunction.EaseCubicOut.Ease(_swingTimer / (float)20));

		if (validTile && !_collided)
		{
			_collided = true;
			_lingerTimer = MAX_LINGERTIME;
			Smash(Projectile.Center);

			SoundEngine.PlaySound(SoundID.Item70, Projectile.Center);
			SoundEngine.PlaySound(SoundID.NPCHit42, Projectile.Center);
		}
	}

	public sealed override bool PreDraw(ref Color lightColor)
	{
		DrawTrail(lightColor);

		Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
		Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Main.player[Projectile.owner].Center - Main.screenPosition, texture.Frame(1, Main.projFrames[Type]), Projectile.GetAlpha(lightColor), Projectile.rotation, Origin, Projectile.scale, Effects, 0);

		SafeDraw(Main.spriteBatch, lightColor);

		if (Projectile.ai[0] >= 1 && !released && _flickerTime > 0)
		{
			float alpha = EaseFunction.EaseSine.Ease(_flickerTime / (float)MAX_FLICKERTIME);

			Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Main.player[Projectile.owner].Center - Main.screenPosition, texture.Frame(1, Main.projFrames[Type], 0, 1, 0, 0), Projectile.GetAlpha(Color.White * alpha), Projectile.rotation, Origin, Projectile.scale, Effects, 0);
		}

		return false;
	}
	public virtual void SafeAI() { }
	public virtual void SafeDraw(SpriteBatch spriteBatch, Color lightColor) { }
	public virtual void SafeSetDefaults() { }
	public virtual void SafeSetStaticDefaults() { }
	public virtual void Smash(Vector2 position) { }

	public virtual SpriteEffects Effects => Main.player[Projectile.owner].direction * (int)Main.player[Projectile.owner].gravDir < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
	public virtual Vector2 Origin => Effects == SpriteEffects.FlipHorizontally ? Size : new Vector2(0, Size.Y);

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