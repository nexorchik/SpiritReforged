using Terraria.IO;
using Terraria.WorldBuilding;

namespace SpiritReforged.Common.WorldGeneration.Ecotones;

internal abstract class EcotoneBase : ILoadable
{
	public static readonly List<EcotoneBase> Ecotones = [];

	public void Load(Mod mod) => Ecotones.Add(this);

	public void Unload()
	{
	}

	public abstract void AddTasks(List<GenPass> tasks, List<EcotoneSurfaceMapping.EcotoneEntry> entries);
}
