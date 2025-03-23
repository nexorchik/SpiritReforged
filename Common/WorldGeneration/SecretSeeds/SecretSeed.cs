using Terraria.IO;

namespace SpiritReforged.Common.WorldGeneration.Seeds;

public abstract class SecretSeed : ILoadable
{
	/// <summary> The name of this custom seed used for identification. </summary>
	public abstract string Name { get; }
	/// <summary> The path of the icon to display on worlds generated with this seed. </summary>
	public virtual string Icon => (GetType().Namespace + $".{GetType().Name}_Icon").Replace('.', '/');

	public virtual Asset<Texture2D> GetIcon(WorldFileData data)
		=> ModContent.Request<Texture2D>(Icon + (data.IsHardMode ? "Hallow" : string.Empty) + (data.HasCorruption ? "Corruption" : "Crimson"), AssetRequestMode.ImmediateLoad);

	public void Load(Mod mod) => SecretSeedSystem.RegisterSeed(this);
	public void Unload() { }
}