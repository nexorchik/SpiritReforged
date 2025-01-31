using SpiritReforged.Common.Misc;
using SpiritReforged.Common.ProjectileCommon;
using SpiritReforged.Common.Visuals.Glowmasks;
using SpiritReforged.Common.WorldGeneration;
using Terraria.Audio;
using Terraria.DataStructures;

namespace SpiritReforged.Content.Underground.Zipline;

public class ZiplineGun : ModItem
{
	public const int UseTime = 60;
	private static Asset<Texture2D> xTexture;

	private static bool CheckTile()
	{
		var coords = Main.MouseWorld.ToTileCoordinates();
		var flags = OpenTools.GetOpenings(coords.X, coords.Y, false, onlySolid: true);

		return !WorldGen.SolidOrSlopedTile(Framing.GetTileSafely(coords)) && !flags.HasFlag(OpenFlags.Above | OpenFlags.Right | OpenFlags.Below | OpenFlags.Left);
	}

	private static bool CheckRemoveable()
	{
		foreach (var zipline in ZiplineHandler.ziplines)
		{
			if (zipline.Owner == Main.LocalPlayer && zipline.Contains(Main.MouseWorld.ToPoint(), out _))
				return true;
		}

		return false;
	}

	public override void Load()
	{
		xTexture = ModContent.Request<Texture2D>(Texture + "_Cancel");
		On_Main.DrawInterface_6_TileGridOption += On_Main_DrawInterface_6_TileGridOption;
	}

	private void On_Main_DrawInterface_6_TileGridOption(On_Main.orig_DrawInterface_6_TileGridOption orig)
	{
		bool oldMouseShowGrid = Main.MouseShowBuildingGrid;

		if (Main.LocalPlayer.HeldItem.type == ModContent.ItemType<ZiplineGun>())
		{
			var grid = TextureAssets.CursorRadial.Value;
			var position = Main.MouseWorld.ToTileCoordinates().ToWorldCoordinates() - Main.screenPosition;
			float rotation = (float)Math.Sin(Main.timeForVisualEffects / 50f) * .1f;

			if (Main.LocalPlayer.gravDir == -1f)
				position.Y = Main.screenHeight - position.Y;

			if (CheckRemoveable())
			{
				var outline = TextureAssets.Extra[2].Value;
				var source = new Rectangle(0, 0, 16, 16);

				Main.spriteBatch.Draw(grid, position - grid.Size() / 2, (Color.Green * .5f).Additive());
				Main.spriteBatch.Draw(outline, position, source, Color.Green.Additive(), rotation, source.Size() / 2, 1 + rotation, default, 0);
			}
			else if (!CheckTile())
			{
				var x = xTexture.Value;

				Main.spriteBatch.Draw(grid, position - grid.Size() / 2, (Color.Red * .5f).Additive());
				Main.spriteBatch.Draw(x, position, null, Color.Red.Additive(), rotation, x.Size() / 2, 1 + rotation, default, 0);
			}
			else
			{
				var outline = TextureAssets.Extra[2].Value;
				var source = new Rectangle(0, 0, 16, 16);

				Main.spriteBatch.Draw(grid, position - grid.Size() / 2, (Color.Cyan * .5f).Additive());
				Main.spriteBatch.Draw(outline, position, source, Color.Cyan.Additive(), rotation, source.Size() / 2, 1 + rotation, default, 0);
			}

			Main.MouseShowBuildingGrid = false; //Always temporarily disable the default building grid before it is drawn
		}

		orig();

		Main.MouseShowBuildingGrid = oldMouseShowGrid;
	}

	public override void SetDefaults()
	{
		Item.width = 44;
		Item.height = 48;
		Item.useTime = Item.useAnimation = UseTime;
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.noMelee = true;
		Item.noUseGraphic = true;
		Item.value = Item.sellPrice(0, 1, 0, 0);
		Item.rare = ItemRarityID.Green;
		Item.autoReuse = true;
		Item.shoot = ModContent.ProjectileType<ZiplineProj>();
		Item.shootSpeed = 8f;
	}

	public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

	public override bool AltFunctionUse(Player player) => true;

	public override bool CanUseItem(Player player) => CheckTile() || CheckRemoveable();

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		Projectile.NewProjectile(source, position, Vector2.Normalize(velocity), ModContent.ProjectileType<ZiplineGunHeld>(), 0, 0, player.whoAmI);
		SoundEngine.PlaySound(SoundID.DD2_BallistaTowerShot with { Pitch = .5f }, position);

		var muzzle = position + Vector2.Normalize(velocity) * 30f;
		for (int i = 0; i < 5; i++)
		{
			float mag = Main.rand.NextFloat();
			Dust.NewDustPerfect(muzzle, DustID.AmberBolt, (velocity * mag).RotatedByRandom(.5f * (1f - mag)), Scale: Main.rand.NextFloat(.5f, 1f)).noGravity = true;
		}

		PreNewProjectile.New(source, position, velocity, type, preSpawnAction: delegate (Projectile p)
		{
			if (p.ModProjectile is ZiplineProj zipline)
				zipline.cursorPoint = Main.MouseWorld.ToTileCoordinates().ToWorldCoordinates();
		});

		return false;
	}
}

[AutoloadGlowmask("255,255,255", false)]
internal class ZiplineGunHeld : ModProjectile
{
	private static int TimeLeftMax => ZiplineGun.UseTime;

	public override LocalizedText DisplayName => Language.GetText("Mods.SpiritReforged.Items.ZiplineGun.DisplayName");

	public override void SetStaticDefaults() => Main.projFrames[Type] = 11;

	public override void SetDefaults()
	{
		Projectile.timeLeft = TimeLeftMax;
		Projectile.ignoreWater = true;
	}

	public override void AI()
	{
		const int fireTime = 10;
		const int idleTime = 20;

		int holdDistance = 20;
		float rotation = Projectile.velocity.ToRotation();

		var owner = Main.player[Projectile.owner];
		Projectile.direction = Projectile.spriteDirection = owner.direction;

		if (Projectile.timeLeft < idleTime)
		{
			Projectile.UpdateFrame(30, Main.projFrames[Type] - 1);
			rotation += .3f * Projectile.direction * Math.Clamp((Projectile.timeLeft - 16f) / 4, 0, 1);
		}
		else if (Projectile.timeLeft < TimeLeftMax - fireTime)
		{
			Projectile.UpdateFrame(30, 5, 8);

			var dustPos = Projectile.Center - new Vector2(6, 4 * Projectile.direction).RotatedBy(rotation);
			Dust.NewDustPerfect(dustPos, DustID.Torch, -(Projectile.velocity * Main.rand.NextFloat()).RotatedByRandom(1)).noGravity = !Main.rand.NextBool(4);

			rotation -= (Projectile.timeLeft < TimeLeftMax - fireTime - 3 ? .5f : .25f) * Projectile.direction;

			if (Projectile.timeLeft == idleTime)
				SoundEngine.PlaySound(new SoundStyle("SpiritReforged/Assets/SFX/Item/LMG") { Volume = .4f, Pitch = .8f }, Projectile.Center);
			else if (Projectile.timeLeft % 8 == 0)
				SoundEngine.PlaySound(new SoundStyle("SpiritReforged/Assets/SFX/Item/LMG") { Volume = .1f, Pitch = MathHelper.Lerp(1f, -.5f, ((float)Projectile.timeLeft - fireTime) / ((float)TimeLeftMax - idleTime)) }, Projectile.Center);
		}
		else
		{
			const int recoilDuration = 2;

			float recoil = Math.Clamp(1f - (Projectile.timeLeft - (TimeLeftMax - recoilDuration)) / (float)recoilDuration, 0, 1) * 8f;
			holdDistance -= (int)recoil;
		}

		var position = owner.MountedCenter + new Vector2(holdDistance, 5 * -Projectile.direction).RotatedBy(rotation);

		Projectile.Center = owner.RotatedRelativePoint(position);
		Projectile.rotation = rotation;

		owner.heldProj = Projectile.whoAmI;
		owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - 1.57f + .4f * owner.direction);
		owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - 1.57f + .4f * owner.direction);
	}

	public override bool ShouldUpdatePosition() => false;
	public override bool? CanCutTiles() => false;
	public override bool? CanDamage() => false;

	public override bool PreDraw(ref Color lightColor)
	{
		var texture = TextureAssets.Projectile[Type].Value;
		var glowmask = GlowmaskProjectile.ProjIdToGlowmask[Type].Glowmask.Value;

		var position = new Vector2((int)(Projectile.Center.X - Main.screenPosition.X), (int)(Projectile.Center.Y - Main.screenPosition.Y));
		var frame = texture.Frame(1, Main.projFrames[Type], 0, Math.Min(Projectile.frame, Main.projFrames[Type] - 1), sizeOffsetY: -2);
		var effects = (Projectile.spriteDirection == -1) ? SpriteEffects.FlipVertically : SpriteEffects.None;

		Main.EntitySpriteDraw(texture, position, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, frame.Size() / 2, Projectile.scale, effects);
		Main.EntitySpriteDraw(glowmask, position, frame, Projectile.GetAlpha(Color.White), Projectile.rotation, frame.Size() / 2, Projectile.scale, effects);
		return false;
	}
}