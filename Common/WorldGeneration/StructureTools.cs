using Terraria.DataStructures;

namespace SpiritReforged.Common.WorldGeneration;

/// <summary>
/// Contains a few helper tools to use <see cref="StructureHelper.Generator"/>.
/// </summary>
internal static class StructureTools
{
	/// <summary>
	/// Places a structure according to an origin. An <paramref name="origin"/> of (0.5f, 0.5f) would be centered, 
	/// (0.5f, 1) would be centered on the X axis, and place from the bottom on the Y axis, and so on.
	/// </summary>
	/// <param name="structure">The path to the structure to place.</param>
	/// <param name="position">Position of the structure.</param>
	/// <param name="origin">Placement origin of the structure.</param>
	/// <param name="mod">Mod to associate the structure with. Defaults to the Spirit Reforged instance.</param>
	/// <param name="cullAbove">Whether to cull tiles directly above the spawned structure. This can be useful when spawning over trees or lare multitiles.</param>
	public static void PlaceByOrigin(string structure, Point16 position, Vector2 origin, Mod mod = null, bool cullAbove = false)
	{
		mod ??= ModContent.GetInstance<SpiritReforgedMod>();
		var dims = new Point16();
		StructureHelper.Generator.GetDimensions(structure, mod, ref dims);
		position = (position.ToVector2() - dims.ToVector2() * origin).ToPoint16();

		if (cullAbove)
			CullLine(position, dims);

		StructureHelper.Generator.GenerateStructure(structure, position, mod);
	}

	private static void CullLine(Point16 position, Point16 dims)
	{
		for (int i = position.X; i < position.X + dims.X; ++i)
			WorldGen.KillTile(i, position.Y - 1);
	}
}
