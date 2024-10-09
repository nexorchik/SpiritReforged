using static SpiritReforged.Common.Visuals.Skies.AutoloadSkyDict;

namespace SpiritReforged.Common.Visuals.Skies;

public static class AutoloadSkyDict
{
	public static IDictionary<string, Func<Player, bool>> LoadedSkies { get; set; } = new Dictionary<string, Func<Player, bool>>();
}

internal class UnloadDict : ILoadable
{
	public void Load(Mod mod) { }

	public void Unload() => LoadedSkies = new Dictionary<string, Func<Player, bool>>();
}