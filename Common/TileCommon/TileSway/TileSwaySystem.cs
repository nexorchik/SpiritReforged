using Terraria.DataStructures;

namespace SpiritReforged.Common.TileCommon.TileSway;

public class TileSwaySystem : ModSystem
{
	public static TileSwaySystem Instance => ModContent.GetInstance<TileSwaySystem>();

	public readonly List<Point16> specialDrawPoints = new();

	public static void AddDrawPoint(Point16 point)
	{
		if (!Instance.specialDrawPoints.Contains(point))
			Instance.specialDrawPoints.Add(point);
	}

	public double TreeWindCounter { get; private set; }
	public double GrassWindCounter { get; private set; }
	public double SunflowerWindCounter { get; private set; }

	public override void PreUpdateWorld()
	{
		if (!Main.dedServ)
		{
			double num = Math.Abs(Main.WindForVisuals);
			num = Utils.GetLerpValue(0.08f, 1.2f, (float)num, clamped: true);

			TreeWindCounter += 0.0041666666666666666 + 0.0041666666666666666 * num * 2.0;
			GrassWindCounter += 0.0055555555555555558 + 0.0055555555555555558 * num * 4.0;
			SunflowerWindCounter += 0.002380952380952 + 0.0023809523809523810 * num * 5.0;
		}
	}
}
