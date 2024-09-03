namespace SpiritReforged.Content.Savanna.Tiles.AcaciaTree;

public class AcaciaTree : ModPalmTree
{
	internal static Asset<Texture2D> Texture, TopsTexture;

	public override TreePaintingSettings TreeShaderSettings => new()
	{
		UseSpecialGroups = true,
		SpecialGroupMinimalHueValue = 11f / 72f,
		SpecialGroupMaximumHueValue = 0.25f,
		SpecialGroupMinimumSaturationValue = 0.88f,
		SpecialGroupMaximumSaturationValue = 1f
	};

	public override void SetStaticDefaults()
	{
		GrowsOnTileId = [ModContent.TileType<SavannaGrass>()];

		if (!Main.dedServ)
		{
			var mod = SpiritReforgedMod.Instance;

			Texture = mod.Assets.Request<Texture2D>("Content/Savanna/Tiles/AcaciaTree/AcaciaTree");
			TopsTexture = mod.Assets.Request<Texture2D>("Content/Savanna/Tiles/AcaciaTree/AcaciaTree_Tops");
		}
	}

	public override int SaplingGrowthType(ref int style) => ModContent.TileType<AcaciaSapling>();
	public override int DropWood() => ItemID.Wood;
	public override Asset<Texture2D> GetTexture() => Texture;
	public override Asset<Texture2D> GetOasisTopTextures() => TopsTexture; //This is never used
	public override Asset<Texture2D> GetTopTextures() => TopsTexture;
}
