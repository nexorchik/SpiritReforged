using Mono.Cecil;

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

		//Defaults- useful for vanilla pots which have no object data
		int wrapLimit = 3;
		int width = 2;
		int height = 2;

		if (data != null)
		{
			wrapLimit = data.StyleWrapLimit;
			width = data.Width;
			height = data.Height;
		}

		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				//Expects a horizontal style
				int startX = (tileStyle % wrapLimit * width + x) * tileFrame;
				int startY = (tileStyle / wrapLimit * height + y) * tileFrame;

				var source = new Rectangle(startX, startY, 16, 16);
				var center = new Vector2(width * 16 / 2, height * 16 / 2);
				var drawPos = position + new Vector2(x * 16, y * 16) - center;

				spriteBatch.Draw(texture, drawPos, source, color, 0, Vector2.Zero, 1, default, 0);
			}
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