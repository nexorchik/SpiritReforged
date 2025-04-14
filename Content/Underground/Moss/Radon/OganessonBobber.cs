using SpiritReforged.Common.ItemCommon;
using SpiritReforged.Common.Visuals.Glowmasks;

namespace SpiritReforged.Content.Underground.Moss.Oganesson;

[AutoloadGlowmask("255, 255, 255")]
public class OganessonMossBobberItem : AccessoryItem
{
	public override void SetDefaults()
	{
		Item.width = 26;
		Item.height = 34;
		Item.value = Item.sellPrice(0, 1, 0, 0);
		Item.rare = ItemRarityID.Blue;
		Item.accessory = true;
	}

	public override void SafeUpdateAccessory(Player player, bool hideVisual)
	{
		player.fishingSkill += 10;
		player.accFishingBobber = true;
		player.overrideFishingBobber = ModContent.ProjectileType<OganessonMossBobber>();
	}

	public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI) => Lighting.AddLight(Item.Center, Color.White.ToVector3() * 0.25f);

	public void AddItemRecipes(ModItem item) => item.CreateRecipe(1).AddIngredient(ItemID.FishingBobberGlowingStar).AddIngredient(ModContent.ItemType<OganessonMossItem>(), 5).AddTile(TileID.TinkerersWorkbench).Register();
}

[AutoloadGlowmask("255,255,255")]
public class OganessonMossBobber : ModProjectile
{
	public override void SetDefaults()
	{
		Projectile.CloneDefaults(ProjectileID.BobberWooden);
		Projectile.aiStyle = 61;
		Projectile.bobber = true;
		AIType = ProjectileID.FishingBobber;
		DrawOriginOffsetY = -8;
	}

	public override void AI()
	{
		if (!Main.dedServ)
			Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.25f);
	}
}