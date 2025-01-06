namespace SpiritReforged.Content.Savanna.Ecotone;

/// <summary> Counteracts auto water conversion in the "Final Cleanup" Genpass. </summary>
internal class StopLava : ModSystem
{
	private static readonly HashSet<Rectangle> areas = [];

	public static bool AddArea(Rectangle area) => areas.Add(area);

	public override void PostWorldGen()
	{
		foreach (var a in areas)
			StopLavaInArea(a);

		areas.Clear();
	}

	private static void StopLavaInArea(Rectangle area)
	{
		for (int i = area.X; i < area.X + area.Width; i++)
		{
			for (int j = area.Y; j < area.Y + area.Height; j++)
			{
				var tile = Main.tile[i, j];

				if (tile.LiquidType == LiquidID.Lava)
					tile.LiquidType = LiquidID.Water;
			}
		}
	}
}
