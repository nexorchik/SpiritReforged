namespace SpiritReforged.Common.TileCommon;

internal class PlanterHandler : ILoadable
{
	/// <summary> Includes modded tile types that use <see cref="TileID.PlanterBox"/> or <see cref="TileID.ClayPot"/> behaviour. </summary>
	public static readonly HashSet<int> PlanterTypes = [];

	public void Load(Mod mod)
	{
		On_WorldGen.CanCutTile += StopCut;
		On_WorldGen.PlaceAlch += ForcePlaceAlch;
		On_WorldGen.CheckAlch += StopCheck;
		On_WorldGen.IsFitToPlaceFlowerIn += AllowFlowers;
	}

	/// <summary> Prevent planted herbs (presumably) from being cut above custom planters, like vanilla does. </summary>
	private static bool StopCut(On_WorldGen.orig_CanCutTile orig, int x, int y, TileCuttingContext context)
	{
		if (Main.tile[x, y + 1] != null && PlanterTypes.Contains(Main.tile[x, y + 1].TileType))
			return false;

		return orig(x, y, context);
	}

	/// <summary> Allow herbs to be placed on custom planters. </summary>
	private static bool ForcePlaceAlch(On_WorldGen.orig_PlaceAlch orig, int x, int y, int style)
	{
		if (Main.tile[x, y + 1] != null && PlanterTypes.Contains(Main.tile[x, y + 1].TileType))
		{
			var tile = Main.tile[x, y];

			tile.HasTile = true;
			tile.TileType = TileID.ImmatureHerbs;
			tile.TileFrameX = (short)(18 * style);
			tile.TileFrameY = 0;

			return true;
		}

		return orig(x, y, style);
	}

	/// <summary> Prevent herbs from killing themselves above custom planters. </summary>
	private static void StopCheck(On_WorldGen.orig_CheckAlch orig, int x, int y)
	{
		if (Main.tile[x, y + 1] != null && PlanterTypes.Contains(Main.tile[x, y + 1].TileType))
			return; //Skips orig

		orig(x, y);
	}

	private static bool AllowFlowers(On_WorldGen.orig_IsFitToPlaceFlowerIn orig, int x, int y, int typeAttemptedToPlace)
	{
		if (Main.tile[x, y + 1] != null && PlanterTypes.Contains(Main.tile[x, y + 1].TileType))
			return true;

		return orig(x, y, typeAttemptedToPlace);
	}

	public void Unload() { }
}
