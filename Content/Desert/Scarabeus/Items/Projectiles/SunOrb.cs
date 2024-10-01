using SpiritReforged.Common.Easing;
using SpiritReforged.Common.Misc;
using SpiritReforged.Common.Particle;
using SpiritReforged.Common.PrimitiveRendering.PrimitiveShape;
using SpiritReforged.Common.PrimitiveRendering;
using SpiritReforged.Common.ProjectileCommon;
using SpiritReforged.Common.Visuals.Glowmasks;
using SpiritReforged.Content.Particles;

namespace SpiritReforged.Content.Desert.Scarabeus.Items.Projectiles;

[AutoloadGlowmask("255,255,255", false)]
public class SunOrb : ModProjectile
{
	private const int GROWSHRINKTIME = 30;
	private const int RAYTIME = 40;
	private const int LIFETIME = RAYTIME + 2 * GROWSHRINKTIME;

	private Vector2 _offset;
	private Vector2 _mousePos;

	private float _staffRise;
	private float SunScale { get => Projectile.scale; set => Projectile.scale = value; }
	private float _rayScale;

	public override void SetDefaults()
	{
		Projectile.width = 26;
		Projectile.height = 26;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.friendly = true;
		Projectile.penetrate = -1;
		Projectile.aiStyle = 0;
		Projectile.scale = 0;
		Projectile.timeLeft = LIFETIME;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 10;
	}

	public override bool? CanCutTiles() => false;

	public override void AI()
	{
		Projectile.TryGetOwner(out Player owner);
		Init(owner);

		owner.itemAnimation = 2;
		owner.itemTime = 2;
		if (owner.whoAmI == Main.myPlayer)
		{
			_mousePos = Vector2.Lerp(_mousePos, Main.MouseWorld - owner.Center, 0.05f);
			owner.direction = Main.MouseWorld.X >= owner.MountedCenter.X ? 1 : -1;
			Projectile.netUpdate = true;
		}

		Projectile.Center = Vector2.Lerp(Projectile.Center, owner.MountedCenter + _offset + owner.velocity, 0.2f);

		GrowShrink();
		Projectile.ai[0]++;

		Projectile.rotation = MathHelper.Lerp(-MathHelper.PiOver4, -3 * MathHelper.PiOver4, _staffRise);
		if (owner.direction < 0)
			Projectile.rotation = MathHelper.Lerp(-MathHelper.PiOver4, -3 * MathHelper.PiOver4, 1 - _staffRise) + MathHelper.Pi;

		owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation);
		Lighting.AddLight(Projectile.Center, Color.LightGoldenrodYellow.ToVector3() * SunScale);
	}

	private void Init(Player owner)
	{
		if (Projectile.timeLeft != LIFETIME)
			return;

		_offset = Projectile.Center - owner.Center;
	}

	private void GrowShrink()
	{
		if (Projectile.timeLeft > LIFETIME - GROWSHRINKTIME)
		{
			float progress = 1 - (float)(Projectile.timeLeft - (LIFETIME - GROWSHRINKTIME)) / GROWSHRINKTIME;
			_staffRise = EaseFunction.EaseOutBack.Ease(progress);
			SunScale = EaseFunction.CompoundEase(EaseFunction.EaseCircularIn, EaseFunction.EaseOutBack, progress, 0.3f);
			_rayScale = EaseFunction.EaseCubicIn.Ease(progress);
		}
		else if (Projectile.timeLeft < GROWSHRINKTIME)
		{
			float progress = (float)Projectile.timeLeft / GROWSHRINKTIME;
			_staffRise = EaseFunction.EaseOutBack.Ease(progress);
			SunScale = EaseFunction.CompoundEase(EaseFunction.EaseCircularIn, EaseFunction.EaseOutBack, progress, 0.3f);
			_rayScale = EaseFunction.EaseCubicIn.Ease(progress);
		}
		else
			_staffRise = SunScale = _rayScale = 1;
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
		if (_rayScale != 1)
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
		rayHeight = _rayScale * 130;

		float mouseDist = _mousePos.X;
		float maxDist = 250f;
		mouseDist = MathHelper.Clamp(mouseDist, -maxDist, maxDist);

		var widthRange = new Vector2(40, 120);
		rayWidth = _rayScale * MathHelper.Lerp(widthRange.X, widthRange.Y, Math.Abs(mouseDist) / maxDist);
		rayDist = _rayScale * mouseDist;
	}

	public override bool PreDraw(ref Color lightColor)
	{
		float strength = 0.5f;
		Color glowColor = new Color(250, 167, 32, 0) * strength;
		Color rayColor = Color.LightGoldenrodYellow.Additive() * strength;

		DrawStaff(lightColor, glowColor);
		DrawBigRay(rayColor);
		DrawBloom(glowColor);
		DrawGodrays(rayColor);

		Projectile.QuickDraw(rot: 0, drawColor: Color.White.Additive((byte)(230 * Projectile.Opacity)));

		DrawGlowmask(glowColor);

		return false;
	}

	private void DrawStaff(Color lightColor, Color glowColor)
	{
		Projectile.TryGetOwner(out Player owner);
		Texture2D staffTex = TextureAssets.Item[ModContent.ItemType<SunStaff>()].Value;
		GlowmaskItem.ItemIdToGlowmask.TryGetValue(ModContent.ItemType<SunStaff>(), out GlowmaskInfo staffGlowAsset);
		Texture2D staffGlow = staffGlowAsset.Glowmask.Value;

		Vector2 origin = staffTex.Size() * new Vector2(0.2f, 0.8f);
		SpriteEffects flip = SpriteEffects.None;
		float rotationFlip = MathHelper.Lerp(Projectile.rotation, 0, 0.9f) - MathHelper.PiOver4;
		if (owner.direction < 0)
		{
			rotationFlip += MathHelper.PiOver2;
			flip = SpriteEffects.FlipHorizontally;
			origin = staffTex.Size() * new Vector2(0.8f, 0.8f);
		}

		Vector2 staffPos = owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation) - Main.screenPosition;
		float stafScale = MathHelper.Lerp(_staffRise, 1, 0.33f);
		Main.spriteBatch.Draw(staffTex, staffPos, null, lightColor, rotationFlip, origin, stafScale, flip, 1f);

		Main.spriteBatch.Draw(staffGlow, staffPos, null, glowColor, rotationFlip, origin, stafScale, flip, 1f);

		float numGlow = 6;
		for(int i = 0; i < numGlow; i++)
		{
			Vector2 offset = Vector2.UnitX.RotatedBy(MathHelper.TwoPi * i / numGlow) * 5;
			float opacity = 2 / numGlow;
			Main.spriteBatch.Draw(staffGlow, staffPos + offset, null, glowColor * opacity, rotationFlip, origin, stafScale, flip, 1f);
		}
	}

	private void DrawBigRay(Color glowColor)
	{
		Effect effect = AssetLoader.LoadedShaders["LightRay"];
		GetRayDimensions(out float rayHeight, out float rayWidth, out float rayDist);

		effect.Parameters["uTexture"].SetValue(AssetLoader.LoadedTextures["vnoise"]);
		float scrollAmount = (EaseFunction.EaseQuadOut.Ease(Projectile.ai[0] / LIFETIME) / 2f);
		effect.Parameters["scroll"].SetValue(new Vector2(scrollAmount * Math.Sign(rayDist), scrollAmount * 2));
		effect.Parameters["textureStretch"].SetValue(new Vector2(1f, 0.125f) * 0.6f);
		effect.Parameters["texExponentRange"].SetValue(new Vector2(0.6f));
		effect.Parameters["finalIntensityMod"].SetValue(3 * _rayScale);
		effect.Parameters["finalExponent"].SetValue(4);

		effect.Parameters["uColor"].SetValue(glowColor.ToVector4());
		effect.Parameters["uColor2"].SetValue(new Color(250, 167, 32, 0).ToVector4());

		var rayVisualStretch = new Vector2(5, 1.2f);

		var squares = new List<SquarePrimitive>();

		for(int i = 2; i >= 0; i--)
		{
			float progress = i / 2f;
			float widthMod = MathHelper.Lerp(1, 1.2f, progress);
			float heightMod = MathHelper.Lerp(1, 0.33f, progress);
			float colorMod = i == 0 ? 1 : heightMod * 0.75f;
			var ray = new SquarePrimitive
			{
				Color = Color.White * colorMod,
				Height = rayHeight * rayVisualStretch.Y * heightMod,
				Length = rayWidth * rayVisualStretch.X * widthMod,
				BottomPosOffset = rayDist * heightMod
			};
			ray.SetTopPosition(Projectile.Center - Main.screenPosition);
			squares.Add(ray);
		}

		PrimitiveRenderer.DrawPrimitiveShapeBatched(squares.ToArray(), effect);
	}

	private void DrawBloom(Color glowColor)
	{
		Texture2D bloomtex = AssetLoader.LoadedTextures["Bloom"];
		float numBloom = 4;
		for (int i = 0; i < numBloom; i++)
		{
			float progress = 1 / numBloom;
			var center = Projectile.Center - Main.screenPosition - new Vector2(SunScale);
			float scale = SunScale * MathHelper.Lerp(0.3f, 0.4f, progress);
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

			Main.spriteBatch.Draw(raytex, center, null, color, rotation, origin, rayScale * SunScale, SpriteEffects.None, 0);
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
			Main.spriteBatch.Draw(glowTex, position, Projectile.DrawFrame(), Projectile.GetAlpha(glowColor) * opacity, 0, Projectile.DrawFrame().Size() / 2, SunScale, SpriteEffects.None, 0);
		}
	}

	public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => overPlayers.Add(index);

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		if (target.type is NPCID.Vampire or NPCID.VampireBat)
			modifiers.SourceDamage *= 100f; //silly
	}
}