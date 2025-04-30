using SpiritReforged.Common.Misc;
using SpiritReforged.Common.Visuals;
using System.IO;
using Terraria.Audio;
using static Microsoft.Xna.Framework.MathHelper;
using static SpiritReforged.Common.Easing.EaseFunction;

namespace SpiritReforged.Common.ProjectileCommon.Abstract;

public abstract partial class BaseClubProj(Vector2 textureSize) : ModProjectile
{
	private const int MAX_FLICKERTIME = 20;

	internal readonly Vector2 Size = textureSize;

	public float DamageScaling { get; private set; }
	public float KnockbackScaling { get; private set; }

	public int ChargeTime { get; private set; }
	public int SwingTime { get; private set; }

	internal int WindupTime => (int)(ChargeTime * WindupTimeRatio);
	internal int LingerTime => (int)(SwingTime * LingerTimeRatio);

	public float Charge { get => Projectile.ai[0]; set => Projectile.ai[0] = value; }
	public float AiState { get => Projectile.ai[1]; set => Projectile.ai[1] = value; }
	public float BaseRotation { get => Projectile.ai[2]; set => Projectile.ai[2] = value; }

	protected int _lingerTimer;
	protected int _swingTimer;
	protected int _windupTimer;
	protected int _flickerTime;

	private bool _hasFlickered = false;

	/// <summary><inheritdoc cref="ModProjectile.DisplayName"/><para/>
	/// Automatically attempts to use the associated item localization. </summary>
	public override LocalizedText DisplayName => Language.GetText("Mods.SpiritReforged.Items." + Name.Replace("Proj", string.Empty) + ".DisplayName");
	/// <summary><inheritdoc cref="ModProjectile.Texture"/><para/>
	/// Automatically attempts to use the associated item texture. </summary>
	public override string Texture
	{
		get
		{
			string def = base.Texture;
			return def.Remove(def.Length - 4); //Remove 'proj'
		}
	}

	public sealed override void SetStaticDefaults()
	{
		ProjectileID.Sets.TrailCacheLength[Type] = 6;
		ProjectileID.Sets.TrailingMode[Type] = 2;

		SafeSetStaticDefaults();
	}

	public sealed override void SetDefaults()
	{
		Projectile.hostile = false;
		Projectile.DamageType = DamageClass.Melee;
		Projectile.width = Projectile.height = 16;
		Projectile.aiStyle = -1;
		Projectile.friendly = true;
		Projectile.penetrate = -1;
		Projectile.tileCollide = false;
		Projectile.ownerHitCheck = true;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = -1;

		SafeSetDefaults();
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		float dummy = 0;
		float lineWidth = Size.Length() / 2;
		var endPoint = Vector2.Lerp(Main.player[Projectile.owner].Center, Projectile.Center, Projectile.scale);

		return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Main.player[Projectile.owner].Center, endPoint, lineWidth, ref dummy);
	}

	public override bool? CanDamage() => CheckAIState(AIStates.SWINGING) ? null : false;

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		if (FullCharge)
		{
			modifiers.FinalDamage *= DamageScaling;
			modifiers.Knockback *= KnockbackScaling;
		}
	}

	public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
	{
		if (FullCharge)
		{
			modifiers.FinalDamage *= DamageScaling;
			modifiers.Knockback *= KnockbackScaling;
		}
	}

	public sealed override void AI()
	{
		SafeAI();

		Player owner = Main.player[Projectile.owner];

		if (owner.dead)
			Projectile.Kill();

		owner.heldProj = Projectile.whoAmI;

		if (AllowUseTurn)
		{
			if (owner == Main.LocalPlayer)
			{
				int newDir = Math.Sign(Main.MouseWorld.X - owner.Center.X);
				Projectile.velocity.X = newDir == 0 ? owner.direction : newDir;

				if (newDir != owner.direction)
					Projectile.netUpdate = true;
			}

			owner.ChangeDir((int)Projectile.velocity.X);
		}

		else
			owner.direction = Math.Sign(Projectile.velocity.X);

		switch (AiState)
		{
			case (float)AIStates.CHARGING: 
				Charging(owner);
				break;

			case (float)AIStates.SWINGING:
				Swinging(owner);
				break;

			case (float)AIStates.POST_SMASH:
				AfterCollision();
				break;
		}

		if (!owner.controlUseItem && _windupTimer >= WindupTime && CheckAIState(AIStates.CHARGING) && AllowRelease)
		{
			SetAIState(AIStates.SWINGING);
			OnSwingStart();

			if (!Main.dedServ)
				SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing.WithPitchOffset(-0.75f), owner.Center);
		}

		TranslateRotation(owner, out float clubRotation, out float armRotation);
		Projectile.rotation = clubRotation;

		owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.ThreeQuarters, armRotation);
		owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.ThreeQuarters, armRotation);
		Projectile.position.X = owner.Center.X - (int)(Math.Cos(armRotation - PiOver2) * Size.X) - Projectile.width / 2;
		Projectile.position.Y = owner.Center.Y - (int)(Math.Sin(armRotation - PiOver2) * Size.Y) - Projectile.height / 2 - owner.gfxOffY;

		owner.itemAnimation = owner.itemTime = 2;
	}

	public sealed override bool PreDraw(ref Color lightColor)
	{
		Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
		Vector2 handPos = Owner.GetFrontHandPosition(Owner.compositeFrontArm.stretch, Owner.compositeFrontArm.rotation);
		Vector2 drawPos = handPos - Main.screenPosition + Vector2.UnitY * Owner.gfxOffY;
		Color drawColor = Projectile.GetAlpha(lightColor);

		Rectangle topFrame = texture.Frame(1, Main.projFrames[Type]);

		//Aftertrail during swing
		if (CheckAIState(AIStates.SWINGING))
			DrawAftertrail(lightColor);

		Main.EntitySpriteDraw(texture, drawPos, topFrame, drawColor, Projectile.rotation, HoldPoint, Projectile.scale, Effects, 0);

		SafeDraw(Main.spriteBatch, lightColor);

		//Flash when fully charged
		if (CheckAIState(AIStates.CHARGING) && _flickerTime > 0)
		{
			Texture2D flash = TextureColorCache.ColorSolid(texture, Color.White);
			float alpha = EaseQuadIn.Ease(EaseSine.Ease(_flickerTime / (float)MAX_FLICKERTIME));

			Main.EntitySpriteDraw(flash, drawPos, topFrame, Color.White * alpha, Projectile.rotation, HoldPoint, Projectile.scale, Effects, 0);
		}

		return false;
	}

	//Multiplayer syncing
	public override void SendExtraAI(BinaryWriter writer)
	{
		writer.Write((Half)DamageScaling);
		writer.Write((Half)KnockbackScaling);

		writer.Write((ushort)ChargeTime);
		writer.Write((ushort)SwingTime);

		writer.Write((ushort)_lingerTimer);
		writer.Write((ushort)_swingTimer);
		writer.Write((ushort)_windupTimer);

		writer.Write((ushort)_flickerTime);
		writer.Write(_hasFlickered);

		SendExtraDataSafe(writer);
	}

	public override void ReceiveExtraAI(BinaryReader reader)
	{
		DamageScaling = (float)reader.ReadHalf();
		KnockbackScaling = (float)reader.ReadHalf();

		ChargeTime = reader.ReadUInt16();
		SwingTime = reader.ReadUInt16();

		_lingerTimer = reader.ReadUInt16();
		_swingTimer = reader.ReadUInt16();
		_windupTimer = reader.ReadUInt16();

		_flickerTime = reader.ReadUInt16();
		_hasFlickered = reader.ReadBoolean();

		ReceiveExtraDataSafe(reader);
	}
}