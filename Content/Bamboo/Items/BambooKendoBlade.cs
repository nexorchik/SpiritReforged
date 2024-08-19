using System.Linq;
using Terraria.Audio;
using Terraria.DataStructures;

namespace SpiritReforged.Content.Bamboo.Items;

public class BambooKendoBlade : ModItem, IDashSword
{
	public override void SetDefaults()
	{
		Item.damage = 7;
		Item.crit = 2;
		Item.knockBack = 3;
		Item.useTime = Item.useAnimation = 20;
		Item.DamageType = DamageClass.Melee;
		Item.width = Item.height = 46;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.value = Item.sellPrice(silver: 18);
		Item.rare = ItemRarityID.White;
		Item.UseSound = SoundID.Item1;
		Item.shoot = ModContent.ProjectileType<BambooKendoBladeProj>();
		Item.shootSpeed = 1f;
		Item.autoReuse = true;
		Item.useTurn = true;
	}

	public override void HoldItem(Player player)
	{
		if (!player.ItemAnimationActive)
		{
			player.GetModPlayer<DashSwordPlayer>().holdingSword = true;
			player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, -1.1f * player.direction);
		}
	}
	
	public override bool AltFunctionUse(Player player) => player.GetModPlayer<DashSwordPlayer>().hasDashCharge;

	public override bool CanUseItem(Player player)
	{
		if (player.altFunctionUse == 2)
		{
			Item.noUseGraphic = true;
			Item.noMelee = true;
		}
		else
		{
			Item.noUseGraphic = false;
			Item.noMelee = false;
			return true;
		}

		return true;
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		if (player.altFunctionUse == 2)
			Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);

		return false;
	}

	public void DrawHeld(ref PlayerDrawSet info)
	{
		Texture2D texture = TextureAssets.Item[Type].Value;

		Vector2 drawPos = info.drawPlayer.Center + new Vector2(0, 6 * info.drawPlayer.gravDir + info.drawPlayer.gfxOffY);
		float rotation = .5f * info.drawPlayer.direction + MathHelper.Pi;

		info.DrawDataCache.Add(new DrawData(texture,
			drawPos - Main.screenPosition,
			null,
			Lighting.GetColor((int)info.drawPlayer.Center.X / 16, (int)info.drawPlayer.Center.Y / 16),
			rotation,
			texture.Size() / 2,
			1,
			info.playerEffect,
			0
		));
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient(ModContent.ItemType<StrippedBamboo>(), 20);
		recipe.AddTile(TileID.WorkBenches);
		recipe.Register();
	}
}

public class BambooKendoBladeProj : ModProjectile
{
	public int targetWhoAmI = -1;
	private Vector2 lastPosition;

	private const int DashDuration = 20;
	private const int StrikeDelay = 20;
	private const int FlourishTime = 10;

	public ref float Counter => ref Projectile.ai[0];

	public override string Texture => "SpiritReforged/Content/Bamboo/Items/BambooKendoBlade";

	public override LocalizedText DisplayName => Language.GetText("Mods.SpiritReforged.Items.BambooKendoBlade.DisplayName");

	public override void SetDefaults()
	{
		Projectile.Size = new Vector2(38);
		Projectile.friendly = true;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 2;
	}

	public override void AI()
	{
		var owner = Main.player[Projectile.owner];

		if (Counter < DashDuration) //Ongoing dash
		{
			float quote = Counter / DashDuration;

			if (Counter + 1 == DashDuration)
				Projectile.scale = 1.5f;
			if (Counter > DashDuration - 4)
				owner.velocity *= .5f;
			else
			{
				int magnitude = 20;
				owner.velocity = Vector2.Lerp(owner.velocity, Projectile.velocity * magnitude * 2, quote * quote * quote * quote);

				if (Counter == DashDuration / 2)
				{
					owner.velocity = Projectile.velocity * magnitude;
					SoundEngine.PlaySound(SoundID.DD2_WyvernDiveDown, Projectile.Center);
				}
			}

			owner.velocity.Y += owner.gravity * 8 * quote;
			owner.armorEffectDrawShadow = true;
			owner.armorEffectDrawShadowLokis = true;

			if (targetWhoAmI == -1) //Find a target
			{
				if (lastPosition == Vector2.Zero)
					lastPosition = owner.Center;

				float collisionPoint = 0;
				var crossed = Main.npc.Where(x => (x.CanBeChasedBy(Projectile) || x.type == NPCID.TargetDummy) && Collision.CheckAABBvLineCollision(x.Hitbox.TopLeft(), x.Hitbox.Size(), lastPosition, owner.Center, 15, ref collisionPoint)).OrderBy(x => x.Distance(lastPosition)).FirstOrDefault();
				
				if (crossed != default)
					targetWhoAmI = crossed.whoAmI;
			}

			//Spawn dusts
			var dust = Dust.NewDustDirect(owner.position, owner.width, owner.height, DustID.Smoke, Alpha: 150, Scale: (1f - quote) * 2f);
			dust.velocity = (owner.velocity * Main.rand.NextFloat(.1f)).RotatedByRandom(.5f);
			dust.noGravity = true;
			dust.noLightEmittence = true;
		}

		if (Counter > DashDuration + StrikeDelay)
			Projectile.localAI[0]++;

		Projectile.scale = MathHelper.Max(Projectile.scale - .05f, 1);

		if (Counter < DashDuration / 1.5f)
		{
			Projectile.Center = owner.MountedCenter + Projectile.velocity;
			Projectile.rotation = 3.925f + Projectile.velocity.ToRotation() - .4f * Projectile.direction;
		}
		else
		{
			float quote = Projectile.localAI[0] / FlourishTime;

			Projectile.Center = owner.MountedCenter + Projectile.velocity * (25 - 25 * quote);
			Projectile.rotation = MathHelper.Lerp(MathHelper.PiOver4 + Projectile.velocity.ToRotation(), MathHelper.PiOver4 + (Projectile.direction == 1 ? MathHelper.Pi : 0), quote);
		}

		owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, owner.AngleTo(Projectile.Center) - 1.57f);
		owner.direction = Projectile.direction;
		owner.heldProj = Projectile.whoAmI;

		if (++Counter < DashDuration + StrikeDelay + FlourishTime)
			owner.itemAnimation = owner.itemTime = Projectile.timeLeft = 2;

		if (Counter == 1)
			lastPosition = owner.Center;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		for (int i = 0; i < 20; i++)
		{
			float magnitude = Main.rand.NextFloat();

			var dust = Dust.NewDustPerfect(target.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(30f), DustID.RainCloud, Projectile.velocity * 8 * magnitude, Alpha: 120, Scale: 2f * (1f - magnitude));
			dust.noGravity = true;
		}
		//SpiritMod.primitives.CreateTrail(new AnimePrimTrailTwo(target));
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		if (CanDamage() is true)
		{
			if (targetWhoAmI != -1 && Main.npc[targetWhoAmI] is NPC target && targetHitbox == target.Hitbox)
				return true;
		}

		return false;
	}

	public override bool? CanDamage() => Counter == DashDuration + StrikeDelay;

	public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) { }

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D texture = TextureAssets.Projectile[Type].Value;
		SpriteEffects effects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
		Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), null, Projectile.GetAlpha(lightColor), Projectile.rotation, texture.Size() / 2, Projectile.scale, effects, 0);

		#region trail
		var owner = Main.player[Projectile.owner];

		Texture2D trail = TextureAssets.Projectile[607].Value;
		float opacity = 1f - Counter / DashDuration;
		float angle = owner.AngleTo(lastPosition);

		for (int i = 0; i < 3; i++)
		{
			Vector2 position = owner.Center + i switch
			{
				1 => new Vector2(0, 12).RotatedBy(angle),
				2 => new Vector2(0, -12).RotatedBy(angle),
				_ => new Vector2(0)
			} - Main.screenPosition;

			Main.EntitySpriteDraw(trail, position, null, (lightColor with { A = 0 }) * opacity, angle + MathHelper.PiOver2, trail.Frame().Bottom(), new Vector2(.5f, 1f / trail.Height * owner.Distance(lastPosition)), effects, 0);
		}

		trail = TextureAssets.Projectile[874].Value;
		Main.EntitySpriteDraw(trail, owner.Center - Main.screenPosition, null, (lightColor with { A = 0 }) * opacity, angle, trail.Frame().Left(), new Vector2(1f / trail.Width * owner.Distance(lastPosition), .25f), effects, 0);
		#endregion

		return false;
	}
}
