using Terraria.IO;
using Terraria.WorldBuilding;

namespace SpiritReforged.Common.WorldGeneration.Micropasses;

public abstract class Micropass : ILoadable
{
	public abstract string WorldGenName { get; }

	public abstract void Run(GenerationProgress progress, GameConfiguration config);
	public abstract int GetWorldGenIndexInsert(List<GenPass> tasks, ref bool afterIndex);

	public virtual void Load(Mod mod) { }
	public virtual void Unload() { }
}
