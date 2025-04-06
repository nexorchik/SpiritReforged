namespace SpiritReforged.Content.Underground.Pottery;

public class CommonTileRecord(string key, int tileType, params int[] tileStyles) : TileRecord(key, tileType, tileStyles)
{
	public override string Description => Language.GetTextValue(LocPath + "Flavour.CommonBlurb");
}

public class UncommonTileRecord(string key, int tileType, params int[] tileStyles) : TileRecord(key, tileType, tileStyles)
{
	public override string Description => Language.GetTextValue(LocPath + "Flavour.BiomeBlurb");
	public override byte Rating => 2;
}

public class GoldTileRecord(string key, int tileType, params int[] tileStyles) : TileRecord(key, tileType, tileStyles)
{
	public override byte Rating => 5;
}