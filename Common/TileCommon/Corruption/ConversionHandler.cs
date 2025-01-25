using Mono.Cecil.Cil;
using MonoMod.Cil;
using System.Linq;

namespace SpiritReforged.Common.TileCommon.Corruption;

internal class ConversionHandler : ILoadable
{
	public void Load(Mod mod)
	{
		On_WorldGen.Convert += OnConvert;
		IL_WorldGen.GERunner += OnHardmodeEvils;
	}

	private void OnHardmodeEvils(ILContext il)
	{
		ILCursor c = new(il);

		var p_good = c.Method.Parameters.Where(x => x.Name == "good").FirstOrDefault();

		c.GotoNext(x => x.Match(OpCodes.Ldarg_S, p_good));

		c.Emit(OpCodes.Ldloc_S, (byte)15);
		c.Emit(OpCodes.Ldloc_S, (byte)16);
		c.Emit(OpCodes.Ldarg_S, p_good);
		c.EmitDelegate(RunnerConversion);
	}

	/// <summary> Add compatibility for our custom conversion types with hardmode evil gen. </summary>
	/// <param name="i"> The X tile coordinate. </param>
	/// <param name="j"> The Y tile coordinate. </param>
	/// <param name="hallow"> Whether this conversion is hallow or <see cref="WorldGen.crimson"/>. </param>
	private void RunnerConversion(int i, int j, bool hallow)
	{
		var type = hallow ? ConversionType.Hallow : WorldGen.crimson ? ConversionType.Crimson : ConversionType.Corrupt;
		TileCorruptor.Convert(null, type, i, j);
	}

	/// <summary> Add compatibility for our custom conversion types when a tile is converted via various means, like solutions or using seeds. </summary>
	/// <param name="orig"></param>
	/// <param name="i"> The X tile coordinate. </param>
	/// <param name="j"> The Y tile coordinate. </param>
	/// <param name="conversionType"> The conversion type corresponding to <see cref="BiomeConversionID"/>. </param>
	/// <param name="size"> The size (in tiles) of the conversion. </param>
	private void OnConvert(On_WorldGen.orig_Convert orig, int i, int j, int conversionType, int size)
	{
		orig(i, j, conversionType, size);

		ConversionType type = conversionType switch //Translate BiomeConversionID
		{
			BiomeConversionID.Corruption => ConversionType.Corrupt,
			BiomeConversionID.Hallow => ConversionType.Hallow,
			BiomeConversionID.Crimson => ConversionType.Crimson,
			_ => ConversionType.Purify,
		};

		for (int x = i - size; x <= i + size; x++)
			for (int y = j - size; y <= j + size; y++)
				TileCorruptor.Convert(null, type, x, y);
	}

	public void Unload() { }
}
