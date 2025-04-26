using SpiritReforged.Common.ItemCommon;
using SpiritReforged.Common.TileCommon;
using SpiritReforged.Common.Visuals.Glowmasks;

namespace SpiritReforged.Content.Underground.Moss.Radon;

[AutoloadGlowmask("255,255,255")]
public class RadonMossBrick : ModTile, IAutoloadTileItem
{
	public void AddItemRecipes(ModItem item) => item.CreateRecipe(4).AddIngredient(ItemMethods.AutoItemType<RadonMoss>()).AddIngredient(ItemID.ClayBlock, 10).AddTile(TileID.Furnaces).Register();

	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileLighted[Type] = true;
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

	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) => (r, g, b) = (0.45f, 0.425f, 0.05f);
}