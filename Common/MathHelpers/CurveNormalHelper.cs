using SpiritReforged.Common.Misc;

namespace SpiritReforged.Common.MathHelpers;

public static class CurveNormalHelper
{
	public static Vector2 CurveNormal(List<Vector2> points, int index)
	{
		if (points.Count == 1)
			return points[0];

		if (index == 0)
			return Vector2.Normalize(points[1] - points[0]).TurnRight();

		if (index == points.Count - 1)
			return Vector2.Normalize(points[index] - points[index - 1]).TurnRight();

		return Vector2.Normalize(points[index] - points[index - 1]).TurnRight();
	}
}
