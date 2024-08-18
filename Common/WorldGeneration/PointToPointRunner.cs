namespace SpiritReforged.Common.WorldGeneration;

internal class PointToPointRunner
{
	public delegate void PerPointDelegate(ref Vector2 position, ref Vector2 direction);

	public static void SingleTile(Queue<Vector2> orderedPoints, PerPointDelegate dele)
	{
		Vector2 current = orderedPoints.Dequeue();
		Vector2 next = orderedPoints.Dequeue();

		while (true)
		{
			Vector2 direction = current.DirectionTo(next);
			dele(ref current, ref direction);
			Vector2 offset = direction * MathF.Min(1, current.Distance(next));
			current.X += offset.X;
			dele(ref current, ref direction);
			offset = direction * MathF.Min(1, current.Distance(next));
			current.Y += offset.Y;

			if (Vector2.Distance(current, next) < 0.1f)
			{
				if (orderedPoints.Count == 0)
					return;

				current = next;
				next = orderedPoints.Dequeue();
			}
		}
	}

	public static void Decaying(Queue<Vector2> orderedPoints, float size, float decayRate, PerPointDelegate dele)
	{
		Vector2 current = orderedPoints.Dequeue();
		Vector2 next = orderedPoints.Dequeue();

		while (true)
		{
			Vector2 direction = current.DirectionTo(next);

			for (float i = -size / 2f; i < size / 2f; ++i)
			{
				Vector2 off = current + direction.RotatedBy(MathHelper.PiOver2) * i;
				dele(ref off, ref direction);
			}

			current += direction * MathF.Min(1, current.Distance(next));

			if (Vector2.Distance(current, next) < 0.1f)
			{
				if (orderedPoints.Count == 0)
					return;

				current = next;
				next = orderedPoints.Dequeue();
			}

			size -= decayRate;

			if (size <= 0f)
				return;
		}
	}

	public static PerPointDelegate PlaceTile(int type, bool forced) => (ref Vector2 pos, ref Vector2 dir) => WorldGen.PlaceTile((int)pos.X, (int)pos.Y, type, true, forced);
	public static PerPointDelegate PlaceTileClearSlope(int type, bool forced) => (ref Vector2 pos, ref Vector2 dir) =>
	{
		WorldGen.PlaceTile((int)pos.X, (int)pos.Y, type, true, forced);
		Tile tile = Main.tile[pos.ToPoint()];
		tile.Slope = SlopeType.Solid;
	};
}
