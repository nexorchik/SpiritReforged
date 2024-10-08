using SpiritReforged.Common.Easing;
using SpiritReforged.Common.Visuals.Glowmasks;
using System.IO;
using static Terraria.Player;

namespace SpiritReforged.Content.Desert.Scarabeus.Items.Projectiles;

[AutoloadGlowmask("255,255,255", false)]
public class SunStaffHeld : ModProjectile
{
	private const int RISE_TIME = 30;
	
	private float AiTimer { get => Projectile.ai[0]; set => Projectile.ai[0] = value; }
	private float RiseProgress { get => Projectile.ai[1]; set => Projectile.ai[1] = value; }

	private bool _stoppedChannel = false;

	public override void SetDefaults()
	{
		Projectile.width = 26;
		Projectile.height = 26;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.friendly = true;
		Projectile.aiStyle = -1;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		DrawHeldProjInFrontOfHeldItemAndArms = false;
	}

	public override bool? CanDamage() => false;

	public override void AI()
	{
		if(!Projectile.TryGetOwner(out Player owner))
		{
			Projectile.Kill();
			return;
		}

		owner.heldProj = Projectile.whoAmI;
		owner.itemAnimation = 2;
		owner.itemTime = 2;
		Projectile.timeLeft = 2;
		if (owner.whoAmI == Main.myPlayer)
		{
			int tempDirection = owner.direction;
			owner.direction = Main.MouseWorld.X >= owner.MountedCenter.X ? 1 : -1;
			if(tempDirection != owner.direction && Main.netMode != NetmodeID.SinglePlayer)
				NetMessage.SendData(MessageID.SyncPlayer, -1, -1, null, owner.whoAmI);
		}

		if(!owner.channel && !_stoppedChannel && AiTimer > RISE_TIME)
		{
			_stoppedChannel = true;
			AiTimer = 0;
			Projectile.netUpdate = true;
		}

		StaffMovement();

		if (AiTimer == RISE_TIME/4 && owner.channel)
			Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), owner.Center - Vector2.UnitY * 140, Vector2.Zero, ModContent.ProjectileType<SunOrb>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

		Projectile.rotation = MathHelper.Lerp(-MathHelper.PiOver4, -3 * MathHelper.PiOver4, RiseProgress);
		if (owner.direction < 0)
			Projectile.rotation = MathHelper.Lerp(-MathHelper.PiOver4, -3 * MathHelper.PiOver4, 1 - RiseProgress) + MathHelper.Pi;

		owner.SetCompositeArmFront(true, CompositeArmStretchAmount.Full, Projectile.rotation);
		Projectile.Center = owner.GetFrontHandPosition(CompositeArmStretchAmount.Full, Projectile.rotation);

		AiTimer++;
	}

	private void StaffMovement()
	{
		float progress;
		if (!_stoppedChannel)
		{
			progress = AiTimer / RISE_TIME;
			progress = Math.Min(progress, 1);
		}
		else
		{
			progress = 1 - AiTimer / RISE_TIME;
			progress = Math.Max(progress, 0);
			if (progress == 0)
				Projectile.Kill();
		}

		RiseProgress = EaseFunction.EaseOutBack.Ease(progress);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		float strength = 0.5f;
		Color glowColor = new Color(250, 167, 32, 0) * strength;

		Projectile.TryGetOwner(out Player owner);
		Texture2D staffTex = TextureAssets.Projectile[Type].Value;
		GlowmaskProjectile.ProjIdToGlowmask.TryGetValue(Type, out GlowmaskInfo staffGlowAsset);
		Texture2D staffGlow = staffGlowAsset.Glowmask.Value;

		float diagonalOrigin = 0.2f;
		Vector2 origin = staffTex.Size() * new Vector2(diagonalOrigin, 1 - diagonalOrigin);
		SpriteEffects flip = SpriteEffects.None;
		float rotationFlip = MathHelper.Lerp(Projectile.rotation, 0, 0.9f) - MathHelper.PiOver4;
		if (owner.direction < 0)
		{
			rotationFlip += MathHelper.PiOver2;
			flip = SpriteEffects.FlipHorizontally;
			origin = staffTex.Size() * new Vector2(1 - diagonalOrigin);
		}

		Vector2 staffPos = owner.GetFrontHandPosition(CompositeArmStretchAmount.Full, Projectile.rotation) - Main.screenPosition + Vector2.UnitY * owner.gfxOffY;
		float stafScale = MathHelper.Lerp(RiseProgress, 1, 0.33f);
		Main.spriteBatch.Draw(staffTex, staffPos, null, lightColor, rotationFlip, origin, stafScale, flip, 1f);

		Main.spriteBatch.Draw(staffGlow, staffPos, null, glowColor, rotationFlip, origin, stafScale, flip, 1f);

		float numGlow = 6;
		for (int i = 0; i < numGlow; i++)
		{
			Vector2 offset = Vector2.UnitX.RotatedBy(MathHelper.TwoPi * i / numGlow) * 3;
			float opacity = 2 / numGlow;
			Main.spriteBatch.Draw(staffGlow, staffPos + offset, null, glowColor * opacity, rotationFlip, origin, stafScale, flip, 1f);
		}

		return false;
	}

	public override void SendExtraAI(BinaryWriter writer) => writer.Write(_stoppedChannel);

	public override void ReceiveExtraAI(BinaryReader reader) => _stoppedChannel = reader.ReadBoolean();
}