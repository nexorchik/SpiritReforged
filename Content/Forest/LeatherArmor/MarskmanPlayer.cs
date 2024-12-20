using SpiritReforged.Common.Particle;
using Terraria;
using Terraria.Audio;

namespace SpiritReforged.Content.Forest.LeatherArmor;

internal class MarksmanPlayer : ModPlayer
{
	public bool active = false;
	public bool concentrated;
	public int concentratedCooldown = 360;
	public override void ResetEffects() => active = false;

	public override void PostUpdateEquips()
	{
		if (active)
		{
			if (concentratedCooldown == 0)
				SoundEngine.PlaySound(SoundID.MaxMana);

			concentratedCooldown -= (Player.velocity.X == 0f) ? 2 : 1;
		}
		else
		{
			concentrated = false;
			concentratedCooldown = 420;
		}

		if (concentratedCooldown <= 0)
			concentrated = true;
	}
	public override void OnHurt(Player.HurtInfo info)
	{
		if (active)
		{
			concentratedCooldown = 360;
			concentrated = false;
		}
	}
	public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers)/* tModPorter If you don't need the Item, consider using ModifyHitNPC instead */
	{
		if (concentrated)
		{
			for (int i = 0; i < 40; i++)
			{
				int dust = Dust.NewDust(target.Center, target.width, target.height, DustID.GoldCoin);
				Main.dust[dust].velocity *= -1f;
				Main.dust[dust].noGravity = true;

				Vector2 vector2_1 = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
				vector2_1.Normalize();

				Vector2 vector2_2 = vector2_1 * (Main.rand.Next(50, 100) * 0.04f);
				Main.dust[dust].velocity = vector2_2;
				vector2_2.Normalize();

				Vector2 vector2_3 = vector2_2 * 34f;
				Main.dust[dust].position = target.Center - vector2_3;
			}

			modifiers.FinalDamage *= 1.2f;
			modifiers.SetCrit();
			concentrated = false;
			concentratedCooldown = 300;
		}
	}
	public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
	{ 
		if (concentrated)
		{
			for (int i = 0; i < 40; i++)
			{
				int dust = Dust.NewDust(target.Center, target.width, target.height, DustID.GoldCoin);
				Main.dust[dust].velocity *= -1f;
				Main.dust[dust].noGravity = true;

				Vector2 velocity = Vector2.Normalize(new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101))) * (Main.rand.Next(50, 100) * 0.04f);
				Main.dust[dust].velocity = velocity;
				velocity.Normalize();

				Vector2 vector2_3 = velocity * 34f;
				Main.dust[dust].position = target.Center - vector2_3;
			}

			modifiers.FinalDamage *= 1.2f;
			modifiers.SetCrit();
			concentrated = false;
			concentratedCooldown = 300;
		}
	}
}
