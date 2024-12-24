using System.Linq;
using Terraria.DataStructures;

namespace SpiritReforged.Content.Desert.CactusStaff;

public class CactusStaff : ModItem
{
	private bool fail = false;

	public override void SetDefaults()
	{
		Item.damage = 6;
		Item.DamageType = DamageClass.Magic;
		Item.mana = 7;
		Item.width = 38;
		Item.height = 42;
		Item.useTime = 20;
		Item.useAnimation = 20;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.noMelee = true;
		Item.knockBack = 4f;
		Item.value = 200;
		Item.rare = ItemRarityID.Blue;
		Item.UseSound = SoundID.Item8;
		Item.shoot = ModContent.ProjectileType<CactusWallProj>();
		Item.shootSpeed = 8f;
		Item.autoReuse = false;
	}

	public override bool CanUseItem(Player player) => !Collision.SolidCollision(Main.MouseWorld, 16, 16);

	public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
	{
		velocity = Vector2.Zero;
		position = Main.MouseWorld.ToTileCoordinates().ToWorldCoordinates(8, 4);

		while (!WorldGen.SolidTile(position.ToTileCoordinates()) && !Main.tileSolidTop[Framing.GetTileSafely(position.ToTileCoordinates()).TileType])
		{
			position.Y += 8;

			if (position.Y > Main.MouseWorld.Y + 4000)
			{
				fail = true;
				return;
			}
		}

		position.Y -= 32;
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		if (fail)
		{
			fail = false;
			return false;
		}

		Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, player.ownedProjectileCounts[Item.shoot]);

		if (player.ownedProjectileCounts[Item.shoot] >= 4)
		{
			var projectile = Main.projectile.Where(x => x.active && x.owner == player.whoAmI && x.type == Item.shoot).OrderBy(x => x.timeLeft).FirstOrDefault();
			if (projectile != default)
				projectile.Kill();
		}

		return false;
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe(1);
		recipe.AddIngredient(ItemID.Cactus, 12);
		recipe.AddIngredient(ItemID.FallenStar, 1);
		recipe.AddTile(TileID.WorkBenches);
		recipe.Register();
	}
}
