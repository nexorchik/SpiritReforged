using System.Linq;
using Terraria.DataStructures;

namespace SpiritReforged.Common.TileCommon;
internal interface IRubble
{
	/// <summary> Data used to define rubble tiles. </summary>
	/// <param name="item"> The item drop. </param>
	/// <param name="size"> The size (small/medium/large). </param>
	/// <param name="styles"> The tile styles to use. </param>
	public struct RubbleData(int item, RubbleSize size, int[] styles = null)
	{
		/// <summary> The item drop. </summary>
		public int item = item;

		/// <summary> The size (small/medium/large). </summary>
		public RubbleSize size = size;

		/// <summary> The tile styles to use. </summary>
		public readonly int[] styles = styles;
	}

	/// <summary> Size settings according to <see cref="FlexibleTileWand"/> rubble placement. </summary>
	public enum RubbleSize : byte
	{
		/// <summary><see cref="FlexibleTileWand.RubblePlacementSmall"/></summary>
		Small = 0,
		/// <summary><see cref="FlexibleTileWand.RubblePlacementMedium"/></summary>
		Medium = 1,
		/// <summary><see cref="FlexibleTileWand.RubblePlacementLarge"/></summary>
		Large = 2
	}

	/// <summary> <see cref="RubbleData"/> belonging to this rubble type. </summary>
	public RubbleData Data { get; }
}

internal class RubbleSystem : ModSystem
{
	public override void PostSetupContent()
	{
		var content = Mod.GetContent<ModTile>().Where(x => x is IRubble);

		foreach (var tile in content)
		{
			int type = tile.Type;

			var data = ((IRubble)TileLoader.GetTile(type)).Data;
			var objData = TileObjectData.GetTileData(type, 0);
			int[] styles = data.styles;

			if (objData != null)
			{
				if (styles == null && objData.RandomStyleRange > 0) //Populate styles automatically from RandomStyleRange
				{
					styles = new int[objData.RandomStyleRange];

					for (int i = 0; i < styles.Length; i++)
						styles[i] = i;
				}

				objData.RandomStyleRange = 0;
			}

			styles ??= [0];
			TileID.Sets.CanDropFromRightClick[type] = false;

			if (data.size == IRubble.RubbleSize.Small)
				FlexibleTileWand.RubblePlacementSmall.AddVariations(data.item, type, styles);
			else if (data.size == IRubble.RubbleSize.Medium)
				FlexibleTileWand.RubblePlacementMedium.AddVariations(data.item, type, styles);
			else if (data.size == IRubble.RubbleSize.Large)
				FlexibleTileWand.RubblePlacementLarge.AddVariations(data.item, type, styles);
		}
	}
}

internal class RubbleGlobalTile : GlobalTile
{
	private static bool IsRubble(int type) => TileLoader.GetTile(type) is IRubble;

	public override bool CanDrop(int i, int j, int type) => !IsRubble(type);

	public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
	{
		if (!fail && Main.netMode != NetmodeID.MultiplayerClient && IsRubble(type) && TileObjectData.IsTopLeft(i, j))
		{
			var data = ((IRubble)TileLoader.GetTile(type)).Data;
			var objData = TileObjectData.GetTileData(type, 0);
			var position = new Vector2(i + objData.Width / 2f, j + objData.Height / 2f) * 16;

			Item.NewItem(new EntitySource_TileBreak(i, j), position, data.item, noGrabDelay: true);
		}
	}
}
