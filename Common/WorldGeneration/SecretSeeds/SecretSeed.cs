namespace SpiritReforged.Common.WorldGeneration.Seeds;

public abstract class SecretSeed : ILoadable
{
	public abstract string Name { get; }

	public void Load(Mod mod) => SecretSeedSystem.RegisterSeed(this);
	public void Unload() { }
}