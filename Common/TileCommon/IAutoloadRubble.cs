using MonoMod.RuntimeDetour;
using SpiritReforged.Common.ItemCommon;
using System.Linq;
using System.Reflection;
using Terraria.DataStructures;

namespace SpiritReforged.Common.TileCommon;

/// <summary> Autoloads a rubble variant for this tile. Must be <see cref="NameableTile"/> for autoloading to work. </summary>
public interface IAutoloadRubble
{
	public struct RubbleData(int item, RubbleSize size, int[] styles = null)
	{
		public int item = item;
		public RubbleSize size = size;

		private readonly int[] styles = styles;
		public readonly int[] Styles => styles ?? [0];
	}

	public enum RubbleSize : byte
	{
		Small = 0,
		Medium = 1,
		Large = 2
	}

	public RubbleData Data { get; }
}

internal class AutoloadedRubbleHandler : ILoadable
{
	public delegate bool orig_AddContent(Mod self, ILoadable instance);

	private static Hook CustomHook = null;
	internal static readonly HashSet<int> RubbleTypes = [];

	private static bool Autoloads(Type type) => typeof(IAutoloadRubble).IsAssignableFrom(type);

	/// <summary> Must be called during loading and after all other types have been loaded. </summary>
	public static void Initialize(Mod mod)
	{
		AddHook(mod);
		var content = mod.GetContent<NameableTile>().ToArray();

		for (int i = 0; i < content.Length; i++)
		{
			if (Autoloads(content[i].GetType()))
			{
				var instance = (NameableTile)Activator.CreateInstance(content[i].GetType());

				mod.AddContent(instance);
				RubbleTypes.Add(instance.Type);
			}
		}
	}

	internal static void AddHook(Mod mod)
	{
		MethodInfo info = mod.GetType().GetMethod("AddContent", BindingFlags.Instance | BindingFlags.Public, [typeof(ILoadable)]);
		CustomHook = new Hook(info, HookAddContent, true);
	}

	public static bool HookAddContent(orig_AddContent orig, Mod self, ILoadable instance)
	{
		if (instance is NameableTile nameable && Autoloads(instance.GetType()))
			nameable.ChangeName(nameable.BaseName + "Rubble");

		return orig(self, instance);
	}

	public void Load(Mod mod) { }

	public void Unload()
	{
		CustomHook.Undo();
		CustomHook = null;
	}
}

internal class RubbleGlobalTile : GlobalTile
{
	public static bool IsRubble(int type) => AutoloadedRubbleHandler.RubbleTypes.Contains(type);

	public override void SetStaticDefaults()
	{
		foreach (int type in AutoloadedRubbleHandler.RubbleTypes)
		{
			var data = ((IAutoloadRubble)TileLoader.GetTile(type)).Data;
			var objData = TileObjectData.GetTileData(type, 0);

			if (objData != null)
				objData.RandomStyleRange = 0;

			if (data.size == IAutoloadRubble.RubbleSize.Small)
				FlexibleTileWand.RubblePlacementSmall.AddVariations(data.item, type, data.Styles);
			else if (data.size == IAutoloadRubble.RubbleSize.Medium)
				FlexibleTileWand.RubblePlacementMedium.AddVariations(data.item, type, data.Styles);
			else if (data.size == IAutoloadRubble.RubbleSize.Large)
				FlexibleTileWand.RubblePlacementLarge.AddVariations(data.item, type, data.Styles);
		}
	}

	public override bool CanDrop(int i, int j, int type) => !IsRubble(type);

	public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
	{
		if (!Main.dedServ && IsRubble(type) && TileObjectData.IsTopLeft(i, j))
		{
			var data = ((IAutoloadRubble)TileLoader.GetTile(type)).Data;
			var objData = TileObjectData.GetTileData(type, 0);
			Vector2 position = new Vector2(i + objData.Width / 2, j + objData.Height / 2) * 16;

			ItemMethods.NewItemSynced(new EntitySource_TileBreak(i, j), data.item, position, true);
		}
	}
}