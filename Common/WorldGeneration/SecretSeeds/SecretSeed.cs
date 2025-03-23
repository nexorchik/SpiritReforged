using Terraria.IO;

namespace SpiritReforged.Common.WorldGeneration.Seeds;

public abstract class SecretSeed : ILoadable, IModType
{
	public Mod Mod { get; internal set; }
	public string FullName => $"{Mod?.Name ?? "Terraria"}/{Name}";
	public virtual string Name => GetType().Name;

	/// <summary> The name/key of this custom seed for input. <b>Not</b> for saving. </summary>
	public abstract string Key { get; }
	/// <summary> The path of the icon to display on worlds generated with this seed. </summary>
	public virtual string Icon => (GetType().Namespace + $".{GetType().Name}_Icon").Replace('.', '/');

	public virtual Asset<Texture2D> GetIcon(WorldFileData data)
		=> ModContent.Request<Texture2D>(Icon + (data.IsHardMode ? "Hallow" : string.Empty) + (data.HasCorruption ? "Corruption" : "Crimson"), AssetRequestMode.ImmediateLoad);

	public void Load(Mod mod)
	{
		Mod = mod;
		SecretSeedSystem.RegisterSeed(this);
	}

	public void Unload() { }
}