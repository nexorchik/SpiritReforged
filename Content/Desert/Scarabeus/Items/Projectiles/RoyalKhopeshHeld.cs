using SpiritReforged.Common.PrimitiveRendering.PrimitiveShape;
using SpiritReforged.Common.PrimitiveRendering;
using SpiritReforged.Common.ProjectileCommon;
using System.IO;
using static Terraria.Player;
using static SpiritReforged.Common.Easing.EaseFunction;
using static Microsoft.Xna.Framework.MathHelper;
using Terraria.Audio;

namespace SpiritReforged.Content.Desert.Scarabeus.Items.Projectiles;

public class RoyalKhopeshHeld : ModProjectile
{
	private const float WindupTime = 0.55f;

	public int SwingTime { get; set; }
	public int Combo { get; set; }
	public float SwingRadians { get; set; }

	public int SwingDirection { get; set; }

	public Vector2 BaseDirection { get; set; }

	private float AiTimer { get => Projectile.ai[0]; set => Projectile.ai[0] = value; }

	public override void SetDefaults()
	{
		Projectile.width = 46;
		Projectile.height = 46;
		Projectile.DamageType = DamageClass.Melee;
		Projectile.friendly = true;
		Projectile.aiStyle = -1;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		Projectile.penetrate = -1;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = -1;
	}

	public override void AI()
	{
		if(!Projectile.TryGetOwner(out Player owner))
		{
			Projectile.Kill();
			return;
		}

		//owner.heldProj = Projectile.whoAmI;
		owner.itemAnimation = 2;
		owner.itemTime = 2;
		if(Combo == 2)
			owner.reuseDelay = 10;

		Projectile.timeLeft = 2;

		int tempDirection = owner.direction;
		owner.direction = BaseDirection.X >= 0 ? 1 : -1;
		if (tempDirection != owner.direction && Main.netMode != NetmodeID.SinglePlayer)
			NetMessage.SendData(MessageID.SyncPlayer, -1, -1, null, owner.whoAmI);

		int direction = SwingDirection * owner.direction;
		Projectile.spriteDirection = direction;

		float progress = AiTimer / SwingTime;
		switch (Combo)
		{
			case 0: FirstSwing(progress, direction);
				break;
			case 1: DoubleSwing(progress, ref direction);
				break;
			default: FinalSwing(progress, direction);
				break;
		}

		float armRot = Projectile.rotation - 3 * PiOver4;
		if (direction < 0)
			armRot -= PiOver2;

		owner.SetCompositeArmFront(true, CompositeArmStretchAmount.Full, armRot);
		Projectile.Center = owner.GetFrontHandPosition(CompositeArmStretchAmount.Full, armRot);

		AiTimer++;
		if (AiTimer > SwingTime)
			Projectile.Kill();
	}

	private void FirstSwing(float progress, int direction)
	{
		float swingProgress = EaseQuarticOut.Ease(progress);
		if (direction < 0)
			swingProgress = 1 - swingProgress;

		Projectile.rotation = Lerp(-SwingRadians * 0.6f, SwingRadians * 0.6f - PiOver4, swingProgress);

		Projectile.rotation += BaseDirection.ToRotation() + PiOver4;
		if (direction < 0)
			Projectile.rotation += PiOver4 + PiOver2;

		Projectile.scale = EaseQuadOut.Ease(EaseSine.Ease(swingProgress)) * 0.4f + 0.6f;
	}

	private void DoubleSwing(float progress, ref int direction)
	{
		float halfProgress = (progress % 0.5f) * 2;

		float swingProgress = EaseQuadOut.Ease(halfProgress);
		if (progress >= 0.5f)
			swingProgress = 1 - swingProgress;

		if(AiTimer == SwingTime / 2)
		{
			Projectile.ResetLocalNPCHitImmunity();
			if(Main.netMode != NetmodeID.Server)
				SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing with {MaxInstances = -1 }, Projectile.Center);
		}

		Projectile.rotation = Lerp(-SwingRadians * 0.6f, SwingRadians * 0.6f - PiOver4, swingProgress);

		Projectile.rotation += BaseDirection.ToRotation() + PiOver4;
		if (direction < 0)
			Projectile.rotation += PiOver4 + PiOver2;

		Projectile.scale = EaseQuadIn.Ease(EaseSine.Ease(halfProgress)) * 0.4f + 0.6f;
	}

	private void FinalSwing(float progress, int direction)
	{
		if(progress < WindupTime)
		{
			float windupProgress = progress / WindupTime;
			windupProgress = EaseCircularOut.Ease(windupProgress);

			Projectile.rotation = Lerp(-SwingRadians * 0.2f, -SwingRadians * 0.6f, windupProgress) * direction;
			Projectile.rotation += BaseDirection.ToRotation() + PiOver4;
			if (direction < 0)
				Projectile.rotation += PiOver4;

			Projectile.scale = windupProgress * 0.2f + 0.6f;
		}
		else
		{
			float swingProgress = (progress - WindupTime) / (1 - WindupTime);
			swingProgress = EaseCircularOut.Ease(swingProgress);
			if (direction < 0)
				swingProgress = 1 - swingProgress;

			Projectile.rotation = Lerp(-SwingRadians * 0.6f, SwingRadians * 0.6f - PiOver4, swingProgress);

			Projectile.rotation += BaseDirection.ToRotation() + PiOver4;
			if (direction < 0)
				Projectile.rotation += PiOver4 + PiOver2;

			Projectile.scale = EaseQuadIn.Ease(EaseSine.Ease(swingProgress)) * 0.2f + 0.8f - EaseCubicIn.Ease(swingProgress) * 0.2f;
		}
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		Projectile.TryGetOwner(out Player Owner);
		Vector2 directionUnit = Vector2.UnitX.RotatedBy(Projectile.rotation - PiOver4);
		return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Owner.MountedCenter, Owner.MountedCenter + directionUnit * Projectile.Size.Length() * Projectile.scale);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D swordTex = TextureAssets.Projectile[Type].Value;
		Vector2 origin = new Vector2(0.2f, 0.8f) * swordTex.Size();
		if (Projectile.spriteDirection < 0)
			origin = new Vector2(0.8f, 0.8f) * swordTex.Size();

		DrawTrail(lightColor);

		Projectile.QuickDraw(origin: origin);
		return false;
	}

	private void DrawTrail(Color lightColor)
	{
		Projectile.TryGetOwner(out Player Owner);
		float progress = AiTimer / SwingTime;
		float swingProgress = EaseCubicOut.Ease(progress);
		float trailDirection = SwingDirection;

		if(Combo == 1)
		{
			if (progress > 0.5f)
				trailDirection *= -1;

			trailDirection *= Owner.direction;

			progress = (progress % 0.5f) * 2;
			swingProgress = EaseQuadOut.Ease(progress);
		}

		if(Combo == 2)
		{
			if (progress < WindupTime)
				return;

			progress = (progress - WindupTime) / (1 - WindupTime);
			swingProgress = EaseCubicOut.Ease(progress);
		}

		Effect effect = AssetLoader.LoadedShaders["NoiseParticleTrail"];
		effect.Parameters["baseTexture"].SetValue(AssetLoader.LoadedTextures["cloudNoise"]);
		effect.Parameters["baseColorDark"].SetValue(new Color(186, 168, 84).ToVector4());
		effect.Parameters["baseColorLight"].SetValue(new Color(223, 219, 147).ToVector4());
		effect.Parameters["overlayTexture"].SetValue(AssetLoader.LoadedTextures["particlenoise"]);
		effect.Parameters["overlayColor"].SetValue(new Color(58, 49, 18).ToVector4());

		effect.Parameters["coordMods"].SetValue(new Vector2(0.5f, 0.25f));
		effect.Parameters["overlayCoordMods"].SetValue(new Vector2(1f, 0.1f) * 1.5f);
		effect.Parameters["overlayScrollMod"].SetValue(new Vector2(0.4f, -1f));
		effect.Parameters["overlayExponentRange"].SetValue(new Vector2(5f, 0.5f));

		effect.Parameters["timer"].SetValue(-swingProgress * 0.5f - progress * 0.2f);
		effect.Parameters["progress"].SetValue(swingProgress);
		effect.Parameters["intensity"].SetValue(2f);
		effect.Parameters["opacity"].SetValue(EaseCircularOut.Ease(EaseSine.Ease(EaseQuadOut.Ease(swingProgress))));

		Vector2 pos = Owner.MountedCenter - Main.screenPosition;
		var slash = new PrimitiveSlashArc
		{
			BasePosition = pos,
			MinDistance = Projectile.Size.Length() * 0.6f,
			MaxDistance = Projectile.Size.Length() * 0.8f,
			Width = Projectile.Size.Length() * 0.4f,
			MaxWidth = Projectile.Size.Length() * 0.7f,
			AngleRange = new Vector2(SwingRadians / 2 * trailDirection, -SwingRadians / 2 * trailDirection) * (Owner.direction * -1),
			DirectionUnit = Vector2.Normalize(BaseDirection),
			Color = lightColor,
			DistanceEase = EaseQuinticIn,
			SlashProgress = swingProgress,
			RectangleCount = 30
		};
		PrimitiveRenderer.DrawPrimitiveShape(slash, effect);
	}

	public override void SendExtraAI(BinaryWriter writer)
	{
		writer.WriteVector2(BaseDirection);
		writer.Write(SwingDirection);
		writer.Write(SwingRadians);
		writer.Write(SwingTime);
	}

	public override void ReceiveExtraAI(BinaryReader reader)
	{
		BaseDirection = reader.ReadVector2();
		SwingDirection = reader.ReadInt32();
		SwingRadians = reader.ReadSingle();
		SwingTime = reader.ReadInt32();
	}
}