using MonoMod.RuntimeDetour;
using System.Reflection;
using System.Runtime.CompilerServices;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace SpiritReforged.Common.UI.ModIconAnimation;

internal class AnimateModIconHook : ILoadable
{
	private static Hook ModUIInitHook = null;

	public void Load(Mod mod)
	{
		var type = typeof(Mod).Assembly.GetType("Terraria.ModLoader.UI.UIModItem");
		MethodInfo info = type.GetMethod("OnInitialize");
		ModUIInitHook = new Hook(info, HookModIcon, true);
	}

	public static void HookModIcon(Action<object> orig, object self)
	{
		orig(self);

		if (GetModName(self) == "SpiritReforged")
		{
			ref UIImage icon = ref GetIcon(self);

			var element = self as UIElement;
			element.RemoveChild(icon);
			var tex = SpiritReforgedMod.Instance.Assets.Request<Texture2D>("icon_animated", AssetRequestMode.ImmediateLoad);
			element.Append(new UIAnimatedImage(tex, 80, 80, 0, 0, 1, 4, 0)
			{
				FrameCount = 4,
				TicksPerFrame = 8,
			});
		}

		[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_modIcon")]
		extern static ref UIImage GetIcon(object c);

		[UnsafeAccessor(UnsafeAccessorKind.Method, Name = "get_ModName")]
		extern static ref string GetModName(object c);
	}

	public void Unload()
	{
		ModUIInitHook.Undo();
		ModUIInitHook = null;
	}
}
