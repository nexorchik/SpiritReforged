using MonoMod.RuntimeDetour;
using System.Reflection;

namespace SpiritReforged.Common.Misc;

/// <summary> Selectively disables content from other mods using <see cref="string"/> identifiers. </summary>
internal class PreAddContent : ILoadable
{
	internal delegate bool orig_AddContent(Mod self, ILoadable instance);
	private static Hook PreAddContentHook = null;

	/// <summary> The full names of content to disable. Must be formatted as "Mod/Content". </summary>
	private static readonly HashSet<string> NameToMod = ["SpiritMod/FloatingItemWorld", "SpiritMod/HeroMemorialMicropass", 
		"SpiritMod/StargrassMicropass", "SpiritMod/ZombieGlobalNPC", "SpiritMod/BoidHost"];

	/// <summary> Hooks <see cref="Mod.AddContent"/> to control whether content from other mods can be added to the game.<br/>
	/// Must be called in the mod's constructor to ignore mod load order. </summary>
	internal static void AddContentHook(Mod mod)
	{
		MethodInfo info = mod.GetType().GetMethod("AddContent", BindingFlags.Instance | BindingFlags.Public, [typeof(ILoadable)]);
		PreAddContentHook = new Hook(info, HookAddContent, true);
	}

	private static bool HookAddContent(orig_AddContent orig, Mod self, ILoadable instance)
	{
		foreach (string str in NameToMod)
		{
			string[] splitMod = str.Split('/');
			string modName = splitMod[0];
			string contentName = splitMod[1];

			if (self.Name == modName && instance.GetType().Name == contentName)
			{
				SpiritReforgedMod.Instance.Logger.Info("Disabled " + str.Replace('/', '.'));
				return false;
			}
		}

		return orig(self, instance);
	}

	public void Load(Mod mod) { } //This is useless because it is called too late
	public void Unload()
	{
		PreAddContentHook?.Undo();
		PreAddContentHook = null;
	}
}
