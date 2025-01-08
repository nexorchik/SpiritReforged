using Terraria.DataStructures;

namespace SpiritReforged.Common.TileCommon.TileSway;

public class TileSwayGlobalTile : GlobalTile
{
	public override bool PreDraw(int i, int j, int type, SpriteBatch spriteBatch)
	{
		if (TileLoader.GetTile(type) is not ISwayInWind wind)
			return true;

		var tile = Framing.GetTileSafely(i, j);
		var data = TileObjectData.GetTileData(tile);

		var frame = new Point(tile.TileFrameX % data.CoordinateFullWidth / 18, tile.TileFrameY % data.CoordinateFullHeight / 18);

		float WindCycle() => wind.SetWindSway(new Point16(i - frame.X, j - frame.Y));

		float rotation;
		int tileOriginY = (data.Origin.Y == 0 && data.Height > 1) ? data.Origin.Y : data.Origin.Y + 1;
		if (tileOriginY == 0)
		{
			float swing = 1f - (1f - (float)(frame.Y + 1) / data.Height) + .5f;
			rotation = WindCycle() * swing * -.1f;
		}
		else
		{
			float swing = 1f - (float)(frame.Y + 1) / tileOriginY + .5f;
			rotation = WindCycle() * swing * .1f;
		}

		wind.ModifyRotation(i, j, ref rotation);
		var rotationOffset = new Vector2(0, Math.Abs(rotation) * 20f) * ((tileOriginY == 0) ? -1 : 1);

		var drawOrigin = new Vector2(-(frame.X * 16) + (data.Origin.X + 1) * 16, -(frame.Y * 16) + tileOriginY * 16);
		if (data.Width % 2 != 0)
			drawOrigin.X -= 8; //Center drawOrigin for multitiles with odd width

		wind.DrawInWind(i, j, spriteBatch, drawOrigin + rotationOffset, rotation, drawOrigin);
		return false;
	}
}
