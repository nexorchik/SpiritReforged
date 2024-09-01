using Terraria.DataStructures;

namespace SpiritReforged.Common.TileCommon.TileSway;

public class TileSwayGlobalTile : GlobalTile
{
	public override void Load() => On_Main.DoDraw_Tiles_NonSolid += (On_Main.orig_DoDraw_Tiles_NonSolid orig, Main self) =>
	{
		orig(self);

		var points = TileSwaySystem.Instance.specialDrawPoints;
		for (int i = points.Count - 1; i >= 0; i--)
		{
			var tile = Framing.GetTileSafely(points[i]);
			if (!tile.HasTile || TileLoader.GetTile(tile.TileType) is not ISwayInWind)
			{
				points.Remove(points[i]); //If the tile is invalid, don't draw
				continue;
			}

			PreDrawInWind(points[i], Main.spriteBatch);
		}
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

		if (!tile.HasTile || TileLoader.GetTile(tile.TileType) is not ISwayInWind wind)
			return;

		float swingMult = 1f;
		float windCycle = wind.SetWindSway(topLeft, ref swingMult);

		for (int y = 0; y < data.Height; y++)
		{
			for (int x = 0; x < data.Width; x++)
			{
				(int i, int j) = (topLeft.X + x, topLeft.Y + y);
				tile = Framing.GetTileSafely(i, j);

				if (!tile.HasTile || TileLoader.GetTile(tile.TileType) is not ISwayInWind)
					continue;

				var origin = new Vector2(-(x * 16) + (data.Origin.X + 1) * 16, -(y * 16) + (data.Origin.Y + 1) * 16);

				float swing = swingMult * (data.Origin.Y - y + 1) / (float)data.Height;
				float rotation = windCycle * MathHelper.Max(swing, .1f) * .07f * swing;
				var offset = origin + new Vector2(windCycle, Math.Abs(windCycle) * 4f * swing);

				if (data.Origin.Y == 0 && data.Height > 1) //The origin is at the top of the multitile - reversed
				{
					swing = swingMult * (1f - (data.Origin.Y - y + 1) / (float)data.Height);
					rotation = windCycle * MathHelper.Max(swing, .1f) * -.07f * swing;
					offset = origin + new Vector2(windCycle, Math.Abs(windCycle) * swing * -4f);
				}

				wind.DrawInWind(i, j, spriteBatch, offset, rotation, origin);
			}
		}
	}
}
