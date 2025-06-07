using SpiritReforged.Common.ItemCommon;
using SpiritReforged.Common.PrimitiveRendering.CustomTrails;
using SpiritReforged.Common.PrimitiveRendering;
using SpiritReforged.Common.ProjectileCommon.Abstract;
using SpiritReforged.Common.Visuals.Glowmasks;
using SpiritReforged.Content.Dusts;
using SpiritReforged.Common.Particle;
using SpiritReforged.Content.Particles;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using SpiritReforged.Common.ModCompat;
using SpiritReforged.Common.ItemCommon.Abstract;

namespace SpiritReforged.Content.Underground.Items;

[AutoloadGlowmask("255,255,255")]
public class Blasphemer : ClubItem
{
	public override bool IsLoadingEnabled(Mod mod) => false;

	internal override float DamageScaling => 2.5f;

	public override void SetStaticDefaults()
	{
		ItemLootDatabase.AddItemRule(ItemID.ObsidianLockbox, ItemDropRule.Common(Type, 5));

		MoRHelper.AddElement(Item, MoRHelper.Fire, true);
	}
	public override void SafeSetDefaults()
	{
		Item.damage = 38;
		Item.knockBack = 6;
		ChargeTime = 40;
		SwingTime = 24;
		Item.width = 60;
		Item.height = 60;
		Item.crit = 8;
		Item.value = Item.sellPrice(0, 1, 20, 0);
		Item.rare = ItemRarityID.Orange;
		Item.shoot = ModContent.ProjectileType<BlasphemerProj>();
	}
}

[AutoloadGlowmask("255,255,255", false)]
class BlasphemerProj : BaseClubProj, IManualTrailProjectile
{
	public BlasphemerProj() : base(new Vector2(104)) { }

	public override float WindupTimeRatio => 0.8f;

	public override bool IsLoadingEnabled(Mod mod) => false;
	public void DoTrailCreation(TrailManager tM)
	{
		float trailDist = 80 * MeleeSizeModifier;
		float trailWidth = 30 * MeleeSizeModifier;
		float angleRangeMod = 1f;
		float rotOffset = -0.1f;

		if (FullCharge)
		{
			trailDist *= 1.1f;
			trailWidth *= 1.1f;
			angleRangeMod = 1.2f;
			rotOffset = -MathHelper.PiOver4 / 2;
		}

		SwingTrailParameters parameters = new(AngleRange * angleRangeMod, -HoldAngle_Final + rotOffset, trailDist, trailWidth)
		{
			Color = Color.White,
			SecondaryColor = Color.DarkSlateBlue,
			TrailLength = 0.33f,
			Intensity = 0.5f,
		};

		tM.CreateCustomTrail(new SwingTrail(Projectile, parameters, GetSwingProgressStatic, SwingTrail.BasicSwingShaderParams));

		parameters.Color = Color.Yellow;
		parameters.SecondaryColor = Color.OrangeRed;
		parameters.UseLightColor = false;
		parameters.Intensity = 2f;

		tM.CreateCustomTrail(new SwingTrail(Projectile, parameters, GetSwingProgressStatic, s => SwingTrail.NoiseSwingShaderParams(s, "cloudNoise", new Vector2(3f, 0.5f)), TrailLayer.UnderProjectile));
	}

	public override void OnSwingStart() => TrailManager.ManualTrailSpawn(Projectile);

	public override void OnSmash(Vector2 position)
	{
		TrailManager.TryTrailKill(Projectile);
		Collision.HitTiles(Projectile.position, Vector2.UnitY, Projectile.width, Projectile.height);

		if (FullCharge)
		{
			if (Projectile.owner == Main.myPlayer)
				Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, -Vector2.UnitY, ModContent.ProjectileType<Firespike>(),
					(int)(Projectile.damage * DamageScaling * 0.5f), Projectile.knockBack * KnockbackScaling * 0.1f, Projectile.owner);

			for (int i = 0; i < 20; i++)
				Dust.NewDustDirect(Projectile.position - new Vector2(0, 10), Projectile.width, Projectile.height, ModContent.DustType<FireClubDust>(), 0, -Main.rand.NextFloat(5f));

			ParticleHandler.SpawnParticle(new Shatter(position + Vector2.UnitY * 10, Color.OrangeRed * 0.2f, TotalScale, 20));
		}
	}

	public override void SafeAI()
	{
		if (FullCharge && CheckAIState(AIStates.SWINGING))
		{
			int count = Main.rand.Next(1, 4);
			for (int i = 0; i < count; i++)
			{
				var center = Projectile.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(30f);
				var velocity = (Projectile.velocity * Main.rand.NextFloat(3f)).RotatedBy(Projectile.rotation);

				ParticleHandler.SpawnParticle(new EmberParticle(center, velocity, Color.Yellow, Color.Red, Main.rand.NextFloat(0.3f), 100, 5));
			}
		}
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.OnFire, 300);

	public override void SafeDraw(SpriteBatch spriteBatch, Texture2D texture, Color lightColor, Vector2 handPosition, Vector2 drawPosition)
	{
		var glow = GlowmaskProjectile.ProjIdToGlowmask[Type].Glowmask.Value;
		Main.EntitySpriteDraw(glow, drawPosition, null, Projectile.GetAlpha(Color.White * GetWindupProgress), Projectile.rotation, HoldPoint, TotalScale, Effects, 0);
	}
}

class Firespike : ModProjectile
{
	public const int TimeLeftMax = 180;

	public override bool IsLoadingEnabled(Mod mod) => false;
	public override string Texture => "Terraria/Images/Projectile_0";
	public override void SetDefaults()
	{
		Projectile.Size = new Vector2(16);
		Projectile.timeLeft = TimeLeftMax;
		Projectile.friendly = true;
		Projectile.tileCollide = false;
		Projectile.hide = true;
		Projectile.penetrate = -1;
	}

	public override void AI()
	{
		int surfaceDuration = 0;

		while (Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
		{
			Projectile.position.Y--; //Move out of solid tiles

			if (++surfaceDuration > 40)
			{
				Projectile.Kill();
				return;
			}
		}

		float speedY = -2.5f;
		var fire = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, Utils.SelectRandom(Main.rand, 6, 259, 158), 0f, speedY, 200, default, Main.rand.NextFloat() + 1f);
		fire.velocity *= new Vector2(0.5f, 2f);
		fire.position = Projectile.Bottom;

		if (Main.rand.NextBool(5))
		{
			var pos = Projectile.position + Main.rand.NextVector2Unit() * Main.rand.NextFloat(10f);
			var gore = Gore.NewGorePerfect(Projectile.GetSource_FromAI(), pos, (Vector2.UnitY * -Main.rand.NextFloat(3f, 7f)).RotatedByRandom(0.25f), GoreID.Smoke1, Main.rand.NextFloat(0.5f, 2f));
			gore.alpha = 150;
		}

		if (Main.rand.NextBool(10))
		{
			var position = Projectile.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(10f);
			var velocity = (Vector2.UnitY * -Main.rand.NextFloat(1f, 5f)).RotatedByRandom(0.25f);

			ParticleHandler.SpawnParticle(new EmberParticle(position, velocity, Color.OrangeRed, Main.rand.NextFloat(0.5f), 200, 5));
		}

		if (++Projectile.localAI[0] % 30 == 0)
			SoundEngine.PlaySound(SoundID.Item34 with { PitchRange = (-0.2f, 0.2f), MaxInstances = 3 }, Projectile.Center);
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		const int heightOffset = 100;

		if (new Rectangle(projHitbox.X, projHitbox.Y - heightOffset, projHitbox.Width, projHitbox.Height + heightOffset).Intersects(targetHitbox))
			return true;

		return null;
	}

	public override bool ShouldUpdatePosition() => false;
	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.OnFire, 120);
	//Reduce damage with hits
	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) => modifiers.FinalDamage *= Math.Max(0.2f, 1f - Projectile.numHits / 8f);
}