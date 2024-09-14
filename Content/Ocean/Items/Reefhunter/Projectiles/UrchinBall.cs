using SpiritReforged.Common.Easing;
using SpiritReforged.Common.Misc;
using SpiritReforged.Common.Particle;
using SpiritReforged.Common.PrimitiveRendering;
using SpiritReforged.Common.PrimitiveRendering.Trail_Components;
using SpiritReforged.Common.ProjectileCommon;
using SpiritReforged.Content.Ocean.Items.Reefhunter.Particles;
using SpiritReforged.Content.Particles;
using System.IO;
using Terraria.Audio;

namespace SpiritReforged.Content.Ocean.Items.Reefhunter.Projectiles;

public class UrchinBall : ModProjectile, ITrailProjectile
{
	private bool hasTarget = false;
	private Vector2 relativePoint = Vector2.Zero;

	private bool stuckInTile = false;
	private Point stuckTilePos = new(0, 0);
	private int squishTime = 0;

	private static Asset<Texture2D> GlowmaskTexture;

	private const int MAX_LIFETIME = 180;
	private const int DETONATION_TIME = 90;
	public const float MAX_SPEED = 10f; //Used by the staff to shoot the projectile
	private const int MAX_SQUISHTIME = 20;

	public override void SetStaticDefaults()
	{
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
		ProjectileID.Sets.TrailingMode[Projectile.type] = 2;

		if (!Main.dedServ)
			GlowmaskTexture = ModContent.Request<Texture2D>(Texture + "Glow");
	}

	public override void SetDefaults()
	{
		Projectile.width = 16;
		Projectile.height = 16;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.friendly = true;
		Projectile.penetrate = -1;
		Projectile.aiStyle = 0;
	}

	public void DoTrailCreation(TrailManager tm)
	{
		tm.CreateTrail(Projectile, new LightColorTrail(new Color(87, 35, 88) * 0.3f, Color.Transparent), new RoundCap(), new DefaultTrailPosition(), 30, 100);
		tm.CreateTrail(Projectile, new LightColorTrail(new Color(87, 35, 88) * 0.3f, Color.Transparent), new RoundCap(), new DefaultTrailPosition(), 15, 50);
	}

	public override bool? CanDamage() => !hasTarget;
	public override bool? CanCutTiles() => !hasTarget;

	public override void AI()
	{
		if (!hasTarget)
		{
			if (stuckInTile) //Check if tile it's stuck in is still active
			{
				Projectile.velocity = Vector2.Zero;
				if (!Main.tile[stuckTilePos.X, stuckTilePos.Y].HasTile) //If not, update and let the projectile fall again
				{
					stuckInTile = false;
					stuckTilePos = new Point(0, 0);
					Projectile.netUpdate = true;
				}
			}
			else
			{
				Projectile.velocity.Y += 0.25f;
				Projectile.rotation += 0.1f * Math.Sign(Projectile.velocity.X);
			}
		}

		else
		{
			NPC npc = Main.npc[(int)Projectile.ai[1]];

			if (!npc.CanBeChasedBy(this) && npc.type != NPCID.TargetDummy)
			{
				Projectile.netUpdate = true;
				Projectile.tileCollide = true;
				Projectile.timeLeft *= 2;

				hasTarget = false;
				return;
			}

			Projectile.Center = npc.Center + relativePoint;
		}

		Projectile.scale = 1 - FlashStrength() / MathHelper.Lerp(6, 3, FlashTimer());
		squishTime = Math.Max(squishTime - 1, 0);
		float squishScale = EaseFunction.EaseCubicIn.Ease((float)squishTime / MAX_SQUISHTIME);
		squishScale = 1 - (float)Math.Sin(MathHelper.Pi * squishScale);

		Projectile.scale *= MathHelper.Lerp(squishScale, 1, 0.85f);
	}

	private float FlashTimer() => Math.Max(DETONATION_TIME - Projectile.timeLeft, 0) / (float)DETONATION_TIME;
	private float FlashStrength()
	{
		int numFlashes = 6;
		return EaseFunction.EaseQuarticIn.Ease((float)Math.Sin(EaseFunction.EaseQuadIn.Ease(FlashTimer()) * numFlashes * MathHelper.Pi));
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		if (Projectile.timeLeft > MAX_LIFETIME)
			Projectile.timeLeft = MAX_LIFETIME;

		Projectile.velocity = Vector2.Zero;
		stuckInTile = true;
		stuckTilePos = (Projectile.Center + oldVelocity).ToTileCoordinates();
		Projectile.netUpdate = true;
		HitEffects(oldVelocity);

		return false;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		HitEffects(Projectile.velocity);
		Projectile.ai[1] = target.whoAmI;
		Projectile.tileCollide = false;
		Projectile.netUpdate = true;
		Projectile.timeLeft = MAX_LIFETIME;
		Projectile.velocity = new Vector2(0, -0.4f);

		hasTarget = true;
		relativePoint = Projectile.Center - target.Center;
	}

	private static Color lightColor = new(145, 67, 111);
	private static Color darkColor = new(87, 35, 88);

	private void HitEffects(Vector2 velocity)
	{
		if (Main.dedServ)
			return;

		float velocityRatio = Math.Min(velocity.Length() / MAX_SPEED, 1);

		int particleLifetime = 20;
		float particleLength = 60 * velocityRatio;

		var noiseCone = new MotionNoiseCone(Projectile.Center - Vector2.Normalize(velocity) * (particleLength * 0.2f), darkColor, lightColor,
			particleLength * 3.5f, particleLength, velocity.ToRotation() + MathHelper.Pi, particleLifetime, 1f);
		noiseCone = noiseCone.SetExtraData(true, 2.5f, 12, 2f, 1.25f * velocityRatio, 1.2f);
		noiseCone.Velocity = Vector2.Normalize(velocity) * velocityRatio;
		ParticleHandler.SpawnParticle(noiseCone);

		AssetLoader.VertexTrailManager.TryEndTrail(Projectile, 12);
		squishTime = MAX_SQUISHTIME;
	}

	public override void OnKill(int timeLeft)
	{
		for (int i = 0; i < 8; ++i)
		{
			Vector2 vel = new Vector2(Main.rand.NextFloat(3f, 4f), 0).RotatedBy(i * MathHelper.TwoPi / 8f).RotatedByRandom(0.33f);
			Vector2 spawnPos = Projectile.Center + (hasTarget ? Vector2.Normalize(relativePoint) * 6 : vel);
			Projectile.NewProjectile(Projectile.GetSource_Death(), spawnPos, vel, ModContent.ProjectileType<UrchinSpike>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
		}

		float angle = Main.rand.NextFloat(-0.1f, 0.1f) - MathHelper.PiOver2;

		for(int i = 0; i < 2; i++)
			ParticleHandler.SpawnParticle(new TexturedPulseCircle(
				Projectile.Center + Main.rand.NextVec2CircularEven(5, 5),
				darkColor * 0.5f,
				darkColor * 0.1f,
				0.35f + Main.rand.NextFloat(-0.1f, 0.1f),
				350 + Main.rand.NextFloat(-50, 100),
				25 + Main.rand.Next(11),
				"noise",
				new Vector2(5, 0.5f),
				(i == 0) ? EaseFunction.EaseQuadOut : EaseFunction.EaseCubicOut).UsesLightColor().WithSkew(Main.rand.NextFloat(0.7f, 0.9f), angle + Main.rand.NextFloat(0.3f, -0.3f)));

		for (int i = -1; i < 2; i += 2)
		{
			Vector2 baseDirection = Vector2.UnitX.RotatedBy(angle) * i;
			for(int j = 0; j < Main.rand.Next(3, 6); j++)
			{
				Vector2 pos = Projectile.Center;
				Vector2 vel = baseDirection.RotatedByRandom(MathHelper.Pi / 3) * Main.rand.NextFloat(3, 6);
				float scale = Main.rand.NextFloat(0.5f, 1f);
				int maxTime = Main.rand.Next(20, 30);

				ParticleHandler.SpawnParticle(Main.rand.NextBool() ? new UrchinShard(pos, vel, scale, maxTime) : new UrchinShardAlt(pos, vel, scale, maxTime));
			}
		}

		SoundEngine.PlaySound(SoundID.Item14 with { PitchVariance = 0.2f, Volume = 0.4f }, Projectile.Center);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		if(!hasTarget && !stuckInTile)
			Projectile.QuickDrawTrail(Main.spriteBatch, 0.33f);

		Projectile.QuickDraw(Main.spriteBatch);

		return false;
	}

	public override void PostDraw(Color lightColor)
	{
		Texture2D tex = GlowmaskTexture.Value;

		float alpha = 1 - FlashStrength();

		Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.OrangeRed.Additive() * (1 - alpha) * 0.5f, Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0f);
	}

	public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
	{
		width /= 3;
		height /= 3;
		return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
	}

	public override void SendExtraAI(BinaryWriter writer)
	{
		writer.Write(hasTarget);
		writer.Write(stuckInTile);
		writer.WriteVector2(relativePoint);
		writer.WriteVector2(stuckTilePos.ToVector2());
	}

	public override void ReceiveExtraAI(BinaryReader reader)
	{
		hasTarget = reader.ReadBoolean();
		stuckInTile = reader.ReadBoolean();
		relativePoint = reader.ReadVector2();
		stuckTilePos = reader.ReadVector2().ToPoint();
	}
}

public class UrchinSpike : ModProjectile, ITrailProjectile
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("Urchin");
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
		ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
	}

	private bool hasTarget = false;
	private Vector2 relativePoint = Vector2.Zero;

	public override void SetDefaults()
	{
		Projectile.width = 6;
		Projectile.height = 6;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.friendly = true;
		Projectile.penetrate = 1;
		Projectile.aiStyle = 0;
		Projectile.extraUpdates = 3;
		Projectile.timeLeft = 60;
		Projectile.scale = Main.rand.NextFloat(0.7f, 1.1f);
	}

	public void DoTrailCreation(TrailManager tm) => tm.CreateTrail(Projectile, new LightColorTrail(new Color(87, 35, 88) * 0.2f, Color.Transparent), new RoundCap(), new DefaultTrailPosition(), 12 * Projectile.scale, 75);

	public override bool? CanDamage()/* tModPorter Suggestion: Return null instead of false */ => !hasTarget;
	public override bool? CanCutTiles() => !hasTarget;

	public override void AI()
	{
		Projectile.alpha = (255 - (int)(Projectile.timeLeft / 60f * 255));
		Projectile.scale = EaseFunction.EaseCircularOut.Ease(Projectile.Opacity);
		Projectile.velocity *= 0.96f;
		if (!hasTarget)
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

		else
		{
			NPC npc = Main.npc[(int)Projectile.ai[1]];
			Projectile.velocity *= 0.9f;
			relativePoint += Projectile.velocity;

			if (!npc.active)
				Projectile.Kill();
			else
				Projectile.Center = npc.Center + relativePoint;
		}
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		Projectile.ai[1] = target.whoAmI;
		Projectile.tileCollide = false;
		Projectile.netUpdate = true;
		Projectile.alpha = 0;
		Projectile.penetrate++;
		Projectile.velocity *= 0.7f;

		hasTarget = true;
		relativePoint = Projectile.Center - target.Center;
		AssetLoader.VertexTrailManager.TryEndTrail(Projectile, 12);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Projectile.QuickDraw();
		Projectile.QuickDrawTrail(baseOpacity: 0.25f);
		return false;
	}
}
