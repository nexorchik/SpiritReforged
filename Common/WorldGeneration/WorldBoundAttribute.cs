using SpiritReforged.Common.WorldGeneration.Micropasses.Passes;
using SpiritReforged.Content.Underground.Zipline;
using System.Reflection;

namespace SpiritReforged.Common.WorldGeneration;

/// <summary> Resets this static field to its original value when <see cref="WorldGen.clearWorld()"/> is called. </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
internal class WorldBoundAttribute : Attribute { }

internal class WorldBoundSystem : ModSystem
{
	private static readonly Dictionary<FieldInfo, object> Defaults = [];

	public override void Load()
	{
		foreach (var type in Mod.Code.GetTypes())
		{
			foreach (var field in type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
			{
				if (field.GetCustomAttribute<WorldBoundAttribute>() != null)
					Defaults.Add(field, field.GetValue(null));
			}
		}
	}

	public override void ClearWorld()
	{
		foreach (var info in Defaults.Keys)
			info.SetValue(null, Defaults[info]);

		FishingAreaMicropass.Coves.Clear(); //TEMPORARY! WorldBound preserves IEnumerables
		ZiplineHandler.Ziplines.Clear();
	}
}
