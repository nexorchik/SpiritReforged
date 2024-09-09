using SpiritReforged.Common.Easing;
using System.IO;
using Terraria.Audio;
using static Terraria.Player;

namespace SpiritReforged.Content.Ocean.Items.Reefhunter.Projectiles;

public class ReefSpearProjectile : ModProjectile
{
	private const int MAX_DISTANCE = 70;
	private const int NUM_STABS = 3;
	private const float WINDUP_TIME = 0.3f; //Percentage of the full lifetime, 0.3f is 30%

	public Vector2 RealDirection => (_direction * MAX_DISTANCE).RotatedBy(_rotationOffset * _rotationDirection);

	private Vector2 _direction = Vector2.Zero;
	private int _maxTimeleft = 0;
	private int _realMaxTimeleft = 0;
	private float _rotationOffset = 0;
	private float _rotationDirection = 0;
	private bool _canHitEnemy = false;

	public override LocalizedText DisplayName => Language.GetText("Mods.SpiritReforged.Items.ReefSpear.DisplayName");

	public override void SetDefaults()
	{
		Projectile.width = 30;
		Projectile.height = 30;
		Projectile.friendly = true;
		Projectile.penetrate = -1;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.DamageType = DamageClass.Melee;
		Projectile.aiStyle = -1;
		Projectile.timeLeft = 70;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = -1;

		DrawHeldProjInFrontOfHeldItemAndArms = false;
	}

	public override bool? CanDamage() => true;

	public override void AI()
	{
		Player p = Main.player[Projectile.owner];
		p.heldProj = Projectile.whoAmI;
		p.itemTime = 2;
		p.itemAnimation = 2;
		p.reuseDelay = 15;
		CompositeArmStretchAmount stretchAmount = CompositeArmStretchAmount.Full;
		float factor = 1; //Lerp factor for pushing out and coming back in
		float length = 1;
		_canHitEnemy = false;

		if (_maxTimeleft == 0) //Initialize
		{
			if(p.whoAmI == Main.myPlayer)
				_direction = Vector2.Normalize(p.Center - Main.MouseWorld);

			float modifiedAttackSpeed = 0.25f + p.GetWeaponAttackSpeed(p.HeldItem) * 0.75f;
			Projectile.timeLeft = (int)(Projectile.timeLeft / modifiedAttackSpeed);
			_realMaxTimeleft = Projectile.timeLeft;
			_maxTimeleft = (int)(_realMaxTimeleft * (1 - WINDUP_TIME));
			_rotationOffset = MathHelper.Pi * 0.13f * (Main.rand.NextBool() ? -1 : 1);
			Projectile.netUpdate = true;
		}

		if(Projectile.timeLeft > _maxTimeleft)
			Windup(p, ref factor, ref stretchAmount);

		else
			StabCombo(ref length, ref factor, ref stretchAmount);

		p.SetCompositeArmFront(true, stretchAmount, _direction.ToRotation() + 1.57f + _rotationOffset * _rotationDirection);
		var clampedStretch = (CompositeArmStretchAmount)MathHelper.Clamp((byte)stretchAmount, 1, 2);
		p.SetCompositeArmBack(true, clampedStretch, _direction.ToRotation() + 1.57f + _rotationOffset * _rotationDirection);

		var offset = new Vector2(0, -2 * p.direction);

		Projectile.Center = p.Center + new Vector2(0, p.gfxOffY) - Vector2.Lerp(Vector2.Zero, RealDirection, factor * length) + RealDirection * 0.5f + offset;
	}

	private void Windup(Player p, ref float factor, ref CompositeArmStretchAmount stretchAmount)
	{
		float progress = (float)(Projectile.timeLeft - _maxTimeleft) / (_realMaxTimeleft - _maxTimeleft);
		progress = EaseFunction.EaseQuadOut.Ease(1 - progress);
		factor = MathHelper.Lerp(0.25f, 0f, progress);
		if (p.whoAmI == Main.myPlayer)
		{
			_direction = Vector2.Normalize(p.Center - Main.MouseWorld);
			p.direction = Main.MouseWorld.X >= p.MountedCenter.X ? 1 : -1;
		}

		if (progress > 0.5f)
			stretchAmount = CompositeArmStretchAmount.Quarter;

		Projectile.netUpdate = true;
	}

	private void StabCombo(ref float length, ref float factor, ref CompositeArmStretchAmount stretchAmount)
	{
		_canHitEnemy = true;
		float progress = Projectile.timeLeft / (float)_maxTimeleft;
		float progressExponent = 2f;
		List<int> stabTimes = [];
		for (int i = NUM_STABS; i > 0; i--)
			stabTimes.Add((int)(Math.Pow(i / (float)NUM_STABS, 1 / progressExponent) * _maxTimeleft));

		progress = (float)(Math.Pow(progress, progressExponent));

		if (stabTimes.Contains(Projectile.timeLeft)) //During the beginning frame of a stab
		{
			if (_rotationDirection == 0)
				_rotationDirection = 1;

			_rotationDirection *= Main.rand.NextFloat(-1.2f, -0.8f);
			if (Projectile.timeLeft == stabTimes[NUM_STABS - 1])
				_rotationDirection = 0;

			Projectile.ResetLocalNPCHitImmunity();
			Projectile.netUpdate = true;

			//do vfx and sfx here

			if (!Main.dedServ)
			{
				SoundEngine.PlaySound(SoundID.DD2_JavelinThrowersAttack with { PitchVariance = 0.3f, Volume = 0.75f, MaxInstances = -1 }, Projectile.Center);
			}
		}

		length = Projectile.timeLeft < stabTimes[2] ? 1f : 0.65f;

		factor = 1 - progress % (1 / (float)NUM_STABS) * NUM_STABS;
		factor = (float)Math.Pow(factor, 0.75f);
		factor = (float)Math.Sin(factor * MathHelper.Pi);
		factor = EaseFunction.EaseQuadIn.Ease(factor);

		//Stretch arm based on distance from player
		if (factor < 0.66f)
			stretchAmount = CompositeArmStretchAmount.Quarter;
		else if (factor < 0.33f)
			stretchAmount = CompositeArmStretchAmount.ThreeQuarters;
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) => modifiers.HitDirectionOverride = Math.Sign(-_direction.X);

	public override void ModifyDamageHitbox(ref Rectangle hitbox)
	{
		Vector2 pos = Projectile.Center - RealDirection - new Vector2(16);

		hitbox.X = (int)pos.X;
		hitbox.Y = (int)pos.Y;
	}

	public override bool? CanHitNPC(NPC target) => _canHitEnemy;

	public override bool CanHitPlayer(Player target) => _canHitEnemy;

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D t = TextureAssets.Projectile[Projectile.type].Value;
		Main.spriteBatch.Draw(t, Projectile.Center - Main.screenPosition, null, lightColor, RealDirection.ToRotation() - MathHelper.Pi, new Vector2(40, 14), 1f, SpriteEffects.None, 1f);
		return false;
	}

	public override void SendExtraAI(BinaryWriter writer)
	{
		writer.WriteVector2(_direction);
		writer.Write(_maxTimeleft);
		writer.Write(_realMaxTimeleft);
		writer.Write(_rotationOffset);
		writer.Write(_rotationDirection);
	}

	public override void ReceiveExtraAI(BinaryReader reader)
	{
		_direction = reader.ReadVector2();
		_maxTimeleft = reader.Read();
		_realMaxTimeleft = reader.Read();
		_rotationOffset = reader.Read();
		_rotationDirection = reader.Read();
	}
}
