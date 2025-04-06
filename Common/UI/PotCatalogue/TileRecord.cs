namespace SpiritReforged.Content.Underground.Pottery;

/// <summary> Records details for tile bestiary purposes. </summary>
public class TileRecord(string key, int tileType, params int[] tileStyles)
{
	public virtual string Name => Language.GetTextValue($"Mods.SpiritReforged.Items.{key}Item.DisplayName");
	public virtual string Description => Language.GetTextValue("Mods.SpiritReforged.Tiles.Records.Common");
	public virtual byte Rating => 1;

	/// <summary> The value used for internal reference. For the front-facing name, see <see cref="Name"/>. </summary>
	public string key = key;

	public int type = tileType;
	public int[] styles = tileStyles;

	public virtual void DrawIcon(SpriteBatch spriteBatch, Vector2 position, Color color, float scale = 1f)
	{
		const int tileFrame = 18;

		int tileStyle = styles[0];
		var data = TileObjectData.GetTileData(type, 0);
		var texture = TextureAssets.Tile[type].Value;

		//Defaults
		int wrapLimit = 3;
		int width = 2;
		int height = 2;

		if (data != null)
		{
			wrapLimit = data.StyleWrapLimit;
			width = data.Width;
			height = data.Height;
		}

		//Expects a horizontal style
		var source = new Rectangle(tileStyle % wrapLimit * width * tileFrame, tileStyle / wrapLimit * height * tileFrame, 16, 16);

		for (int i = 0; i < 4; i++)
		{
			var newSource = source with { X = source.X + i % 2 * tileFrame, Y = source.Y + i / 2 * tileFrame };
			var origin = i switch
			{
				1 => new Vector2(0, 16),
				2 => new Vector2(16, 0),
				3 => new Vector2(0, 0),
				_ => new Vector2(16, 16)
			};

			spriteBatch.Draw(texture, position, newSource, color, 0, origin, 1, default, 0);
		}
	}

	public virtual void Load(Mod mod) { }
	public virtual void Unload() { }
}

public class BiomeTileRecord(string key, int tileType, params int[] tileStyles) : TileRecord(key, tileType, tileStyles)
{
	public override string Description => Language.GetTextValue("Mods.SpiritReforged.Tiles.Records.Biome");
	public override byte Rating => 2;
}

public class GoldTileRecord(string key, int tileType, params int[] tileStyles) : TileRecord(key, tileType, tileStyles)
{
	public override string Description => Language.GetTextValue("Mods.SpiritReforged.Tiles.Records.CoinPortal");
	public override byte Rating => 5;
}