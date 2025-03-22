namespace SpiritReforged.Common.WorldGeneration.Seeds;

public abstract class SecretSeed : ILoadable
{
	public static readonly Dictionary<string, Asset<Texture2D>> NameToTexture = [];

	/// <summary> The name of this custom seed used for identification. </summary>
	public abstract string Name { get; }
	/// <summary> The icon to display on worlds generated with this seed. </summary>
	public virtual string Icon => (GetType().Namespace + $".{GetType().Name}_Icon").Replace('.', '/');

	public void Load(Mod mod)
	{
		SecretSeedSystem.RegisterSeed(this);

		if (Icon != null && ModContent.RequestIfExists<Texture2D>(Icon, out var asset))
			NameToTexture.Add(Name, asset);
	}

	public void Unload() { }
}