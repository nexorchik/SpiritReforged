using SpiritReforged.Common.ProjectileCommon;
using Terraria.Audio;

namespace SpiritReforged.Content.Ocean.Items.Reefhunter.Projectiles;

public class ReefSpearThrown : ModProjectile
{
	public override string Texture => Mod.Name + "/Content/Ocean/Items/Reefhunter/Projectiles/ReefSpearProjectile";

	private bool hasTarget = false;
	private Vector2 relativePoint = Vector2.Zero;

	public override LocalizedText DisplayName => Language.GetText("Mods.SpiritReforged.Items.ReefSpear.DisplayName");

	public override void SetStaticDefaults()
	{
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
		ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
	}

	public override void SetDefaults()
	{
		Projectile.width = 30;
		Projectile.height = 30;
		Projectile.friendly = true;
		Projectile.penetrate = -1;
		Projectile.tileCollide = true;
		Projectile.ignoreWater = true;
		Projectile.DamageType = DamageClass.Melee;
		Projectile.aiStyle = 0;
	}

	public override bool? CanDamage() => !hasTarget;
	public override bool? CanCutTiles() => !hasTarget;

	public override void AI()
	{
		if (!hasTarget)
		{
			Projectile.velocity.Y += 0.4f;
			Projectile.rotation = Projectile.velocity.ToRotation();
			Projectile.tileCollide = Projectile.ai[0]++ > 6;
		}
		else
		{
			NPC npc = Main.npc[(int)Projectile.ai[1]];

			if (!npc.active)
			{
				Projectile.netUpdate = true;
				Projectile.tileCollide = true;
				Projectile.timeLeft *= 2;
				Projectile.velocity *= 0;

				hasTarget = false;
				return;
			}

			Projectile.ai[0]++;
			float factor = 1 - Projectile.ai[0] / 10f;
			if (Projectile.ai[0] >= 10f)
				factor = 0;

			relativePoint += Projectile.velocity * factor * 0.1f;

			Projectile.Center = npc.Center + relativePoint;

			if (Projectile.timeLeft <= 10)
				Projectile.alpha += 25;
		}
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		Projectile.ai[0] = 0;
		Projectile.ai[1] = target.whoAmI;
		Projectile.tileCollide = false;
		Projectile.netUpdate = true;
		Projectile.timeLeft = 240;

		target.AddBuff(BuffID.Poisoned, 300);

		hasTarget = true;
		relativePoint = Projectile.Center - target.Center;
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D projTex = TextureAssets.Projectile[Projectile.type].Value;
		const int halfTipWidth = 15;
		var drawOrigin = new Vector2(Projectile.spriteDirection > 0 ? projTex.Width - halfTipWidth : halfTipWidth, projTex.Height / 2);

		if (!hasTarget)
			Projectile.QuickDrawTrail(Main.spriteBatch, 0.25f, drawOrigin: drawOrigin);

		Projectile.QuickDraw(Main.spriteBatch, origin: drawOrigin);
		return false;
	}

	public override void OnKill(int timeLeft)
	{
		if(!hasTarget)
		{
			SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
			Vector2 goreVel = Projectile.oldVelocity / 2;
			Vector2 pos = Projectile.Center;
			for (int i = 1; i <= 6; i++)
			{
				if (i >= 4)
					pos -= Vector2.Normalize(Projectile.oldVelocity) * (15 + (i - 3) * 3);

				var g = Gore.NewGorePerfect(Projectile.GetSource_Death(), pos, goreVel, Mod.Find<ModGore>("Trident" + i).Type);
				g.timeLeft = 0;
				g.rotation = Projectile.rotation;
			}
		}
	}
}
