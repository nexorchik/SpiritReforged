using MonoMod.RuntimeDetour;
using System.Linq;
using System.Reflection;
using Terraria.DataStructures;

namespace SpiritReforged.Common.TileCommon;

/// <summary> Autoloads a rubble variant for this tile. Must be <see cref="NameableTile"/> for autoloading to work.<br/>
/// Rubbles are stored by type and can be checked using <see cref="RubbleSystem.IsRubble"/>. </summary>
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

internal class RubbleSystem : ModSystem
{
	public delegate bool orig_AddContent(Mod self, ILoadable instance);

	private static Hook CustomHook = null;
	private static readonly HashSet<int> RubbleTypes = [];

	/// <returns> Whether <paramref name="type"/> is an autoloaded rubble tile. </returns>
	public static bool IsRubble(int type) => RubbleTypes.Contains(type);
	private static bool Autoloads(Type type) => typeof(IAutoloadRubble).IsAssignableFrom(type);

	/// <summary> Initializes rubble autoloading. Must be called during loading and after all other content has been loaded. </summary>
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

	private static void AddHook(Mod mod)
	{
		MethodInfo info = mod.GetType().GetMethod("AddContent", BindingFlags.Instance | BindingFlags.Public, [typeof(ILoadable)]);
		CustomHook = new Hook(info, HookAddContent, true);
	}

	/// <summary> Changes <see cref="NameableTile"/> names right before being added to content. </summary>
	public static bool HookAddContent(orig_AddContent orig, Mod self, ILoadable instance)
	{
		if (instance is NameableTile nameable && Autoloads(instance.GetType()))
			nameable.ChangeName(nameable.BaseName + "Rubble");

		return orig(self, instance);
	}

	public override void PostSetupContent()
	{
		foreach (int type in RubbleTypes)
		{
			var data = ((IAutoloadRubble)TileLoader.GetTile(type)).Data;
			var objData = TileObjectData.GetTileData(type, 0);

			if (objData != null)
				objData.RandomStyleRange = 0;

			TileID.Sets.CanDropFromRightClick[type] = false;

			if (data.size == IAutoloadRubble.RubbleSize.Small)
				FlexibleTileWand.RubblePlacementSmall.AddVariations(data.item, type, data.Styles);
			else if (data.size == IAutoloadRubble.RubbleSize.Medium)
				FlexibleTileWand.RubblePlacementMedium.AddVariations(data.item, type, data.Styles);
			else if (data.size == IAutoloadRubble.RubbleSize.Large)
				FlexibleTileWand.RubblePlacementLarge.AddVariations(data.item, type, data.Styles);
		}
	}

	public override void Unload()
	{
		CustomHook.Undo();
		CustomHook = null;
	}
}

/// <summary> Prevents normal item drops for autoloaded rubble tiles. </summary>
internal class RubbleGlobalTile : GlobalTile
{
	public override bool CanDrop(int i, int j, int type) => !RubbleSystem.IsRubble(type);

	public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
	{
		if (Main.netMode != NetmodeID.MultiplayerClient && RubbleSystem.IsRubble(type) && TileObjectData.IsTopLeft(i, j))
		{
			var data = ((IAutoloadRubble)TileLoader.GetTile(type)).Data;
			var objData = TileObjectData.GetTileData(type, 0);
			var position = new Vector2(i + objData.Width / 2f, j + objData.Height / 2f) * 16;

			Item.NewItem(new EntitySource_TileBreak(i, j), position, data.item, noGrabDelay: true);
		}
	}
}