using SpiritReforged.Common.Easing;

namespace SpiritReforged.Common.PrimitiveRendering.PrimitiveShape;

/// <summary>
/// Draws a strip of rectangles in the pattern of an arc, given an input center position, starting and ending distances from the position, and the angle range
/// </summary>
public class PrimitiveSlashArc : IPrimitiveShape
{
	public PrimitiveType GetPrimitiveType => PrimitiveType.TriangleStrip;
	public Vector2 BasePosition { get; set; }
	public Vector2 DirectionUnit { get; set; }
	public float MinDistance { get; set; } = 0;
	public float MaxDistance { get; set; } = -1;
	public EaseFunction DistanceEase { get; set; } = EaseFunction.Linear;
	public float Width { get; set; }
	public Vector2 AngleRange { get; set; }
	public Color Color { get; set; }
	public float SlashProgress { get; set; }
	public int RectangleCount { get; set; } = 20;

	public void PrimitiveStructure(out VertexPositionColorTexture[] vertices, out short[] indeces)
	{
		var vertexList = new List<VertexPositionColorTexture>();
		var indexList = new List<short>();

		if (MaxDistance == -1)
			MaxDistance = MinDistance;

		//Cut down a bit on boilerplate by adding a method
		void AddVertexIndex(Vector2 position, Vector2 TextureCoords)
		{
			indexList.Add((short)vertexList.Count);
			vertexList.Add(new VertexPositionColorTexture(new Vector3(position, 0), Color, TextureCoords));
		}

		for (int i = 0; i <= RectangleCount; i++)
		{
			float progress = i / (float)RectangleCount;
			progress *= SlashProgress;

			float angle = MathHelper.Lerp(AngleRange.X, AngleRange.Y, progress);
			float distance = MathHelper.Lerp(MinDistance, MaxDistance, DistanceEase.Ease(EaseFunction.EaseSine.Ease(progress)));

			Vector2 minDistPoint = BasePosition + DirectionUnit.RotatedBy(angle) * (distance - Width / 2);
			Vector2 maxDistPoint = BasePosition + DirectionUnit.RotatedBy(angle) * (distance + Width / 2);

			AddVertexIndex(maxDistPoint, new Vector2(progress, 1));
			AddVertexIndex(minDistPoint, new Vector2(progress, 0));
		}

		vertices = [.. vertexList];
		indeces = [.. indexList];
	}
}
