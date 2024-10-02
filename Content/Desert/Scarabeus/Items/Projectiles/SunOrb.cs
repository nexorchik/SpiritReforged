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
	private const int RAYTIME = 22;
	private const int RAY_COOLDOWNTIME = 18;

	private float AiTimer { get => Projectile.ai[0]; set => Projectile.ai[0] = value; }
	private float FlashTimer { get => Projectile.ai[1]; set => Projectile.ai[1] = value; }
	private float ShrinkTimer { get => Projectile.ai[2]; set => Projectile.ai[2] = value; }

	private Vector2 _offset;
	private Vector2 _mousePos;

	private float _rayScale;

	private bool _stoppedChannel = false;
	private bool _initialized = false;

	public override void SetDefaults()
	{
		Projectile.width = 26;
		Projectile.height = 26;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.friendly = true;
		Projectile.penetrate = -1;
		Projectile.aiStyle = 0;
		Projectile.scale = 0;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 20;
	}

	public override bool? CanCutTiles() => false;

	public override void AI()
	{
		Projectile.TryGetOwner(out Player owner);
		Projectile.timeLeft = 2;

		if(!_initialized)
		{
			Projectile.netUpdate = true;
			_offset = Projectile.Center - owner.Center;
			_initialized = true;
		}

		if(Main.myPlayer == owner.whoAmI)
		{
			_mousePos = Vector2.Lerp(_mousePos, Main.MouseWorld - owner.Center, 0.07f);
			Projectile.netUpdate = true;
		}

		Projectile.Center = Vector2.Lerp(Projectile.Center, owner.MountedCenter + _offset + owner.velocity + _mousePos/6, 0.2f);

		GrowShrink();
		if (AiTimer > GROWSHRINKTIME)
			TryFlashRay(); 
		
		if (!owner.channel && !_stoppedChannel)
		{
			_stoppedChannel = true;
			Projectile.netUpdate = true;
		}

		Lighting.AddLight(Projectile.Center, Color.Goldenrod.ToVector3() * Projectile.scale);
		GetRayDimensions(out float rayHeight, out _, out float rayDist);
		for(int i = 0; i < 10; i++)
		{
			Vector2 pos = new Vector2(rayDist, rayHeight) * i / 10f;
			Lighting.AddLight(Projectile.Center + pos, Color.Goldenrod.ToVector3() * Projectile.scale * 0.5f);
		}

		FlashTimer = Math.Max(FlashTimer - 1, 0);
		AiTimer++;
	}

	private void GrowShrink()
	{
		float progress;
		if (!_stoppedChannel)
		{
			progress = AiTimer / GROWSHRINKTIME;
			progress = Math.Min(progress, 1);
		}
		else
		{
			ShrinkTimer++;
			progress = 1 - (float)ShrinkTimer / GROWSHRINKTIME;
			progress = Math.Max(progress, 0);
			if (progress == 0)
				Projectile.Kill();
		}

		Projectile.scale = EaseFunction.CompoundEase(EaseFunction.EaseCircularIn, EaseFunction.EaseOutBack, progress, 0.3f);
		_rayScale = EaseFunction.EaseQuadOut.Ease(progress) * Projectile.scale;
	}

	private void TryFlashRay()
	{
		Projectile.TryGetOwner(out Player owner);
		if ((AiTimer - GROWSHRINKTIME) % (RAYTIME + RAY_COOLDOWNTIME) == 0)
		{
			FlashTimer = RAYTIME;
			Projectile.ResetLocalNPCHitImmunity();
		}
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		if (Main.dedServ)
			return;

		ParticleHandler.SpawnParticle(new LightBurst(target.Center, Main.rand.NextFloatDirection(), Color.LightGoldenrodYellow.Additive(), 0.6f, 30));

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
			int curWidth = (int)MathHelper.Lerp(Projectile.width, rayWidth, progress);
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
		rayHeight = _rayScale * (-_offset.Y + Projectile.height/2);

		float mouseDist = _mousePos.X;
		float maxDist = 170f;
		mouseDist = MathHelper.Clamp(mouseDist, -maxDist, maxDist);

		var widthRange = new Vector2(80, 120);
		rayWidth = _rayScale * MathHelper.Lerp(widthRange.X, widthRange.Y, Math.Abs(mouseDist) / maxDist);
		rayDist = _rayScale * mouseDist;
	}

	private float GetFlashProgress => FlashTimer / RAYTIME;

	public override bool PreDraw(ref Color lightColor)
	{
		var glowColor = new Color(250, 167, 32, 0);
		var rayColor = Color.LightGoldenrodYellow.Additive(100);
		var darkRayColor = new Color(250, 167, 32, 200);
		GetRayDimensions(out float rayHeight, out float rayWidth, out float rayDist);

		DrawBigRay(rayColor * 0.5f, darkRayColor, rayHeight, rayWidth, rayDist);
		float strength = MathHelper.Lerp(GetFlashProgress, 1, 0.25f);
		glowColor *= strength;
		rayColor *= strength;
		darkRayColor *= strength;

		DrawBloom(glowColor);

		float bigRayRotation = new Vector2(rayDist, rayHeight).ToRotation() - MathHelper.PiOver2;
		//DrawGodrays(rayColor, darkRayColor, bigRayRotation);

		Projectile.QuickDraw(rot: 0, drawColor: Color.White.Additive((byte)(230 * Projectile.Opacity)));

		DrawGlowmask(glowColor);

		return false;
	}

	private void DrawBigRay(Color rayColor, Color darkRayColor, float rayHeight, float rayWidth, float rayDist)
	{
		Effect effect = AssetLoader.LoadedShaders["LightRay"];

		effect.Parameters["uTexture"].SetValue(AssetLoader.LoadedTextures["vnoise"]);
		float scrollAmount = Main.GlobalTimeWrappedHourly / 3f;
		effect.Parameters["scroll"].SetValue(new Vector2(scrollAmount * Math.Sign(rayDist), scrollAmount / 4));
		effect.Parameters["textureStretch"].SetValue(new Vector2(1f, 0.05f) * 0.5f);
		effect.Parameters["texExponentRange"].SetValue(new Vector2(2f, 0.25f));
		effect.Parameters["finalIntensityMod"].SetValue(4 * EaseFunction.EaseCircularIn.Ease(MathHelper.Clamp(_rayScale, 0, 1)));
		effect.Parameters["finalExponent"].SetValue(3);

		effect.Parameters["uColor"].SetValue(rayColor.ToVector4());
		effect.Parameters["uColor2"].SetValue(darkRayColor.ToVector4());

		var rayVisualStretch = new Vector3(2.5f, 1.4f, 1.15f); //Stretch the primitive to better fit the actual hitbox, since shader makes it innaccurate
		var rayFinalDimensions = new Vector3(rayWidth * rayVisualStretch.X, rayHeight * rayVisualStretch.Y, rayDist * rayVisualStretch.Z);

		float sunWidth = 40;
		effect.Parameters["taperRatio"].SetValue(sunWidth / rayFinalDimensions.X);

		var square = new SquarePrimitive
		{
			Color = Color.White,
			Height = rayFinalDimensions.Y,
			Length = rayFinalDimensions.X,
			BottomPosOffset = rayFinalDimensions.Z
		};

		square.SetTopPosition(Projectile.Center - Main.screenPosition);

		PrimitiveRenderer.DrawPrimitiveShape(square, effect);
	}

	private void DrawBloom(Color glowColor)
	{
		Texture2D bloomtex = AssetLoader.LoadedTextures["Bloom"];
		float numBloom = 3;
		for (int i = 0; i < numBloom; i++)
		{
			float progress = 1 / numBloom;
			var center = Projectile.Center - Main.screenPosition - new Vector2(Projectile.scale);
			float scale = Projectile.scale * MathHelper.Lerp(0.3f, 0.4f, progress);
			var origin = bloomtex.Size() / 2;
			Color color = Projectile.GetAlpha(glowColor);
			Main.spriteBatch.Draw(bloomtex, center, null, color, 0, origin, scale, SpriteEffects.None, 0);
		}
	}

	private void DrawGodrays(Color rayColor, Color darkRayColor, float bigRayAngle)
	{
		Texture2D raytex = AssetLoader.LoadedTextures["Ray"];
		int numRays = 15;
		for (int i = 0; i < numRays; i++)
		{
			float timeSpeed = Main.GlobalTimeWrappedHourly * 4;
			var center = Projectile.Center - Main.screenPosition;
			var rayScale = new Vector2(2f, 1) * MathHelper.Lerp(GetFlashProgress, 0.5f, 0.9f);
			Color color = Projectile.GetAlpha(rayColor);
			var origin = new Vector2(raytex.Width / 2, 0);
			float rotation = timeSpeed + (MathHelper.TwoPi * i / numRays);
			if (i % 3 == 0) //Smaller inverse rotation rays
			{
				rayScale.Y *= 0.9f;
				rotation = -rotation + timeSpeed / 2;
			}

			//Reduce size and opacity based on how far the small ray is from the big one
			float angleDist = Math.Abs(MathHelper.WrapAngle(bigRayAngle - rotation)) / MathHelper.TwoPi;
			angleDist = EaseFunction.EaseCircularOut.Ease(angleDist);
			angleDist = MathHelper.Lerp(1 - angleDist, 1, 0.3f);
			rayScale.Y *= angleDist;
			color *= angleDist;
			darkRayColor *= angleDist;

			Main.spriteBatch.Draw(raytex, center, null, darkRayColor, rotation, origin, rayScale * Projectile.scale * 1.2f, SpriteEffects.None, 0);
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

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		if (target.type is NPCID.Vampire or NPCID.VampireBat)
			modifiers.SourceDamage *= 100f; //silly
	}
}