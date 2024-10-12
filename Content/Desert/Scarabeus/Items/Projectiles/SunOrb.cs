using SpiritReforged.Common.Misc;
using SpiritReforged.Common.Particle;
using SpiritReforged.Common.PrimitiveRendering.PrimitiveShape;
using SpiritReforged.Common.PrimitiveRendering;
using SpiritReforged.Common.ProjectileCommon;
using SpiritReforged.Common.Visuals.Glowmasks;
using SpiritReforged.Content.Particles;
using static SpiritReforged.Common.Easing.EaseFunction;
using static Microsoft.Xna.Framework.MathHelper;

namespace SpiritReforged.Content.Desert.Scarabeus.Items.Projectiles;

[AutoloadGlowmask("255,255,255", false)]
public class SunOrb : ModProjectile
{
	private const int GROWTIME = 30;
	private const int FLASHTIME = 70;
	private const int NUMHITS = 4;

	private const float CAN_HIT_THRESHOLD = 0.66f; //What % of the flash progress is needed to actually do damage

	private float AiTimer { get => Projectile.ai[0]; set => Projectile.ai[0] = value; }
	private float FlashTimer { get => Projectile.ai[1]; set => Projectile.ai[1] = value; }
	private float ParentProjID => Projectile.ai[2];

	private Vector2 _offset;
	private Vector2 _mousePos;
	private Vector3 _rayScale;

	private bool _stoppedChannel = false;
	private bool _initialized = false;

	public override void SetDefaults()
	{
		Projectile.width = 34;
		Projectile.height = 34;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.friendly = true;
		Projectile.penetrate = -1;
		Projectile.aiStyle = 0;
		Projectile.scale = 0;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = (int)((FLASHTIME / NUMHITS) * (1 - CAN_HIT_THRESHOLD)) + 1;
	}

	public override bool? CanCutTiles() => false;

	public override void AI()
	{
		Projectile.TryGetOwner(out Player owner);
		Projectile.timeLeft = 2;

		if (owner.dead || !IsParentActive(out SunStaffHeld sunStaff) && !_stoppedChannel)
		{
			Projectile.Kill();
			return;
		}

		if (!_initialized)
		{
			Projectile.netUpdate = true;
			_offset = Projectile.Center - owner.Center;
			_initialized = true;
		}

		//Only change the stored mouse position on the owner's client- and only do it if the player is still channeling
		if(Main.myPlayer == owner.whoAmI && !_stoppedChannel)
		{
			_mousePos = Vector2.Lerp(_mousePos, Main.MouseWorld - owner.Center, 0.07f);
			Projectile.netUpdate = true;
		}

		//Move the orb slightly towards the mouse so you can control it a little
		Vector2 mouseOffset = Vector2.Normalize(_mousePos) * Min(_mousePos.Length() / 5, 40) * new Vector2(0.5f, 1f);

		Projectile.Center = Vector2.Lerp(Projectile.Center, owner.MountedCenter + _offset + owner.velocity + mouseOffset, 0.2f);

		GrowShrink();
		GetRayDimensions(out float rayHeight, out _, out float rayDist);

		float earlyChannelStopTime = GROWTIME * 0.8f;
		if (!owner.channel && !_stoppedChannel && AiTimer >= earlyChannelStopTime)
		{
			_stoppedChannel = true;
			Projectile.netUpdate = true;
			FlashTimer = FLASHTIME;
			sunStaff.StartStaffLowering();
		}

		//Create light at the sun's position and following the ray
		float lightStrength = Projectile.scale / 2 + GetFlashProgress / 2;
		Lighting.AddLight(Projectile.Center, Color.Goldenrod.ToVector3() * lightStrength);
		float numLight = 10;
		for(int i = 0; i < numLight; i++)
		{
			Vector2 pos = new Vector2(rayDist, rayHeight) * i / numLight;
			Lighting.AddLight(Projectile.Center + pos, Color.Goldenrod.ToVector3() * lightStrength * 0.5f);
		}

		AiTimer++;
	}

	private bool IsParentActive(out SunStaffHeld sunStaff)
	{
		sunStaff = null;
		Projectile parent = Main.projectile[(int)ParentProjID];
		if (!parent.active || parent.ModProjectile == null)
			return false;

		if(parent.ModProjectile is SunStaffHeld staff)
		{
			sunStaff = staff;
			return true;
		}

		return false;
	}

	private void GrowShrink()
	{
		float progress;
		if (!_stoppedChannel) //Grow if still channeling
		{
			progress = AiTimer / GROWTIME;
			progress = Min(progress, 1);

			Projectile.scale = CompoundEase(EaseCircularIn, EaseOutBack(2), progress, 0.2f);
			_rayScale = new Vector3(EaseQuadOut.Ease(Min(progress, 1)) * Projectile.scale);
			_rayScale.X *= Projectile.scale;
		}
		else
		{
			FlashTimer = Max(FlashTimer - 1, 0);
			progress = GetFlashProgress;
			progress = Max(progress, 0);
			if (progress == 0)
				Projectile.Kill();

			Projectile.scale = CompoundEase(EaseQuadIn, EaseCubicOut, progress, 0.4f) * Lerp(1, 1.3f, progress);
			_rayScale = new Vector3(EaseQuadOut.Ease(Min(Projectile.scale, 1)));
			_rayScale.X *= Lerp(0.5f, 1.2f, EaseCircularIn.Ease(progress)) * Projectile.scale;
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
			float scale = Lerp(0.08f, 0.04f, progress);
			var vel = Vector2.Lerp(-Vector2.UnitY / 3, -Vector2.UnitY * 2, progress);
			ParticleHandler.SpawnParticle(new DissipatingImage(target.Center, smokeColor, Main.rand.NextFloatDirection(), scale, 0.6f, "Smoke", 40) { Velocity = vel });
		}

		for (int i = 0; i < 6; i++)
		{
			Vector2 particleCenter = target.Center + Main.rand.NextVector2Circular(15, 15);
			Vector2 particleVel = -Vector2.UnitY.RotatedByRandom(PiOver4) * Main.rand.NextFloat(1, 4);
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
		if (GetFlashProgress < CAN_HIT_THRESHOLD)
			return false;

		GetRayDimensions(out float rayHeight, out float rayWidth, out float rayDist);
		var tip = Projectile.Center.ToPoint();
		int numCalcs = 20;

		//This def can be done more efficiently but for now this just uses a summation to estimate the hitbox
		for(int i = 0; i < numCalcs; i++)
		{
			float progress = i / (float)numCalcs;
			int curHeight = (int)(progress * rayHeight);
			int curWidth = (int)Lerp(Projectile.width, rayWidth, progress);
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
		Vector2 projCenter = Projectile.Center - owner.Center;
		float mouseAngle = projCenter.AngleTo(_mousePos) + Pi;

		//Constrain the angle to be downwards from the sun- can't ever be exactly horizontal, because the below while loop would be infinite
		float minAngle = -0.01f;
		if (mouseAngle < PiOver2)
			mouseAngle = minAngle;
		else if (mouseAngle < Pi)
			mouseAngle = Pi + minAngle;
		mouseAngle += Pi;

		float maxDist = 200f;
		//Set the ray's height to be at least reach the player from the base offset- then extend it based on mouse position, up to the set max distance
		rayHeight = -Min(_offset.Y, Max(-_mousePos.Y + _offset.Y, -maxDist));

		//Get a distance unit used for calculating how far the beam goes- then extend it until that distance unit matches the beam's height
		Vector2 mouseDirection = mouseAngle.ToRotationVector2() * maxDist;
		while (mouseDirection.Y < -rayHeight)
			mouseDirection += mouseAngle.ToRotationVector2();

		//Constrain the ray's distance
		rayDist = Clamp(mouseDirection.X, -maxDist, maxDist);

		//Scale the width based on how far the ray is from the player
		var widthRange = new Vector2(100, 180);
		rayWidth = Lerp(widthRange.X, widthRange.Y, Math.Abs(rayDist) / maxDist);
	}

	private float GetFlashProgress => FlashTimer / FLASHTIME;

	public override bool PreDraw(ref Color lightColor)
	{
		float strength = Lerp(GetFlashProgress, 1, 0.25f);
		var rayColor = Color.LightGoldenrodYellow.Additive(100) * strength;
		var darkRayColor = new Color(250, 167, 32, 200);

		DrawBloom(rayColor, darkRayColor * strength);
		DrawGodrays(rayColor, darkRayColor * strength, 0);

		GetRayDimensions(out float rayHeight, out float rayWidth, out float rayDist);
		DrawBigRay(rayColor * 0.5f, darkRayColor * strength, rayHeight, rayWidth, rayDist);

		DrawGlowmask(Color.Lerp(darkRayColor, rayColor, EaseCircularIn.Ease(GetFlashProgress)));
		Projectile.QuickDraw(rot: 0, drawColor: Color.White.Additive(240) * Projectile.Opacity);

		return false;
	}

	private void DrawBigRay(Color rayColor, Color darkRayColor, float rayHeight, float rayWidth, float rayDist)
	{
		Effect effect = AssetLoader.LoadedShaders["LightRay"];

		effect.Parameters["uTexture"].SetValue(AssetLoader.LoadedTextures["FlameTrail"]);
		float scrollAmount = (EaseCircularIn.Ease(GetFlashProgress) * 0.4f);
		effect.Parameters["scroll"].SetValue(new Vector2(scrollAmount * Math.Sign(rayDist), scrollAmount));
		effect.Parameters["textureStretch"].SetValue(new Vector2(0.4f, 0.05f) * 0.5f);
		effect.Parameters["texExponentRange"].SetValue(new Vector2(1f, 0.25f));
		effect.Parameters["flipCoords"].SetValue(true);

		float easedScale = EaseCircularIn.Ease(Clamp(Projectile.scale, 0, 1));
		float easedFlashProgress = EaseCubicIn.Ease(GetFlashProgress);
		effect.Parameters["finalIntensityMod"].SetValue((2 + 2 * easedFlashProgress) * easedScale * Projectile.scale);
		effect.Parameters["textureStrength"].SetValue(easedFlashProgress); //Don't display texture while not flashing
		effect.Parameters["finalExponent"].SetValue(2.5f);

		effect.Parameters["uColor"].SetValue(rayColor.ToVector4());
		effect.Parameters["uColor2"].SetValue(darkRayColor.ToVector4());

		var rayVisualStretch = new Vector3(2.5f, 1.3f, 1.15f); //Stretch the primitive to better fit the actual hitbox, since shader makes it innaccurate
		rayVisualStretch *= _rayScale;

		var rayFinalDimensions = new Vector3(rayWidth * rayVisualStretch.X, rayHeight * rayVisualStretch.Y, rayDist * rayVisualStretch.Z);

		float sunWidth = 40 * Projectile.scale * Lerp(_rayScale.X, 1, 0.5f);
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

	private void DrawBloom(Color lightColor, Color darkColor)
	{
		Texture2D bloomtex = AssetLoader.LoadedTextures["Bloom"];
		float numBloom = 2;
		var center = Projectile.Center - Main.screenPosition - new Vector2(Projectile.scale);
		Vector2 projSize = Projectile.scale * Projectile.Size;
		float smallSize = 4.65f * Projectile.scale;
		float bigSize = 6.5f * Projectile.scale;
		float easedFlashProgress = EaseCircularIn.Ease(GetFlashProgress);

		for (int i = 0; i < numBloom; i++)
		{
			float progress = i / numBloom;
			Vector2 scale = 10 * (projSize + new Vector2(Lerp(smallSize, bigSize, progress * easedFlashProgress))) / bloomtex.Size();
			var origin = bloomtex.Size() / 2;
			Color color = Projectile.GetAlpha(Color.Lerp(lightColor, darkColor, easedFlashProgress).Additive());
			Main.spriteBatch.Draw(bloomtex, center, null, color, 0, origin, scale.X, SpriteEffects.None, 0);
		}
	}
	private void DrawGodrays(Color rayColor, Color darkRayColor, float bigRayAngle)
	{
		Texture2D raytex = AssetLoader.LoadedTextures["Ray"];
		int numRays = 12;
		for (int i = 0; i < numRays; i++)
		{
			float timeSpeed = EaseCircularIn.Ease(GetFlashProgress) * 4f;
			var center = Projectile.Center - Main.screenPosition;
			var rayScale = new Vector2(1.5f, 0.6f) * Lerp(GetFlashProgress, 0.5f, 0.8f);
			Color color = Projectile.GetAlpha(rayColor);
			var origin = new Vector2(raytex.Width / 2, 0);
			float rotation = timeSpeed + (TwoPi * i / numRays);
			if (i % 3 == 0) //Smaller inverse rotation rays
			{
				rayScale.Y *= 0.9f;
				rotation = -rotation + timeSpeed / 2;
			}

			rayScale.Y *= EaseQuadOut.Ease(GetFlashProgress);
			color *= EaseQuadIn.Ease(GetFlashProgress);
			darkRayColor *= EaseQuadIn.Ease(GetFlashProgress);

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
			float distance = Lerp(1.5f, 3, EaseCircularIn.Ease(GetFlashProgress)) * Projectile.scale;
			position += Vector2.UnitX.RotatedBy(TwoPi * i / numGlow) * distance;
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