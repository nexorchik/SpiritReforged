using System.Linq;

namespace SpiritReforged.Common.TileCommon;

/// <summary> Boilerplate used to help organize tile style for identification. </summary>
public interface INamedStyles
{
	public Dictionary<string, int[]> Styles { get; }
}

public class StyleDatabase : ModSystem
{
	public readonly record struct StyleGroup(string name, int[] styles)
	{
		public readonly string name = name;
		public readonly int[] styles = styles;
	}

	/// <summary> Invoked after <see cref="Groups"/> is fully populated. </summary>
	public static event Action OnPopulateStyleGroups;
	public static readonly Dictionary<int, StyleGroup[]> Groups = [];

	/// <inheritdoc cref="GetName(int, byte)"/>
	public static string GetName(int i, int j)
	{
		var t = Main.tile[i, j];
		int style = TileObjectData.GetTileStyle(t);

		return (style == -1) ? null : GetName(t.TileType, style);
	}

	/// <summary> Gets the registered style name of the tile at the given coordinates. Returns null if not <see cref="INamedStyles"/> or object data is invalid. </summary>
	public static string GetName(int type, byte style)
	{
		if (Groups.TryGetValue(type, out var value))
		{
			foreach (var group in value)
			{
				if (group.styles.Contains(style))
					return group.name;
			}
		}

		return null;
	}

	public override void OnModLoad()
	{
		foreach (var c in Mod.GetContent<ModTile>())
		{
			if (c is INamedStyles nms)
			{
				List<StyleGroup> list = [];

				foreach (string key in nms.Styles.Keys)
					list.Add(new(c.Name + key, nms.Styles[key]));

				Groups.Add(c.Type, [.. list]);
			}
		}

		OnPopulateStyleGroups?.Invoke();
	}

	public override void Unload() => OnPopulateStyleGroups = null;
}