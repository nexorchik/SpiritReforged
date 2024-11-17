using SpiritReforged.Common.PrimitiveRendering.PrimitiveShape;
using SpiritReforged.Common.PrimitiveRendering;
using SpiritReforged.Common.ProjectileCommon;
using System.IO;
using static Terraria.Player;
using static SpiritReforged.Common.Easing.EaseFunction;
using static Microsoft.Xna.Framework.MathHelper;
using Terraria.Audio;
using SpiritReforged.Common.Visuals.Glowmasks;

namespace SpiritReforged.Content.Desert.Scarabeus.Items.Projectiles;

[AutoloadGlowmask("255,255,255", false)]
public class RoyalKhopeshHeld : ModProjectile
{
	private const float WindupTime = 0.35f;

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

	public void DoSwingNoise()
	{
		if (Main.netMode != NetmodeID.Server)
			SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing with { MaxInstances = -1 }, Projectile.Center);
	}

	private void FirstSwing(float progress, int direction)
	{
		float swingProgress = EaseQuarticOut.Ease(progress);
		Projectile.scale = EaseCircularIn.Ease(EaseSine.Ease(swingProgress)) * 0.4f + 0.6f;

		if (direction < 0)
			swingProgress = 1 - swingProgress;

		Projectile.rotation = Lerp(-SwingRadians * 0.6f, SwingRadians * 0.6f - PiOver4, swingProgress);

		Projectile.rotation += BaseDirection.ToRotation() + PiOver4;
		if (direction < 0)
			Projectile.rotation += PiOver4 + PiOver2;
	}

	private void DoubleSwing(float progress, ref int direction)
	{
		float halfProgress = (progress % 0.5f) * 2;

		float swingProgress = EaseQuarticOut.Ease(halfProgress);
		Projectile.scale = EaseQuadIn.Ease(EaseSine.Ease(halfProgress)) * 0.2f + 0.6f;

		if (progress >= 0.5f)
			swingProgress = 1 - swingProgress;

		if(AiTimer == SwingTime / 2)
		{
			Projectile.ResetLocalNPCHitImmunity();
			DoSwingNoise();
		}

		Projectile.rotation = Lerp(-SwingRadians * 0.6f, SwingRadians * 0.6f - PiOver4, swingProgress);

		Projectile.rotation += BaseDirection.ToRotation() + PiOver4;
		if (direction < 0)
			Projectile.rotation += PiOver4 + PiOver2;
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
			if ((int)(progress * SwingTime) == (int)(WindupTime * SwingTime) + 1)
				DoSwingNoise();

			float swingProgress = (progress - WindupTime) / (1 - WindupTime);
			swingProgress = EaseCircularOut.Ease(EaseQuadOut.Ease(swingProgress));
			Projectile.scale = EaseQuadIn.Ease(EaseSine.Ease(swingProgress)) * 0.2f + 0.8f - EaseCubicIn.Ease(swingProgress) * 0.2f;

			if (direction < 0)
				swingProgress = 1 - swingProgress;

			Projectile.rotation = Lerp(-SwingRadians * 0.6f, SwingRadians * 0.6f - PiOver4, swingProgress);

			Projectile.rotation += BaseDirection.ToRotation() + PiOver4;
			if (direction < 0)
				Projectile.rotation += PiOver4 + PiOver2;
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

		DrawTrail();

		Projectile.QuickDraw(origin: origin);
		if (Combo == 2)
			EmpoweredGlow(origin);

		return false;
	}

	private void DrawTrail()
	{
		Projectile.TryGetOwner(out Player Owner);
		float progress = AiTimer / SwingTime;
		float swingProgress = EaseCubicOut.Ease(progress);
		float trailDirection = SwingDirection;
		float maxDist = 0.9f;
		int rectCount = 30;
		bool useLightColor = true;
		Effect effect;

		if (Combo == 1)
		{
			if (progress < 0.5f)
				trailDirection *= -1;

			trailDirection *= Owner.direction;

			progress = (progress % 0.5f) * 2;
			maxDist = 0.7f;
			swingProgress = EaseQuadOut.Ease(progress);
		}

		if(Combo == 2)
		{
			if (progress < WindupTime)
				return;

			progress = (progress - WindupTime) / (1 - WindupTime);
			swingProgress = EaseCircularOut.Ease(EaseQuadOut.Ease(progress));
			SetRedtrailParams(out effect, progress, swingProgress);
			//rectCount = 10;
			useLightColor = false;
		}
		else
			SetSandtrailParams(out effect, progress, swingProgress);

		Vector2 pos = Owner.MountedCenter - Main.screenPosition;
		var slash = new PrimitiveSlashArc
		{
			BasePosition = pos,
			MinDistance = Projectile.Size.Length() * 0.6f,
			MaxDistance = Projectile.Size.Length() * maxDist,
			Width = Projectile.Size.Length() * 0.45f,
			MaxWidth = Projectile.Size.Length() * 0.55f,
			AngleRange = new Vector2(SwingRadians / 2 * trailDirection, -SwingRadians / 2 * trailDirection) * (Owner.direction * -1),
			DirectionUnit = Vector2.Normalize(BaseDirection),
			Color = Color.White,
			UseLightColor = useLightColor,
			DistanceEase = EaseQuinticIn,
			SlashProgress = swingProgress,
			RectangleCount = rectCount
		};
		PrimitiveRenderer.DrawPrimitiveShape(slash, effect);
	}

	private static void SetSandtrailParams(out Effect effect, float progress, float swingProgress)
	{
		effect = AssetLoader.LoadedShaders["NoiseParticleTrail"];
		effect.Parameters["baseTexture"].SetValue(AssetLoader.LoadedTextures["noise"]);
		effect.Parameters["baseColorDark"].SetValue(new Color(186, 168, 84).ToVector4() * 0.25f);
		effect.Parameters["baseColorLight"].SetValue(new Color(223, 219, 147).ToVector4());
		effect.Parameters["overlayTexture"].SetValue(AssetLoader.LoadedTextures["particlenoise"]);
		effect.Parameters["overlayColor"].SetValue(new Color(58, 49, 18).ToVector4());

		effect.Parameters["coordMods"].SetValue(new Vector2(1.8f, 0.3f));
		effect.Parameters["overlayCoordMods"].SetValue(new Vector2(1f, 0.1f));
		effect.Parameters["overlayScrollMod"].SetValue(new Vector2(0.4f, -1f));
		effect.Parameters["overlayExponentRange"].SetValue(new Vector2(5f, 0.5f));

		effect.Parameters["timer"].SetValue(-swingProgress * 0.5f - progress * 0.2f);
		effect.Parameters["progress"].SetValue(swingProgress);
		effect.Parameters["intensity"].SetValue(2f);
		effect.Parameters["opacity"].SetValue(EaseCircularOut.Ease(EaseSine.Ease(EaseQuadOut.Ease(swingProgress))));
	}

	private static void SetRedtrailParams(out Effect effect, float progress, float swingProgress)
	{
		effect = AssetLoader.LoadedShaders["NoiseParticleTrail"];
		effect.Parameters["baseTexture"].SetValue(AssetLoader.LoadedTextures["noiseCrystal"]);
		effect.Parameters["baseColorDark"].SetValue(new Color(255, 3, 32).ToVector4());
		effect.Parameters["baseColorLight"].SetValue(new Color(252, 124, 158).ToVector4());
		effect.Parameters["overlayTexture"].SetValue(AssetLoader.LoadedTextures["vnoise"]);
		effect.Parameters["overlayColor"].SetValue(new Color(252, 124, 158).ToVector4());

		effect.Parameters["coordMods"].SetValue(new Vector2(4f, 1f));
		effect.Parameters["overlayCoordMods"].SetValue(new Vector2(1f, 0.1f) * 2f);
		effect.Parameters["overlayScrollMod"].SetValue(new Vector2(0.4f, -1f));
		effect.Parameters["overlayExponentRange"].SetValue(new Vector2(50f, 50f));

		effect.Parameters["timer"].SetValue(-swingProgress * 0.5f - progress * 0.2f);
		effect.Parameters["progress"].SetValue(swingProgress);
		effect.Parameters["intensity"].SetValue(2f);
		effect.Parameters["opacity"].SetValue(EaseCircularOut.Ease(EaseSine.Ease(EaseQuadOut.Ease(swingProgress))));
	}

	private void EmpoweredGlow(Vector2 origin)
	{
		float progress = AiTimer / SwingTime;
		float windupProgress = (progress) / WindupTime;
		windupProgress = Min(windupProgress, 1);

		Texture2D starTex = AssetLoader.LoadedTextures["Star"];
		GlowmaskProjectile.ProjIdToGlowmask.TryGetValue(Type, out GlowmaskInfo glowmaskInfo);
		Texture2D glowmaskTex = glowmaskInfo.Glowmask.Value;


		var center = Projectile.Center - Main.screenPosition;
		float maxSize = 0.6f * Projectile.scale;
		float starProgress = EaseSine.Ease(windupProgress);

		Vector2 scale = new Vector2(1f, 1f) * Lerp(0, maxSize, starProgress) * 0.7f;
		var starOrigin = starTex.Size() / 2;
		Color color = Projectile.GetAlpha(Color.Lerp(new Color(252, 124, 158, 0), new Color(255, 3, 32, 0), starProgress)) * EaseQuadOut.Ease(starProgress);
		Main.spriteBatch.Draw(starTex, center, null, color, Projectile.rotation, starOrigin, scale, SpriteEffects.None, 0);

		float glowProgress = EaseCircularIn.Ease(windupProgress);
		color = new Color(255, 53, 72, 0) * glowProgress * EaseCircularOut.Ease(1 - progress);
		SpriteEffects effects = (Projectile.spriteDirection < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
		for (int i = 0; i < 6; i++)
		{
			Vector2 offset = Vector2.UnitX.RotatedBy((TwoPi * i / 6) + Projectile.rotation);

			Main.spriteBatch.Draw(glowmaskTex, center + offset, null, color * 0.33f, Projectile.rotation, origin, Projectile.scale, effects, 0);
		}
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