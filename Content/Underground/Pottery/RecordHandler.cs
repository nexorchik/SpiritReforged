using SpiritReforged.Common.TileCommon;
using SpiritReforged.Content.Underground.Tiles;
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
			int type = value.type;

			if (type == ModContent.TileType<Pots>())
				type = TileID.Pots; //Pretend type is actually vanilla

			if (type == Main.tile[i, j].TileType && HasStyle(i, j, value))
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
	}
}

internal class RecordPlayer : ModPlayer
{
	/// <summary> The list of unlocked entries saved per player. </summary>
	private IList<string> _validated = [];
	/// <summary> The list of entries newly discovered by the player. Not saved. </summary>
	private readonly HashSet<string> _newAndShiny = [];

	public bool IsNew(string name) => _newAndShiny.Contains(name);
	public bool RemoveNew(string name) => _newAndShiny.Remove(name);

	/// <summary> Unlocks the entry of <paramref name="name"/> for this player. </summary>
	public void Validate(string name)
	{
		if (!IsValidated(name))
			_newAndShiny.Add(name);

		_validated.Add(name);
	}

	/// <returns> Whether the entry of <paramref name="name"/> is unlocked for this player. </returns>
	public bool IsValidated(string name) => _validated.Contains(name);

	public override void SaveData(TagCompound tag) => tag[nameof(_validated)] = _validated;
	public override void LoadData(TagCompound tag) => _validated = tag.GetList<string>(nameof(_validated));
}

internal class RecordGlobalTile : GlobalTile
{
	public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
	{
		const int maxDistance = 800;

		if (effectOnly || fail)
			return;

		if (RecordHandler.Matching(i, j, out string name))
		{
			var world = new Vector2(i, j).ToWorldCoordinates();
			var p = Main.player[Player.FindClosest(world, 16, 16)];

			if (p.DistanceSQ(world) < maxDistance * maxDistance)
				p.GetModPlayer<RecordPlayer>().Validate(name);
		}
	}
}