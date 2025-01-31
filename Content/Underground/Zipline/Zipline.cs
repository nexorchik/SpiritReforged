using SpiritReforged.Common.PlayerCommon;

namespace SpiritReforged.Content.Underground.Zipline;

internal class Zipline(int owner)
{
	public readonly List<Vector2> points = [];
	private readonly int owner = owner;

	public Player Owner => Main.player[owner];
	public bool DrawingLine => points.Count == 2;

	public bool Contains(Point point, out Vector2 contained)
	{
		const int size = 16;

		foreach (var p in points)
		{
			if (new Rectangle((int)(p.X - size / 2), (int)(p.Y - size / 2), size, size).Contains(point))
			{
				contained = p;
				return true;
			}
		}

		contained = Vector2.Zero;
		return false;
	}

	/// <summary> Removes <paramref name="point"/> from this zipline and destroys it if no points remain. </summary>
	/// <param name="point"> The point to remove. </param>
	public void RemovePoint(Vector2 point)
	{
		points.Remove(point);

		if (points.Count == 0)
			ZiplineHandler.ziplines.Remove(this);
	}

	public void Draw(SpriteBatch spriteBatch)
	{
		var texture = ZiplineHandler.hookTexture.Value;

		for (int i = 0; i < points.Count; i++)
		{
			var p = points[i];

			if (i + 1 < points.Count)
				DrawLine(spriteBatch, p, points[i + 1]);

			var lightColor = Lighting.GetColor(p.ToTileCoordinates());
			spriteBatch.Draw(texture, p - Main.screenPosition, null, lightColor, 0, texture.Size() / 2, 1, default, 0);
		}

		static void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end)
		{
			var texture = ZiplineHandler.wireTexture.Value;
			int length = (int)(start.Distance(end) / texture.Width) + 2;

			for (int i = 0; i < length; i++)
			{
				var position = Vector2.Lerp(start, end, i / (length - 1f));
				float rotation = start.AngleTo(end);
				var lightColor = Lighting.GetColor(position.ToTileCoordinates());

				spriteBatch.Draw(texture, position - Main.screenPosition, null, lightColor, rotation, texture.Size() / 2, 1, default, 0);
			}
		}
	}

	public bool OnZipline(Player player)
	{
		const int width = 6;
		if (!DrawingLine)
			return false;

		GetRange(out var start, out var end);

		float collisionPoint = 0;
		var lowRect = new Rectangle((int)player.position.X, (int)player.position.Y + player.height / 2, player.width, player.height / 2);

		if (player.velocity.Y >= 0 && Collision.CheckAABBvLineCollision(lowRect.TopLeft(), lowRect.Size(), start, end, width, ref collisionPoint) && !player.FallThrough())
		{
			var delta = Vector2.Lerp(start, end, collisionPoint / start.Distance(end));
			float angle = start.AngleTo(end);

			if (Math.Abs(angle) > .5f)
				return false;

			UpdatePlayer(player, delta, angle);

			return true;
		}

		return false;
	}

	private static void UpdatePlayer(Player player, Vector2 delta, float rotation)
	{
		player.position = new Vector2(player.position.X, delta.Y - player.height);

		player.velocity.Y = 0;
		player.gfxOffY = rotation * 10;

		player.fullRotation = rotation;
		player.fullRotationOrigin = new Vector2(player.width / 2, player.height + 8);
	}

	private void GetRange(out Vector2 start, out Vector2 end)
	{
		if (points[0].X < points[1].X)
		{
			start = points[0];
			end = points[1];
		}
		else
		{
			start = points[1];
			end = points[0];
		}
	}
}
