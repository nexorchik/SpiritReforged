using Microsoft.CodeAnalysis.Differencing;
using SpiritReforged.Common.WorldGeneration.Noise;
using SpiritReforged.Common.WorldGeneration.Tools;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace SpiritReforged.Common.WorldGeneration.Micropasses.Passes.CaveEntrances;

internal class ArchwayEntrance : CaveEntrance
{
	public override CaveEntranceType Type => CaveEntranceType.Archways;

	public override void Generate(int x, int y)
	{
		FastNoiseLite noise = new();
		TunnelBetween(new Point16(x, y), new Point16(x, y + 50), noise);
	}

	private void TunnelBetween(Point16 entrance, Point16 exit, FastNoiseLite noise)
	{
		var current = entrance.ToVector2();
		var end = exit.ToVector2();
		List<Vector2> positions = [];
		bool flip = WorldGen.genRand.NextBool();
		float scale = WorldGen.genRand.NextFloat(0.8f, 1.3f);

		while (true)
		{
			positions.Add(current);

			current.X = entrance.X + WorldGen.genRand.Next(30, 60) * (flip ? -1 : 1);
			current.Y += WorldGen.genRand.Next(15, 25);
			flip = !flip;

			if (current.Y > end.Y)
			{
				current = end;
				positions.Add(current);
				break;
			}
		}

		var points = SmoothTunnel.GeneratePoints([.. positions], new SmoothTunnel.VariationData(5, (0.2f, 0.4f), (5, 10), 0.05f));

		for (int i = 0; i < points.Length; i++)
		{
			Vector2 point = points[i];
			TunnelDig(noise, point, MathHelper.Lerp(scale, 0.5f, i / (float)points.Length));
		}
	}

	private static void TunnelDig(FastNoiseLite noise, Vector2 item, float scale)
	{
		float mul = 1.2f + MathF.Abs(noise.GetNoise(item.X, item.Y));

		CircleOpening(item, 4.5f * mul * scale);
		CircleOpening(item, WorldGen.genRand.Next(3, 6) * mul * scale);

		if (WorldGen.genRand.NextBool(6))
			WallCircleOpening(item, WorldGen.genRand.Next(4, 6) * mul * scale);
	}

	/// <summary>
	/// Digs a perfectly circular space into tiles.
	/// </summary>
	/// <param name="pos">Center of the circle.</param>
	/// <param name="size">Radius of the circle.</param>
	public static void CircleOpening(Vector2 pos, float size)
	{
		for (int i = (int)(pos.X - size); i < (int)pos.X + size; ++i)
		{
			for (int j = (int)(pos.Y - size); j < (int)pos.Y + size; ++j)
			{
				float distance = Vector2.Distance(pos, new Vector2(i, j));

				if (distance < size)
				{
					Tile tile = Main.tile[i, j];
					tile.HasTile = false;

					if (tile.WallType > 0)
					{
						tile.WallType = distance > size * 0.9f ? WallID.Dirt : WallID.GrassUnsafe;
					}
				}
			}
		}
	}

	/// <summary>
	/// Digs a perfectly circular space into walls.
	/// </summary>
	/// <param name="pos">Center of the circle.</param>
	/// <param name="size">Radius of the circle.</param>
	public static void WallCircleOpening(Vector2 pos, float size)
	{
		for (int i = (int)(pos.X - size); i < (int)pos.X + size; ++i)
			for (int j = (int)(pos.Y - size); j < (int)pos.Y + size; ++j)
				if (Vector2.DistanceSquared(pos, new Vector2(i, j)) < size * size)
					WorldGen.KillWall(i, j);
	}

	public override bool ModifyOpening(ref int x, ref int y, bool isCavinator) => isCavinator;
}
