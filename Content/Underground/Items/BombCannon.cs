using SpiritReforged.Common.PlayerCommon;
using SpiritReforged.Common.ProjectileCommon;
using SpiritReforged.Common.Visuals.Glowmasks;
using SpiritReforged.Content.Underground.Items.BigBombs;
using System.IO;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace SpiritReforged.Content.Underground.Items;

[AutoloadGlowmask("255,255,255")]
public class BombCannon : ModItem
{
	public override void SetDefaults()
	{
		Item.width = 44;
		Item.height = 48;
		Item.useTime = Item.useAnimation = 30;
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.noMelee = true;
		Item.noUseGraphic = true;
		Item.value = Item.sellPrice(0, 1, 0, 0);
		Item.rare = ItemRarityID.Blue;
		Item.autoReuse = true;
		Item.shoot = ProjectileID.Bomb;
		Item.channel = true;
		Item.shootSpeed = 8f;
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		if (player.HasAccessory<BoomShroom>())
			type = BoomShroomPlayer.MakeLarge(type);

		Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<BombCannonHeld>(), 0, 0, player.whoAmI, type);
		return false;
	}
}

[AutoloadGlowmask("255,255,255", false)]
internal class BombCannonHeld : ModProjectile
{
	private const int ChargeTimeMax = 60 * 3;

	public int ShootType
	{
		get => (int)Projectile.ai[0];
		set => Projectile.ai[0] = value;
	}

	public ref float Charge => ref Projectile.ai[1];
	public float Progress => (float)Charge / ChargeTimeMax;

	public override string Texture => ModContent.GetInstance<BombCannon>().Texture;
	public override LocalizedText DisplayName => Language.GetText("Mods.SpiritReforged.Items.BombCannon.DisplayName");

	private bool _released;

	public override void SetDefaults()
	{
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 2;
	}

	public override void AI()
	{
		var owner = Main.player[Projectile.owner];
		if (!_released)
		{
			if (!owner.channel || Progress == 1)
			{
				if (Progress >= .33f) //Only fire if charged enough
					Fire();

				_released = true;
			}

			if (owner.whoAmI == Main.myPlayer)
			{
				Projectile.velocity = (Vector2.UnitX * Projectile.velocity.Length()).RotatedBy(owner.AngleTo(Main.MouseWorld));
				Projectile.netUpdate = true;
			}

			Charge = Math.Min(Charge + 1, ChargeTimeMax);
			Projectile.timeLeft++;

			owner.itemAnimation = owner.itemTime = 2;
			owner.direction = (Projectile.velocity.X < 0) ? -1 : 1;
		}

		int holdDistance = 20;
		float rotation = Projectile.velocity.ToRotation();

		Projectile.direction = Projectile.spriteDirection = owner.direction;
		var position = owner.MountedCenter + new Vector2(holdDistance, 5 * -Projectile.direction).RotatedBy(rotation);

		Projectile.Center = owner.RotatedRelativePoint(position);
		Projectile.rotation = rotation;

		owner.heldProj = Projectile.whoAmI;
		owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - 1.57f + .4f * owner.direction);
		owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - 1.57f + .4f * owner.direction);
	}

	private void Fire()
	{
		if (Projectile.owner == Main.myPlayer)
		{
			PreNewProjectile.New(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity, ShootType, owner: Projectile.owner, preSpawnAction: delegate (Projectile p)
			{
				p.timeLeft = (int)(p.timeLeft * (1f - Progress));
				p.GetGlobalProjectile<NullBombProjectile>().noExplode = true;
			});
		}

		var unit = Vector2.Normalize(Projectile.velocity);
		var muzzle = Projectile.Center + unit * 30f;

		for (int i = 0; i < 9; i++)
		{
			float mag = Main.rand.NextFloat();
			Dust.NewDustPerfect(muzzle, DustID.AmberBolt, (unit * 10f * mag).RotatedByRandom(.5f * (1f - mag)), Scale: Main.rand.NextFloat(.5f, 1f)).noGravity = true;
		}

		SoundEngine.PlaySound(SoundID.DD2_BallistaTowerShot with { Pitch = .5f }, Projectile.Center);
		_released = true;
	}

	public override bool ShouldUpdatePosition() => false;
	public override bool? CanCutTiles() => false;
	public override bool? CanDamage() => false;

	public override bool PreDraw(ref Color lightColor)
	{
		var texture = TextureAssets.Projectile[Type].Value;
		var glowmask = GlowmaskProjectile.ProjIdToGlowmask[Type].Glowmask.Value;

		var position = new Vector2((int)(Projectile.Center.X - Main.screenPosition.X), (int)(Projectile.Center.Y - Main.screenPosition.Y));
		var effects = (Projectile.spriteDirection == -1) ? SpriteEffects.FlipVertically : SpriteEffects.None;

		int width = 32 + (int)(6 * (Progress / .33f));
		var glowFrame = new Rectangle(0, 0, width, texture.Height);

		Main.EntitySpriteDraw(texture, position, null, Projectile.GetAlpha(lightColor), Projectile.rotation, texture.Size() / 2, Projectile.scale, effects);
		Main.EntitySpriteDraw(glowmask, position, glowFrame, Projectile.GetAlpha(Color.White), Projectile.rotation, texture.Size() / 2, Projectile.scale, effects);
		return false;
	}
}

/// <summary> Prevents <see cref="Projectile.ExplodeTiles"/> from exploding tiles when <see cref="noExplode"/> is <see cref="true"/>. </summary>
internal class NullBombProjectile : GlobalProjectile
{
	public override bool InstancePerEntity => true;

	public bool noExplode;

	public override void Load() => On_Projectile.ExplodeTiles += PreventTileDamage;
	private static void PreventTileDamage(On_Projectile.orig_ExplodeTiles orig, Projectile self, Vector2 compareSpot, int radius, int minI, int maxI, int minJ, int maxJ, bool wallSplode)
	{
		if (self.GetGlobalProjectile<NullBombProjectile>().noExplode)
			return;

		orig(self, compareSpot, radius, minI, maxI, minJ, maxJ, wallSplode);
	}

	public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter) => bitWriter.WriteBit(noExplode);
	public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader) => noExplode = bitReader.ReadBit();
}