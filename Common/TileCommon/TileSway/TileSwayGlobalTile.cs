using Terraria.DataStructures;

namespace SpiritReforged.Common.TileCommon.TileSway;

public class TileSwayGlobalTile : GlobalTile
{
	public override void Load() => On_Main.DoDraw_Tiles_NonSolid += (On_Main.orig_DoDraw_Tiles_NonSolid orig, Main self) =>
	{
		orig(self);

		var points = TileSwaySystem.Instance.specialDrawPoints;
		foreach (var p in points)
			PreDrawInWind(p, Main.spriteBatch);
	};

	public override bool PreDraw(int i, int j, int type, SpriteBatch spriteBatch)
	{
		Tile tile = Framing.GetTileSafely(i, j);
		if (TileLoader.GetTile(tile.TileType) is ISwayInWind)
		{
			var data = TileObjectData.GetTileData(tile);
			if (tile.TileFrameX % (data.Width * 18) == 0 && tile.TileFrameY % (data.Height * 18) == 0)
				TileSwaySystem.AddDrawPoint(new Point16(i, j));

			return false;
		}

		return true;
	}

	/// <summary> Automatically does most of the math related to wind effects then calls <see cref="ISwayInWind.DrawInWind"/>. </summary>
	/// <param name="topLeft"> The top left tile in the multitile. </param>
	/// <param name="spriteBatch"> The spritebatch. </param>
	public static void PreDrawInWind(Point16 topLeft, SpriteBatch spriteBatch)
	{
		var tile = Framing.GetTileSafely(topLeft);
		var data = TileObjectData.GetTileData(tile);

		if (data is null)
			return;

		float windCycle = 0;
		int tileOriginY = (data.Origin.Y == 0 && data.Height > 1) ? data.Origin.Y : data.Origin.Y + 1;

		for (int y = 0; y < data.Height; y++)
		{
			for (int x = 0; x < data.Width; x++)
			{
				(int i, int j) = (topLeft.X + x, topLeft.Y + y);
				tile = Framing.GetTileSafely(i, j);

				if (!tile.HasTile || TileLoader.GetTile(tile.TileType) is not ISwayInWind wind)
					continue;

				if (i == topLeft.X && j == topLeft.Y)
					windCycle = wind.SetWindSway(topLeft); //Set wind cycle using our interface

				float rotation;

				if (tileOriginY == 0)
				{
					float swing = 1f - (1f - (float)(y + 1) / data.Height) + .5f;
					rotation = windCycle * swing * -.1f;
				}
				else
				{
					float swing = 1f - (float)(y + 1) / tileOriginY + .5f;
					rotation = windCycle * swing * .1f;
				}

				wind.ModifyRotation(i, j, ref rotation);
				var rotationOffset = new Vector2(0, Math.Abs(rotation) * 20f) * ((tileOriginY == 0) ? -1 : 1);

				var drawOrigin = new Vector2(-(x * 16) + (data.Origin.X + 1) * 16, -(y * 16) + tileOriginY * 16);
				if (data.Width % 2 != 0)
					drawOrigin.X -= 8; //Center origin for multitiles with odd width

				wind.DrawInWind(i, j, spriteBatch, drawOrigin + rotationOffset, rotation, drawOrigin);
			}
		}
	}
}
