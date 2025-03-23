using rail;
using SpiritReforged.Common.WorldGeneration.Micropasses.Passes;
using SpiritReforged.Content.Underground.Zipline;
using System.Collections;
using System.Reflection;

namespace SpiritReforged.Common.WorldGeneration;

/// <summary> 
/// Resets this static field to its original value when <see cref="WorldGen.clearWorld()"/> is called.<br/>
/// For <see cref="IEnumerable"/>s, this will call any Clear method (such as <see cref="HashSet{T}.Clear"/>) instead of nulling the value.
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
internal class WorldBoundAttribute : Attribute { }

internal class WorldBoundSystem : ModSystem
{
	private static readonly Dictionary<FieldInfo, object> Defaults = [];
	private static readonly Dictionary<FieldInfo, MethodInfo> ClearAction = [];

	public override void Load()
	{
		foreach (var type in Mod.Code.GetTypes())
		{
			foreach (var field in type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
			{
				if (field.GetCustomAttribute<WorldBoundAttribute>() != null)
				{
					Defaults.Add(field, field.GetValue(null));

					var clearMethod = field.FieldType.GetMethod("Clear", BindingFlags.Public | BindingFlags.Instance);

					if (clearMethod != null)
						ClearAction.Add(field, clearMethod);
				}
			}
		}
	}

	public override void ClearWorld()
	{
		foreach (var info in Defaults.Keys)
		{
			if (ClearAction.TryGetValue(info, out var clearMethod))
				clearMethod.Invoke(info.GetValue(Defaults[info]), []);
			else 
				info.SetValue(null, Defaults[info]);
		}
	}
}
