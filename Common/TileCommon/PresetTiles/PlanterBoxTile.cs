namespace SpiritReforged.Common.TileCommon.PresetTiles;

public abstract class PlanterBoxTile : ModTile, IAutoloadTileItem
{
	public virtual void SetItemDefaults(ModItem item) => item.Item.value = Item.buyPrice(silver: 1);

	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileSolidTop[Type] = true;
		Main.tileSolid[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileTable[Type] = true;

		AddMapEntry(new Color(185, 150, 110));
		DustType = DustID.WoodFurniture;

		PlanterHandler.PlanterTypes.Add(Type);
		Mod.Find<ModItem>(Name + "Item").Item.ResearchUnlockCount = 25;
	}

	public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
	{
		PlanterBoxMerge.Frame(i, j);
		return false;
	}
}

/// <summary> Allows <see cref="PlanterBoxTile"/> and vanilla planter boxes to merge. </summary>
public class PlanterBoxMerge : GlobalTile
{
	public override bool TileFrame(int i, int j, int type, ref bool resetFrame, ref bool noBreak)
	{
		if (type == TileID.PlanterBox)
		{
			Frame(i, j);
			return false;
		}

		return true;
	}

	/// <summary> Mimics <see cref="TileID.PlanterBox"/> framing. </summary>
	public static void Frame(int i, int j)
	{
		var tile = Main.tile[i, j];

		if (Exists(i + 1, j) && Exists(i - 1, j))
			tile.TileFrameX = 18;
		else if (Exists(i - 1, j))
			tile.TileFrameX = 36;
		else if (Exists(i + 1, j))
			tile.TileFrameX = 0;
		else
			tile.TileFrameX = 54;
	}

	/// <summary> Checks whether <see cref="TileID.PlanterBox"/> or <see cref="PlanterBoxTile"/> exists at the given coordinates. </summary>
	private static bool Exists(int i, int j)
	{
		var t = Framing.GetTileSafely(i, j);
		return t.HasTile && (t.TileType == TileID.PlanterBox || TileLoader.GetTile(t.TileType) is PlanterBoxTile);
	}
}