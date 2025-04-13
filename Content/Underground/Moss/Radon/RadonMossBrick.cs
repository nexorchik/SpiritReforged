using SpiritReforged.Common.ItemCommon;
using SpiritReforged.Common.TileCommon;
using SpiritReforged.Common.Visuals.Glowmasks;
using SpiritReforged.Common.WallCommon;
using Terraria.Audio;

namespace SpiritReforged.Content.Underground.Moss.Radon;

[AutoloadGlowmask("255,255,255")]
public class RadonMossBrick : ModTile, IAutoloadTileItem
{
	public void AddItemRecipes(ModItem item) => item.CreateRecipe(4).AddIngredient(ItemMethods.AutoItemType<RadonMoss>()).AddIngredient(ItemID.ClayBlock, 10).AddTile(TileID.Furnaces).Register();
	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileBlockLight[Type] = true;
		Main.tileBrick[Type] = true;
		Main.tileMergeDirt[Type] = true;

		AddMapEntry(new Color(252, 248, 3));
		HitSound = SoundID.Tink;
		DustType = DustID.YellowTorch;

		//Set item StaticDefaults
		var item = this.AutoItem();
		item.ResearchUnlockCount = 100;

	}
	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		Lighting.AddLight(new Vector2(i, j).ToWorldCoordinates(), Color.Yellow.ToVector3() * .3f);
		return true;
	}
}

public class RadonMossBrickWall : ModWall, IAutoloadWallItem
{
	public void AddItemRecipes(ModItem item)
	{
		item.CreateRecipe(4).AddIngredient(ItemMethods.AutoItemType<RadonMossBrick>()).AddTile(TileID.WorkBenches).Register();

		//Allow wall items to be crafted back into base materials
		Recipe.Create(ItemMethods.AutoItemType<RadonMossBrick>()).AddIngredient(item.Type, 4)
			.AddTile(TileID.WorkBenches).Register();
	}

	public override void SetStaticDefaults()
	{
		Main.wallHouse[Type] = true;
		AddMapEntry(new Color(126, 124, 1));
	}

	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
	{
		r = 0.45f;
		g = 0.425f;
		b = 0.05f;
	}
}