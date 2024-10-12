using SpiritReforged.Common.Particle;
using SpiritReforged.Common.PlayerCommon;
using SpiritReforged.Content.Ocean.Items.Reefhunter;
using SpiritReforged.Content.Ocean.Tiles.Hydrothermal;
using Terraria.Audio;

namespace SpiritReforged.Content.Ocean.Hydrothermal;

public class HydrothermalVentPlume : ModProjectile
{
	private const int timeLeftMax = 120;

	public override string Texture => "Terraria/Images/NPC_0";

	public override void SetDefaults()
	{
		Projectile.ignoreWater = true;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 60 * 10;
	}

	public override void AI()
	{
		if (Projectile.timeLeft == timeLeftMax) //On-spawn effects
		{
			for (int k = 0; k <= 20; k++)
				Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.BoneDust>(), new Vector2(0, 6).RotatedByRandom(1) * Main.rand.NextFloat(-1, 1));
			for (int k = 0; k <= 20; k++)
				Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.FireClubDust>(), new Vector2(0, 6).RotatedByRandom(1) * Main.rand.NextFloat(-1, 1));

			SoundEngine.PlaySound(new SoundStyle("SpiritReforged/Assets/SFX/Ambient/StoneCrack" + Main.rand.Next(1, 3)) { PitchVariance = .6f }, Projectile.Center);
			SoundEngine.PlaySound(SoundID.Drown with { Pitch = -.5f, PitchVariance = .25f, Volume = 1.5f }, Projectile.Center);

			for (int i = 0; i < 5; i++) //Large initial smoke plume
			{
				ParticleHandler.SpawnParticle(new DissipatingSmoke(Projectile.Center + Main.rand.NextVector2Unit() * 25f, -Vector2.UnitY,
					new Color(40, 40, 50), Color.Black, Main.rand.NextFloat(.05f, .3f), 150));
			}

			Main.LocalPlayer.SimpleShakeScreen(2, 3, 90, 16 * 10);
		}

		if (Projectile.timeLeft % 7 == 0 && Main.rand.NextBool(4))
		{
			SoundEngine.PlaySound(SoundID.Drown with { Pitch = -.5f, PitchVariance = .25f, Volume = 1.5f }, Projectile.Center);

			int item = Item.NewItem(Projectile.GetSource_FromAI(), Projectile.Center, 0, 0, ModContent.ItemType<SulfurDeposit>(), 1, false, 0, false);
			Main.item[item].velocity = (Projectile.velocity * Main.rand.NextFloat()).RotatedByRandom(1f);
			Main.item[item].noGrabDelay = 100;

			if (Main.netMode != NetmodeID.SinglePlayer)
				NetMessage.SendData(MessageID.SyncItem, -1, -1, null, item, 1f);
		}

		if (Main.rand.NextBool()) //Foreground embers
			FireParticleScreen.Spawn();

		if (Main.rand.NextBool(4)) //Small embers
			ParticleHandler.SpawnParticle(new Particles.GlowParticle(Projectile.Center + new Vector2(Main.rand.NextFloat(-1f, 1f) * 4, 16),
				(Projectile.velocity * Main.rand.NextFloat(.25f)).RotatedByRandom(.4f), Color.OrangeRed, Main.rand.NextFloat(.1f, .4f), 190, 8, delegate (Particle p)
				{
					p.Velocity = p.Velocity.RotatedByRandom(.05f);
				}));

		for (int i = 0; i < 3; i++)
		{
			var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, .1f + Main.rand.Next(5) * .1f);
			dust.fadeIn = 1.5f + Main.rand.Next(5) * 0.1f;
			dust.noGravity = true;
			dust.position = Projectile.Center + new Vector2(0f, -Projectile.height / 2f).RotatedBy(Projectile.rotation, default) * 1.1f;
		}

		float speedY = -2.5f;
		var dust2 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, Utils.SelectRandom(Main.rand, 6, 259, 31), 0f, speedY, 200, default, Main.rand.NextFloat() + .5f);

		dust2.velocity *= new Vector2(0.3f, 2f);
		dust2.velocity.Y -= 2;
		dust2.position = new Vector2(Projectile.Center.X, Projectile.Center.Y + Projectile.height * -0.5f);
	}

	public override bool ShouldUpdatePosition() => false;

	public override bool PreDraw(ref Color lightColor) => false;
}