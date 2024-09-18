namespace SpiritReforged.Common.Visuals.Skies;

public static class AutoloadSkyDict
{
	public static IDictionary<string, Func<Player, bool>> LoadedSkies { get; set; } = new Dictionary<string, Func<Player, bool>>();
}