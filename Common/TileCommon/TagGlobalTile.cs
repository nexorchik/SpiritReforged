using System.Linq;

namespace SpiritReforged.Common.TileCommon;

public partial class TagGlobalTile : GlobalTile
{
	public static List<int> Indestructibles => Instance._indestructibles;
	public static List<int> IndestructiblesUngrounded => Instance._indestructiblesUngrounded;
	public static List<int> HarvestableHerbs => Instance._harvestableHerbs;

	private static TagGlobalTile Instance => ModContent.GetInstance<TagGlobalTile>();

	private readonly List<int> _indestructibles = [];
	private readonly List<int> _indestructiblesUngrounded = [];
	private readonly List<int> _harvestableHerbs = [];

	public void Load(Mod mod)
	{
		var types = typeof(TagGlobalTile).Assembly.GetTypes();
		foreach (var type in types)
		{
			if (typeof(ModTile).IsAssignableFrom(type))
			{
				var tag = (TileTagAttribute)Attribute.GetCustomAttribute(type, typeof(TileTagAttribute));

				if (tag == null || tag.Tags.Length == 0)
					continue;

				int id = mod.Find<ModTile>(type.Name).Type;

				if (tag.Tags.Contains(TileTags.Indestructible))
					_indestructibles.Add(id);

				if (tag.Tags.Contains(TileTags.IndestructibleNoGround))
					_indestructiblesUngrounded.Add(id);

				if (tag.Tags.Contains(TileTags.HarvestableHerb))
					_harvestableHerbs.Add(id);
			}
		}
	}
}
