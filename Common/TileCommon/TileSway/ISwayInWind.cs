using Terraria.DataStructures;

namespace SpiritReforged.Common.TileCommon.TileSway;

/// <summary> Helps draw a tile affected by wind - rotates around tile origin. </summary>
public interface ISwayInWind
{
	/// <summary> Add natural wind cycle and grid wind push math here. </summary>
	/// <param name="topLeft"> The top left tile in the multitile. </param>
	/// <param name="swayMult"> Multiplier for sway rotation on vertical segments. </param>
	public float SetWindSway(Point16 topLeft, ref float swayMult)
	{
		var data = TileObjectData.GetTileData(Framing.GetTileSafely(topLeft));
		float rotation = Main.instance.TilesRenderer.GetWindCycle(topLeft.X, topLeft.Y, TileSwaySystem.Instance.SunflowerWindCounter);

		if (!WorldGen.InAPlaceWithWind(topLeft.X, topLeft.Y, data.Width, data.Height))
			rotation = 0f;

		return rotation + TileSwayHelper.GetHighestWindGridPushComplex(topLeft.X, topLeft.Y, data.Width, data.Height, 30, 2f, 1, true);
	}

	public void DrawInWind(int i, int j, SpriteBatch spriteBatch, Vector2 offset, float rotation, Vector2 origin);
}
