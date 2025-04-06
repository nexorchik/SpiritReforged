using SpiritReforged.Content.Underground.Tiles;
using System.Linq;
using Terraria.ModLoader.IO;

namespace SpiritReforged.Content.Underground.Pottery;

public class CatalogueHandler : ModSystem
{
	/// <summary> Checks whether the tile at the given coordinates corresponds to a record. </summary>
	public static bool Matching(int i, int j, out string name)
	{
		name = null;
		foreach (var value in Records)
		{
			if (value.type == Main.tile[i, j].TileType && HasStyle(i, j, value))
			{
				name = value.key;
				return true;
			}
		}

		return false;

		static bool HasStyle(int i, int j, TileRecord record)
		{
			var t = Main.tile[i, j];
			//Vanilla pots can't use GetTileStyle because they don't have object data
			int style = (t.TileType is TileID.Pots) ? t.TileFrameX / 36 + t.TileFrameY / 36 * 3 : TileObjectData.GetTileStyle(t);

			return record.styles.Contains(style);
		}
	}

	public static readonly HashSet<TileRecord> Records = [];

	#region handle content
	public const string CommonPrefix = "Common_";
	public const string BiomePrefix = "Biome_";

	//Both sort by index
	public static readonly string[] CommonNames = ["Cavern", "Ice", "Jungle", "Dungeon", "Hell", "Corrupt", "Spider", "Crimson", "Pyramid", "Temple", "Marble", "Desert"];
	public static readonly string[] BiomeNames = ["Cavern", "Gold", "Ice", "Desert", "Jungle", "Dungeon", "Corrupt", "Crimson", "Marble", "Hell"];

	public override void SetStaticDefaults()
	{
		//Common
		Records.Add(new CommonTileRecord(CommonPrefix + BiomeNames[0], TileID.Pots, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11));
		const int length = 9;

		for (int i = 1; i < BiomeNames.Length; i++)
		{
			int skip = i * length + 12;
			int[] styles = [skip, skip + 1, skip + 2, skip + 3, skip + 4, skip + 5, skip + 6, skip + 7, skip + 8];

			Records.Add(new CommonTileRecord(CommonPrefix + BiomeNames[i], TileID.Pots, styles));
		}

		//Biome
		for (int i = 0; i < BiomeNames.Length; i++)
		{
			int skip = i * 3;
			int[] styles = [skip, skip + 1, skip + 2];

			if (BiomeNames[i] == "Gold")
			{
				Records.Add(new GoldTileRecord(BiomePrefix + BiomeNames[i], ModContent.TileType<BiomePots>(), styles)); //Add a specialised record
				continue;
			}

			Records.Add(new UncommonTileRecord(BiomePrefix + BiomeNames[i], ModContent.TileType<BiomePots>(), styles));
		}
	}
	#endregion
}

internal class RecordPlayer : ModPlayer
{
	/// <summary> The list of unlocked entries saved per player. </summary>
	private IList<string> _validated = [];

	public void Validate(string name) => _validated.Add(name);
	public bool IsValidated(string name) => _validated.Contains(name);

	public override void SaveData(TagCompound tag) => tag[nameof(_validated)] = _validated;
	public override void LoadData(TagCompound tag) => _validated = tag.GetList<string>(nameof(_validated));
}

internal class RecordGlobalTile : GlobalTile
{
	public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
	{
		if (effectOnly || fail)
			return;

		if (CatalogueHandler.Matching(i, j, out string name))
			Main.LocalPlayer.GetModPlayer<RecordPlayer>().Validate(name);
	}
}