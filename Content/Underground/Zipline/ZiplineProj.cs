using SpiritReforged.Common.Misc;
using SpiritReforged.Common.Particle;
using SpiritReforged.Common.ProjectileCommon;
using SpiritReforged.Content.Particles;
using System.IO;
using Terraria.Audio;

namespace SpiritReforged.Content.Underground.Zipline;

public class ZiplineProj : ModProjectile
{
	public Vector2 cursorPoint;

	public override void SetDefaults()
	{
		Projectile.extraUpdates = 3;
		Projectile.tileCollide = false;
		Projectile.alpha = 255;
	}

	public override void AI()
	{
		Projectile.Opacity = MathHelper.Min(Projectile.Opacity + .025f, 1);
		Projectile.rotation = Projectile.velocity.ToRotation();

		float length = Projectile.velocity.Length();
		if (Projectile.Distance(cursorPoint) <= length)
		{
			Projectile.Center = cursorPoint;
			Projectile.Kill();
		}
	}

	public override void OnKill(int timeLeft)
	{
		bool removed = false;

		foreach (var zipline in ZiplineHandler.ziplines)
		{
			if (zipline.Owner == Main.player[Projectile.owner] && zipline.Contains(Projectile.Center.ToPoint(), out var contained))
			{
				zipline.RemovePoint(contained);
				removed = true;
			}
		}

		if (!removed)
			ZiplineHandler.Add(Main.player[Projectile.owner], Projectile.Center);

		ParticleHandler.SpawnParticle(new TexturedPulseCircle(Projectile.Center, (Color.Goldenrod * .75f).Additive(), 1, 100, 30, "Bloom", new Vector2(1), Common.Easing.EaseFunction.EaseCircularOut));
		ParticleHandler.SpawnParticle(new TexturedPulseCircle(Projectile.Center, (Color.White * .5f).Additive(), 1, 100, 20, "Bloom", new Vector2(1), Common.Easing.EaseFunction.EaseCircularOut));

		for (int i = 0; i < 12; i++)
			Dust.NewDustPerfect(Projectile.Center, DustID.AmberBolt, Main.rand.NextVector2Unit() * Main.rand.NextFloat(5f), Scale: Main.rand.NextFloat(.5f, 1.5f)).noGravity = true;

		SoundEngine.PlaySound(SoundID.Item101 with { Pitch = .25f }, Projectile.Center);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		var trail = AssetLoader.LoadedTextures["Ray"];
		var scale = new Vector2(.45f, 1f) * Projectile.scale;

		Main.EntitySpriteDraw(trail, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.Orange.Additive()), 
			Projectile.rotation + MathHelper.PiOver2, new Vector2(trail.Width / 2, 0), scale, default);

		Main.EntitySpriteDraw(trail, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White.Additive()),
			Projectile.rotation + MathHelper.PiOver2, new Vector2(trail.Width / 2, 0), scale * .75f, default);

		Projectile.QuickDraw();

		return false;
	}

	public override void SendExtraAI(BinaryWriter writer) => writer.WriteVector2(cursorPoint);
	public override void ReceiveExtraAI(BinaryReader reader) => cursorPoint = reader.ReadVector2();
}
