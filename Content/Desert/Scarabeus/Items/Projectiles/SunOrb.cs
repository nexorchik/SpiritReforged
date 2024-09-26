using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using SpiritReforged.Common.Easing;
using SpiritReforged.Common.Misc;
using SpiritReforged.Common.Particle;
using SpiritReforged.Common.PrimitiveRendering.PrimitiveShape;
using SpiritReforged.Common.PrimitiveRendering;
using SpiritReforged.Common.ProjectileCommon;
using SpiritReforged.Common.Visuals.Glowmasks;
using SpiritReforged.Content.Desert.Scarabeus.Items.Particles;
using SpiritReforged.Content.Particles;

namespace SpiritReforged.Content.Desert.Scarabeus.Items.Projectiles;

[AutoloadGlowmask("255,255,255", false)]
public class SunOrb : ModProjectile
{
	private const int LIFETIME = 80;

	private float AiTimer { get => Projectile.ai[0]; set => Projectile.ai[0] = value; }
	private float FlashTimer { get => Projectile.ai[1]; set => Projectile.ai[1] = value; }

	private Vector2 _offset;

	public override void SetDefaults()
	{
		Projectile.width = 26;
		Projectile.height = 26;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.friendly = true;
		Projectile.penetrate = -1;
		Projectile.aiStyle = 0;
		Projectile.timeLeft = LIFETIME;
		Projectile.scale = 0;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 10;
	}

	public override bool? CanCutTiles() => false;

	public override void AI()
	{
		Projectile.TryGetOwner(out Player owner);

		owner.itemAnimation = 2;
		owner.itemTime = 2;

		Projectile.Center = Vector2.Lerp(Projectile.Center, owner.MountedCenter + _offset + owner.velocity, 0.2f);
		_offset += Projectile.velocity;
		Projectile.velocity *= 0.93f;

		var growShrinkTime = new Point(20, 20);
		GrowShrink(growShrinkTime.X, growShrinkTime.Y);
		if(Projectile.scale >= 1)
		{

		}

		FlashTimer = Math.Max(FlashTimer - 1, 0);
		Lighting.AddLight(Projectile.Center, Color.LightGoldenrodYellow.ToVector3() * Projectile.scale);
	}

	private void GrowShrink(int growTime, int shrinkTime)
	{
		if (Projectile.timeLeft > LIFETIME - growTime)
		{
			float progress = 1 - (float)(Projectile.timeLeft - (LIFETIME - growTime)) / growTime;
			Projectile.scale = MathHelper.Lerp(0, 1, EaseFunction.EaseCircularOut.Ease(progress));
		}

		else if (Projectile.timeLeft < shrinkTime)
		{
			float progress = (float)Projectile.timeLeft / shrinkTime;
			Projectile.scale = MathHelper.Lerp(0, 1, EaseFunction.EaseQuadOut.Ease(progress));
		}
		else
			Projectile.scale = 1;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		if (Main.dedServ)
			return;

		ParticleHandler.SpawnParticle(new LightBurst(target.Center, Main.rand.NextFloatDirection(), Color.LightGoldenrodYellow.Additive(), 0.6f, Projectile.localNPCHitCooldown * 3));

		int numSmoke = 6;
		for (int i = 0; i < numSmoke; i++)
		{
			Color smokeColor = Color.LightGoldenrodYellow.Additive() * 0.3f;
			float progress = i / (float)numSmoke;
			float scale = MathHelper.Lerp(0.08f, 0.04f, progress);
			var vel = Vector2.Lerp(-Vector2.UnitY / 3, -Vector2.UnitY * 2, progress);
			ParticleHandler.SpawnParticle(new DissipatingImage(target.Center, smokeColor, Main.rand.NextFloatDirection(), scale, 0.6f, "Smoke", 40) { Velocity = vel });
		}

		for (int i = 0; i < 6; i++)
		{
			Vector2 particleCenter = target.Center + Main.rand.NextVector2Circular(15, 15);
			Vector2 particleVel = -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(1, 4);
			Color lightColor = Color.LightGoldenrodYellow.Additive();
			Color darkColor = Color.Lerp(Color.LightGoldenrodYellow, Color.OrangeRed, 0.5f).Additive() * 0.5f;
			float scale = Main.rand.NextFloat(0.6f, 1f);
			int lifeTime = Main.rand.Next(20, 40);
			static void delegateAction(Particle p)
			{
				p.Velocity = p.Velocity.RotatedByRandom(0.1f);
				p.Velocity *= 0.97f;
				p.Velocity.X *= 0.92f;
			}

			ParticleHandler.SpawnParticle(new GlowParticle(particleCenter, particleVel, lightColor, darkColor, scale, lifeTime, 5, delegateAction));
		}
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		if (Projectile.scale != 1)
			return false;

		GetRayDimensions(out float rayHeight, out float rayWidth, out float rayDist);
		var tip = Projectile.Center.ToPoint();
		int numCalcs = 20;

		for(int i = 0; i < numCalcs; i++)
		{
			float progress = i / (float)numCalcs;
			int curHeight = (int)(progress * rayHeight);
			int curWidth = (int)(progress * rayWidth);
			int curPosOffset = (int)(progress * rayDist);
			int rectangleHeight = (int)(rayHeight / numCalcs);

			var collisionLine = new Rectangle(tip.X - (curWidth / 2) + curPosOffset, tip.Y + curHeight, curWidth, rectangleHeight);
			if (collisionLine.Intersects(targetHitbox))
				return true;
		}

		return false;
	}

	private void GetRayDimensions(out float rayHeight, out float rayWidth, out float rayDist)
	{
		Projectile.TryGetOwner(out Player owner);

		rayHeight = 100;
		rayWidth = 100;
		rayDist = 130 * owner.direction;
	}

	public override bool PreDraw(ref Color lightColor)
	{
		float strength = 0.5f;
		Color glowColor = new Color(250, 167, 32, 0) * strength;
		Color rayColor = Color.LightGoldenrodYellow.Additive() * strength;

		DrawBigRay(rayColor);
		DrawBloom(glowColor);
		DrawGodrays(rayColor);

		Projectile.QuickDraw(rot: 0, drawColor: Color.White.Additive((byte)(230 * Projectile.Opacity)));

		DrawGlowmask(glowColor);

		return false;
	}

	private void DrawBigRay(Color glowColor)
	{
		Effect effect = AssetLoader.LoadedShaders["LightRay"];
		GetRayDimensions(out float rayHeight, out float rayWidth, out float rayDist);

		effect.Parameters["uTexture"].SetValue(AssetLoader.LoadedTextures["vnoise"]);
		float scrollAmount = Main.GlobalTimeWrappedHourly / 4;
		effect.Parameters["scroll"].SetValue(new Vector2(scrollAmount * Math.Sign(rayDist) * 0.75f, scrollAmount));
		effect.Parameters["textureStretch"].SetValue(new Vector2(1f, 0.125f) * 0.66f);
		effect.Parameters["texExponentRange"].SetValue(new Vector2(0.8f, 0.5f));
		effect.Parameters["finalIntensityMod"].SetValue(1.5f);

		effect.Parameters["uColor"].SetValue(glowColor.ToVector4());
		effect.Parameters["uColor2"].SetValue(new Color(250, 167, 32, 0).ToVector4());

		var square = new SquarePrimitive
		{
			Color = Color.White * EaseFunction.EaseCircularIn.Ease(Projectile.scale),
			Height = rayHeight * 1.5f * Projectile.scale,
			Length = rayWidth * 5 * EaseFunction.EaseQuadIn.Ease(Projectile.scale),
			BottomPosOffset = rayDist * Projectile.scale
		};
		square.SetTopPosition(Projectile.Center - Main.screenPosition);
		PrimitiveRenderer.DrawPrimitiveShape(square, effect);
	}

	private void DrawBloom(Color glowColor)
	{
		Texture2D bloomtex = AssetLoader.LoadedTextures["Bloom"];
		for (int i = 0; i < 3; i++)
		{
			var center = Projectile.Center - Main.screenPosition - new Vector2(Projectile.scale);
			float scale = Projectile.scale * 0.35f;
			var origin = bloomtex.Size() / 2;
			Color color = Projectile.GetAlpha(glowColor);
			Main.spriteBatch.Draw(bloomtex, center, null, color, 0, origin, scale, SpriteEffects.None, 0);
		}
	}

	private void DrawGodrays(Color glowColor)
	{
		Texture2D raytex = AssetLoader.LoadedTextures["Ray"];
		int numRays = 14;
		for (int i = 0; i < numRays; i++)
		{
			var center = Projectile.Center - Main.screenPosition;
			var rayScale = new Vector2(2f, 0.9f + (float)Math.Sin(i + Main.GlobalTimeWrappedHourly * (i / (float)numRays) * 2) / 6) * 0.4f;
			Color color = Projectile.GetAlpha(glowColor) * 0.6f;
			var origin = new Vector2(raytex.Width / 2, 0);
			float rotation = Main.GlobalTimeWrappedHourly + (MathHelper.TwoPi * i / numRays);
			if (i % 3 == 0) //Smaller inverse rotation rays
			{
				rayScale.Y *= 0.9f;
				rotation = -rotation + Main.GlobalTimeWrappedHourly / 2;
			}

			Main.spriteBatch.Draw(raytex, center, null, color, rotation, origin, rayScale * Projectile.scale, SpriteEffects.None, 0);
		}
	}

	private void DrawGlowmask(Color glowColor)
	{
		GlowmaskProjectile.ProjIdToGlowmask.TryGetValue(Projectile.type, out var glowMask);
		Texture2D glowTex = glowMask.Glowmask.Value;
		int numGlow = 8;
		for (int i = 0; i < numGlow; i++)
		{
			var position = Projectile.Center - Main.screenPosition;
			position += Vector2.UnitX.RotatedBy(MathHelper.TwoPi * i / numGlow) * 2f;
			float opacity = 2f / numGlow;
			Main.spriteBatch.Draw(glowTex, position, Projectile.DrawFrame(), Projectile.GetAlpha(glowColor) * opacity, 0, Projectile.DrawFrame().Size() / 2, Projectile.scale, SpriteEffects.None, 0);
		}
	}

	public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => overPlayers.Add(index);
}