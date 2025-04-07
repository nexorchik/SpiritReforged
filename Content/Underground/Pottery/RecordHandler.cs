using SpiritReforged.Common.TileCommon;
using System.Linq;
using Terraria.ModLoader.IO;

namespace SpiritReforged.Content.Underground.Pottery;

public interface IRecordTile : INamedStyles
{
	public void AddRecord(int type, StyleDatabase.StyleGroup group) => RecordHandler.Records.Add(new TileRecord(group.name, type, group.styles));
}

public class RecordHandler : ModSystem
{
	public static readonly HashSet<TileRecord> Records = [];

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

	public override void SetStaticDefaults()
	{
		foreach (int type in StyleDatabase.Groups.Keys)
		{
			if (TileLoader.GetTile(type) is not IRecordTile r)
				continue;

			foreach (var group in StyleDatabase.Groups[type])
				r.AddRecord(type, group);
		}

		AddPotRecords();
	}

	/// <summary> Adds all records for vanilla pots. </summary>
	private static void AddPotRecords()
	{
		int type = TileID.Pots;

		foreach (var group in StyleDatabase.Groups[type])
			Records.Add(new TileRecord(group.name, type, group.styles));
	}
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

		if (RecordHandler.Matching(i, j, out string name))
		{
			var p = Main.player[Player.FindClosest(new Vector2(i, j).ToWorldCoordinates(16, 16), 0, 0)];
			p.GetModPlayer<RecordPlayer>().Validate(name);
		}
	}
}