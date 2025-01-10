using SpiritReforged.Common.Particle;
using SpiritReforged.Content.Ocean.Items.Reefhunter.Particles;
using Terraria.Audio;

namespace SpiritReforged.Content.Ocean.Items.Reefhunter.CascadeArmor;

public class CascadeArmorPlayer : ModPlayer
{
	private static Asset<Texture2D> ShieldTexture, OutlineTexture;

	public const float MaxResist = .20f;

	internal float bubbleStrength = 0;
	internal int bubbleCooldown = 120;
	internal bool setActive = false;

	//Visual data
	internal Vector2 bubbleSquish = Vector2.One;
	internal Vector2 squishVelocity = Vector2.Zero;
	internal float bubbleVisual = 0;
	internal Vector2 realOldVelocity = Vector2.Zero; //Mandatory, player.oldVelocity just doesn't work????????? ???? ???

	public override void SetStaticDefaults()
	{
		if (!Main.dedServ)
		{
			string texturePath = typeof(CascadeArmorPlayer).Namespace.Replace(".", "/") + "/BubbleShield";

			ShieldTexture = ModContent.Request<Texture2D>(texturePath);
			OutlineTexture = ModContent.Request<Texture2D>(texturePath + "Outline");
		}
	}

	public override void ResetEffects() => setActive = false;

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		if (setActive && bubbleCooldown == 0)
			bubbleStrength = MathHelper.Clamp(bubbleStrength += .125f, 0, 1);
	}

	public override void PreUpdate() => realOldVelocity = Player.velocity;

	public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers) => TryPopBubble(ref modifiers.FinalDamage);
	
	public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers) => TryPopBubble(ref modifiers.FinalDamage);

	public override void ModifyHurt(ref Player.HurtModifiers modifiers)
	{
		if (modifiers.DamageSource.SourceOtherIndex is 2 or 3)
			TryPopBubble(ref modifiers.FinalDamage);
	}

	public override void PostUpdate()
	{
		if (setActive && !Player.dead)
		{
			var drawOn = Player.GetModPlayer<Common.PlayerCommon.ExtraDrawOnPlayer>();

			drawOn.DrawDict.Add(delegate (SpriteBatch sB) { DrawBubble(sB); }, Common.PlayerCommon.ExtraDrawOnPlayer.DrawType.NonPremultiplied);
			//drawOn.DrawDict.Add(delegate (SpriteBatch sB) { DrawBubble(sB, true); }, Common.PlayerCommon.ExtraDrawOnPlayer.DrawType.AlphaBlend);

			HandleBubbleJiggle();

			if (bubbleCooldown > 0)
				bubbleCooldown--;
		}
		else
		{
			if (bubbleStrength > 0) //Kill bubble if armor piece unequipped
				PopBubble();

			bubbleCooldown = 60;
			bubbleSquish = Vector2.One;
			squishVelocity = Vector2.Zero;
		}

		if (bubbleVisual < bubbleStrength) //smooth transition for visual
			bubbleVisual = MathHelper.Lerp(bubbleVisual, bubbleStrength, .1f);

		bubbleVisual = Math.Min(bubbleVisual, bubbleStrength); //Cap visual strength at real strength value
	}

	/// <summary>
	/// Makes the bubble squash and stretch based on the player's momentum, by loosely adjusting the bubble's size over time based on how fast the player is moving
	/// </summary>
	private void HandleBubbleJiggle()
	{
		float squishspeed = 0.08f; //How fast the bubble squishes and stretches
		float interpolationspeed = 0.05f; //How fast the velocity catches up to what it's meant to be
		var desiredSquish = new Vector2(1, 1);
		float moveStrength = 0.15f; //how much the player's speed affects the bubble
		float squishBounds = 0.075f; //the bounds to which the bubble can squash and stretch
		float SUPERjiggleThreshold = 5f;

		//Make it squish in the direction the player is moving
		var absVel = new Vector2(Math.Abs(Player.velocity.X), Math.Abs(Player.velocity.Y));

		float playerMovementFactor = MathHelper.Clamp((1 + absVel.X * moveStrength) / (1 + absVel.Y * moveStrength), 1 - squishBounds, 1 + squishBounds);
		desiredSquish.X *= playerMovementFactor;
		desiredSquish.Y /= playerMovementFactor;

		var squishDirectionUnit = (desiredSquish - bubbleSquish).SafeNormalize(Vector2.One);
		if (Vector2.Distance(desiredSquish, bubbleSquish) > 0.05f) //don't change velocity if it's super minor, to avoid rapid tiny changes in size
			squishVelocity = Vector2.Lerp(squishVelocity, squishDirectionUnit, interpolationspeed);

		//jiggle more if player makes a sudden big jump in velocity (ie dashing, landing, jumping)
		if (Math.Abs(Player.velocity.Length() - realOldVelocity.Length()) > SUPERjiggleThreshold)
			squishVelocity += squishDirectionUnit / 2;

		bubbleSquish += squishVelocity * squishspeed;

		//Loosely clamp the squish so it doesnt stretch too far in either direction, without making an awkward looking hard restriction
		var clampedBubbleSquish = new Vector2(MathHelper.Clamp(bubbleSquish.X, 1 - squishBounds, 1 + squishBounds), MathHelper.Clamp(bubbleSquish.Y, 1 - squishBounds, 1 + squishBounds));
		bubbleSquish = Vector2.Lerp(bubbleSquish, clampedBubbleSquish, interpolationspeed);
	}

	private void TryPopBubble(ref StatModifier damage)
	{
		if (bubbleStrength > 0f)
		{
			ModContent.GetInstance<CascadeCombatText>().resistanceValue = MaxResist * bubbleStrength;

			damage *= 1 - MaxResist * bubbleStrength;
			PopBubble();
		}
	}

	private void PopBubble()
	{
		int radius = (int)(120 * bubbleStrength);

		foreach (var npc in Main.ActiveNPCs)
		{
			if (npc.CanBeChasedBy() && npc.DistanceSQ(Player.Center) < radius * radius)
				npc.SimpleStrikeNPC(1, Player.Center.X < npc.Center.X ? 1 : -1, false, 3f * bubbleStrength);
		}

		if (!Main.dedServ)
		{
			ParticleHandler.SpawnParticle(new BubblePop(Player.Center, GetBaseBubbleScale, 0.8f * bubbleVisual, 35));
			SoundEngine.PlaySound(SoundID.Item54 with { PitchVariance = 0.2f }, Player.Center);
			SoundEngine.PlaySound(SoundID.NPCHit3 with { PitchVariance = 0.2f }, Player.Center);
			SoundEngine.PlaySound(SoundID.Item86, Player.Center);
		}

		bubbleStrength = 0f;
		bubbleVisual = 0f;
		bubbleCooldown = 120;
	}

	private void DrawBubble(SpriteBatch sB, bool outline = false)
	{
		if (bubbleVisual > 0f)
		{
			Texture2D texture = (outline ? OutlineTexture : ShieldTexture).Value;

			Vector2 drawPos = Player.Center - Main.screenPosition + new Vector2(0, Player.gfxOffY);

			float opacity = outline ? .25f : 0.8f;
			Color lightColor = Lighting.GetColor((int)(Player.Center.X / 16), (int)(Player.Center.Y / 16));

			sB.Draw(texture, drawPos, null, lightColor * bubbleVisual * opacity, 0f, texture.Size() / 2f, bubbleSquish * GetBaseBubbleScale, SpriteEffects.None, 0);
		}
	}

	private float GetBaseBubbleScale => Common.Easing.EaseFunction.EaseCubicOut.Ease(bubbleVisual) * (1 + (float) Math.Sin(Main.time * MathHelper.TwoPi / 120) / 30);
}