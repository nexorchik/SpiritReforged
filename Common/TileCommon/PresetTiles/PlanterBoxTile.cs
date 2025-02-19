namespace SpiritReforged.Common.TileCommon.PresetTiles;

public abstract class PlanterBoxTile : ModTile, IAutoloadTileItem
{
	public override void Load() => On_WorldGen.CanCutTile += StopCut;

	/// <summary> Prevent planted herbs (presumably) from being cut above planter boxes, like vanilla does. </summary>
	private static bool StopCut(On_WorldGen.orig_CanCutTile orig, int x, int y, TileCuttingContext context)
	{
		if (Main.tile[x, y + 1] != null && TileLoader.GetTile(Main.tile[x, y + 1].TileType) is PlanterBoxTile)
			return false;

		return orig(x, y, context);
	}

	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileSolidTop[Type] = true;
		Main.tileSolid[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileTable[Type] = true;
		Main.tileLavaDeath[Type] = true;

		TileID.Sets.DisableSmartCursor[Type] = true;

		this.Merge(TileID.PlanterBox);
		AddMapEntry(new Color(179, 146, 107));
		DustType = DustID.WoodFurniture;
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

	private static bool Exists(int i, int j)
	{
		var t = Framing.GetTileSafely(i, j);
		return t.HasTile && (t.TileType == TileID.PlanterBox || TileLoader.GetTile(t.TileType) is PlanterBoxTile);
	}
}