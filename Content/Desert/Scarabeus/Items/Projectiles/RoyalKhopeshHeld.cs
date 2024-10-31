using SpiritReforged.Common.Easing;
using SpiritReforged.Common.Particle;
using SpiritReforged.Common.ProjectileCommon;
using SpiritReforged.Common.Visuals.Glowmasks;
using SpiritReforged.Content.Particles;
using System.IO;
using static Terraria.Player;

namespace SpiritReforged.Content.Desert.Scarabeus.Items.Projectiles;

public class RoyalKhopeshHeld : ModProjectile
{
	private const int SWING_TIME = 30;
	private const float SWING_RADIANS = MathHelper.Pi * 1.5f;
	
	private float AiTimer { get => Projectile.ai[0]; set => Projectile.ai[0] = value; }

	public Vector2 BaseDirection { get; set; }

	private bool _stoppedChannel = false;

	public override void SetDefaults()
	{
		Projectile.width = 46;
		Projectile.height = 46;
		Projectile.DamageType = DamageClass.Melee;
		Projectile.friendly = true;
		Projectile.aiStyle = -1;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		Projectile.penetrate = -1;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = -1;
	}

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

		int tempDirection = owner.direction;
		owner.direction = BaseDirection.X >= 0 ? 1 : -1;
		if (tempDirection != owner.direction && Main.netMode != NetmodeID.SinglePlayer)
			NetMessage.SendData(MessageID.SyncPlayer, -1, -1, null, owner.whoAmI);

		Projectile.rotation = MathHelper.Lerp(-SWING_RADIANS * 0.66f, SWING_RADIANS * 0.33f, EaseFunction.EaseCubicOut.Ease(AiTimer / SWING_TIME)) + MathHelper.PiOver4 + BaseDirection.ToRotation();

		owner.SetCompositeArmFront(true, CompositeArmStretchAmount.Full, Projectile.rotation - 3 * MathHelper.PiOver4);
		Projectile.Center = owner.GetFrontHandPosition(CompositeArmStretchAmount.Full, Projectile.rotation - 3 * MathHelper.PiOver4);

		AiTimer++;
		if (AiTimer > SWING_TIME)
			Projectile.Kill();
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D swordTex = TextureAssets.Projectile[Type].Value;
		Vector2 origin = new Vector2(0.2f, 0.8f) * swordTex.Size();
		Projectile.QuickDraw(origin: origin);
		return false;
	}

	public override void SendExtraAI(BinaryWriter writer)
	{
		writer.Write(_stoppedChannel);
		writer.WriteVector2(BaseDirection);
	}

	public override void ReceiveExtraAI(BinaryReader reader)
	{
		_stoppedChannel = reader.ReadBoolean();
		BaseDirection = reader.ReadVector2();
	}
}