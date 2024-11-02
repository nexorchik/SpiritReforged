using Microsoft.Xna.Framework.Graphics;
using SpiritReforged.Common.Easing;
using SpiritReforged.Common.Particle;
using SpiritReforged.Common.PrimitiveRendering.PrimitiveShape;
using SpiritReforged.Common.PrimitiveRendering;
using SpiritReforged.Common.ProjectileCommon;
using SpiritReforged.Common.Visuals.Glowmasks;
using SpiritReforged.Content.Particles;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using static Terraria.Player;

namespace SpiritReforged.Content.Desert.Scarabeus.Items.Projectiles;

public class RoyalKhopeshHeld : ModProjectile
{
	public int SwingTime { get; set; }
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
		Projectile.timeLeft = 2;

		int tempDirection = owner.direction;
		owner.direction = BaseDirection.X >= 0 ? 1 : -1;
		if (tempDirection != owner.direction && Main.netMode != NetmodeID.SinglePlayer)
			NetMessage.SendData(MessageID.SyncPlayer, -1, -1, null, owner.whoAmI);

		int direction = SwingDirection * owner.direction;
		Projectile.spriteDirection = direction;

		float progress = AiTimer / SwingTime;
		float swingProgress = EaseFunction.EaseQuarticOut.Ease(progress);
		if (direction < 0)
			swingProgress = 1 - swingProgress;

		Projectile.rotation = MathHelper.Lerp(-SwingRadians * 0.6f, SwingRadians * 0.6f - MathHelper.PiOver4, swingProgress);

		Projectile.rotation += BaseDirection.ToRotation() + MathHelper.PiOver4;
		if (direction < 0)
			Projectile.rotation += MathHelper.PiOver4 + MathHelper.PiOver2;

		Projectile.scale = EaseFunction.EaseQuadOut.Ease((float)Math.Sin(swingProgress * MathHelper.Pi)) * 0.4f + 0.6f;

		float armRot = Projectile.rotation - 3 * MathHelper.PiOver4;
		if (direction < 0)
			armRot -= MathHelper.PiOver2;

		owner.SetCompositeArmFront(true, CompositeArmStretchAmount.Full, armRot);
		Projectile.Center = owner.GetFrontHandPosition(CompositeArmStretchAmount.Full, armRot);

		AiTimer++;
		if (AiTimer > SwingTime)
			Projectile.Kill();
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		Projectile.TryGetOwner(out Player Owner);
		Vector2 directionUnit = Vector2.UnitX.RotatedBy(Projectile.rotation - MathHelper.PiOver4);
		return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Owner.MountedCenter, Owner.MountedCenter + directionUnit * Projectile.Size.Length() * Projectile.scale);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Projectile.TryGetOwner(out Player Owner);
		Texture2D swordTex = TextureAssets.Projectile[Type].Value;
		Vector2 origin = new Vector2(0.2f, 0.8f) * swordTex.Size();
		if (Projectile.spriteDirection < 0)
			origin = new Vector2(0.8f, 0.8f) * swordTex.Size();

		float progress = AiTimer / SwingTime;
		float swingProgress = EaseFunction.EaseCubicOut.Ease(progress);

		Effect effect = AssetLoader.LoadedShaders["NoiseParticleTrail"];
		effect.Parameters["baseTexture"].SetValue(AssetLoader.LoadedTextures["cloudNoise"]);
		effect.Parameters["baseColorDark"].SetValue(new Color(186, 168, 84).ToVector4());
		effect.Parameters["baseColorLight"].SetValue(new Color(223, 219, 147).ToVector4());
		effect.Parameters["overlayTexture"].SetValue(AssetLoader.LoadedTextures["particlenoise"]);
		effect.Parameters["overlayColor"].SetValue(new Color(58, 49, 18).ToVector4());

		effect.Parameters["coordMods"].SetValue(new Vector2(0.5f, 0.25f));
		effect.Parameters["overlayCoordMods"].SetValue(new Vector2(1f, 0.1f) * 1.5f);
		effect.Parameters["overlayScrollMod"].SetValue(new Vector2(0.5f, -1f));
		effect.Parameters["overlayExponentRange"].SetValue(new Vector2(5f, 0.5f));

		effect.Parameters["timer"].SetValue(-swingProgress * 0.5f - progress * 0.3f);
		effect.Parameters["progress"].SetValue(swingProgress);
		effect.Parameters["alphaMod"].SetValue(1);
		effect.Parameters["intensity"].SetValue(2);

		Vector2 pos = Owner.MountedCenter - Main.screenPosition;
		var slash = new PrimitiveSlashArc
		{
			BasePosition = pos,
			StartDistance = Projectile.Size.Length() * 0.5f,
			EndDistance = Projectile.Size.Length() * 0.9f,
			AngleRange = new Vector2(SwingRadians / 2 * SwingDirection, -SwingRadians / 2 * SwingDirection) * (Owner.direction * -1),
			DirectionUnit = Vector2.Normalize(BaseDirection),
			Color = lightColor * EaseFunction.EaseQuadOut.Ease(EaseFunction.EaseCircularOut.Ease((float)Math.Sin(swingProgress * MathHelper.Pi))),
			SlashProgress = swingProgress,
			RectangleCount = 30
		};
		PrimitiveRenderer.DrawPrimitiveShape(slash, effect);

		Projectile.QuickDraw(origin: origin);
		return false;
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