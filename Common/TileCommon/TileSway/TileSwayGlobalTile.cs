using Terraria.DataStructures;

namespace SpiritReforged.Common.TileCommon.TileSway;

public class TileSwayGlobalTile : GlobalTile
{
	public override bool PreDraw(int i, int j, int type, SpriteBatch spriteBatch)
	{
		Tile tile = Framing.GetTileSafely(i, j);
		if (TileLoader.GetTile(tile.TileType) is ISwayInWind)
		{
			var data = TileObjectData.GetTileData(tile);
			if (tile.TileFrameX % (data.Width * 18) == 0 && tile.TileFrameY % (data.Height * 18) == 0)
				PreDrawInWind(new Point16(i, j), spriteBatch);

			return false;
		}

		return true;
	}

	/// <summary> Automatically does most of the math related to wind effects then calls <see cref="ISwayInWind.DrawInWind"/>. </summary>
	/// <param name="topLeft"> The top left tile in the multitile. </param>
	/// <param name="spriteBatch"> The spritebatch used for drawing. </param>
	/// <param name="restartSpriteBatch"> Whether the spritebatch should be restarted for better visuals in wind. </param>
	public static void PreDrawInWind(Point16 topLeft, SpriteBatch spriteBatch, bool restartSpriteBatch = true)
	{
		var tile = Framing.GetTileSafely(topLeft);
		var data = TileObjectData.GetTileData(tile);

		if (!tile.HasTile || TileLoader.GetTile(tile.TileType) is not ISwayInWind wind)
			return;

		if (restartSpriteBatch)
		{
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Matrix.Identity);
		}

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

				var origin = new Vector2(-(x * 16) + data.Origin.X * 16, -(y * 16) + data.Origin.Y * 16);

				float swing = swingMult * (data.Origin.Y - y + 1) / (float)data.Height;
				float rotation = windCycle * MathHelper.Max(swing, .05f) * .05f * swing;
				var offset = origin + new Vector2(windCycle, Math.Abs(windCycle) * 2f * swing);

				if (data.Origin.Y == 0) //The origin is at the top of the tile - reversed
				{
					swing = 1f - (data.Origin.Y - y + 1) / (float)data.Height;
					rotation = windCycle * MathHelper.Max(swing, .1f) * -.07f * swing;
					offset = origin + new Vector2(windCycle, Math.Abs(windCycle) * -4f * swing);
				}

				wind.DrawInWind(i, j, spriteBatch, offset, rotation, origin);
			}
		}

		if (restartSpriteBatch)
		{
			spriteBatch.End();
			spriteBatch.Begin();
		}
	}
}
