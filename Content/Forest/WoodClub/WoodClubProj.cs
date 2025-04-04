using SpiritReforged.Common.ProjectileCommon;
using SpiritReforged.Common.Particle;
using SpiritReforged.Content.Particles;
using static SpiritReforged.Common.Easing.EaseFunction;
using SpiritReforged.Common.Easing;
using SpiritReforged.Common.Misc;

namespace SpiritReforged.Content.Forest.WoodClub;

class WoodClubProj : BaseClubProj
{
	public WoodClubProj() : base(new Vector2(58)) { }

	public override void SafeSetStaticDefaults() => Main.projFrames[Projectile.type] = 2;

	public override void Smash(Vector2 position)
	{
		float chargeFactor = MathHelper.Clamp(EaseQuadIn.Ease(Charge), 0.33f, 1f);
		float chargeFactorLerped = MathHelper.Lerp(chargeFactor, 1, 0.5f);

		Collision.HitTiles(Projectile.position, Vector2.UnitY, Projectile.width, Projectile.height);

		for(int i = 0; i < 8 * chargeFactorLerped; i++)
		{
			Vector2 smokePos = Projectile.Bottom + Vector2.UnitX * Main.rand.NextFloat(-20, 20);

			float scale = Main.rand.NextFloat(0.06f, 0.07f);
			scale *= 1 + chargeFactor/2;

			float speed = Main.rand.NextFloat(4);
			speed *= chargeFactorLerped;
			speed += 0.5f;

			int lifeTime = (int)(40 + Main.rand.Next(-10, 16) * (1 + chargeFactor));

			ParticleHandler.SpawnParticle(new SmokeCloud(smokePos, -Vector2.UnitY * speed, Color.LightGray, scale, EaseQuadOut, lifeTime));
		}

		if(Charge == 1)
			DoShockwaveCircle(Vector2.Lerp(Projectile.Center, Main.player[Projectile.owner].Center, 0.275f), 280, -MathHelper.PiOver4 * Projectile.direction * 1.5f, 0.7f);

		DoShockwaveCircle(Projectile.Bottom, 180, -MathHelper.PiOver2, MathHelper.Lerp(0.5f, 0.7f, Charge));
	}

	private static void DoShockwaveCircle(Vector2 pos, float size, float xyRotation, float opacity)
	{
		var easeFunction = EaseQuadOut;
		float ringWidth = 0.6f;
		int lifetime = 20;
		float zRotation = 0.85f;

		ParticleHandler.SpawnParticle(new TexturedPulseCircle(
			pos,
			Color.LightGray * opacity,
			Color.LightGray * opacity,
			ringWidth,
			size,
			lifetime,
			"supPerlin",
			new Vector2(1, 1.5f),
			easeFunction, false, ringWidth / 3).WithSkew(zRotation, xyRotation).UsesLightColor());
	}
}
