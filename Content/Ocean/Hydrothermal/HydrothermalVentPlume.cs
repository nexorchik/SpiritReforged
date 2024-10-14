using SpiritReforged.Common.Particle;
using SpiritReforged.Content.Ocean.Items.Reefhunter;
using SpiritReforged.Content.Ocean.Tiles.Hydrothermal;
using SpiritReforged.Content.Particles;
using Terraria.Audio;

namespace SpiritReforged.Content.Ocean.Hydrothermal;

public class HydrothermalVentPlume : ModProjectile
{
	public override string Texture => "Terraria/Images/NPC_0";

	public override void SetDefaults()
	{
		Projectile.ignoreWater = true;
		Projectile.penetrate = -1;
		Projectile.timeLeft = Tiles.HydrothermalVent.eruptDuration;
	}

	public override void AI()
	{
		if (Projectile.timeLeft % 20 == 0)
		{
			SoundEngine.PlaySound(SoundID.Drown with { Pitch = -.5f, PitchVariance = .25f, Volume = 1.5f }, Projectile.Center);

			if (Main.netMode != NetmodeID.MultiplayerClient && Main.rand.NextBool(4))
			{
				var vel = (Projectile.velocity * 2 * Main.rand.NextFloat(.5f, 1f)).RotatedByRandom(1.75f);
				var proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), 
					Projectile.Center, vel, ModContent.ProjectileType<MineralPickup>(), 0, 0);

				MineralPickup.SpawnItemPickup(ModContent.ItemType<SulfurDeposit>(), proj);
			}
		}

		if (Main.dedServ || !new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight).Contains(Projectile.Center.ToPoint()))
			return; //Don't create vfx for the local player if the projectile isn't on screen

		if (Main.rand.NextBool(10)) //Foreground embers
			FireParticleScreen.Spawn();

		if (Main.rand.NextBool(12)) //Small embers
			ParticleHandler.SpawnParticle(new GlowParticle(Projectile.Center + new Vector2(Main.rand.NextFloat(-1f, 1f) * 4, 0),
				(Projectile.velocity * Main.rand.NextFloat(.25f)).RotatedByRandom(.4f), Color.OrangeRed, Main.rand.NextFloat(.1f, .4f), 190, 8, delegate (Particle p)
				{
					p.Velocity = p.Velocity.RotatedByRandom(.05f);
				}));

		for (int i = 0; i < 2; i++)
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
		dust2.noGravity = true;
		dust2.fadeIn = 1.5f;
	}

	public override bool ShouldUpdatePosition() => false;

	public override bool PreDraw(ref Color lightColor) => false;
}