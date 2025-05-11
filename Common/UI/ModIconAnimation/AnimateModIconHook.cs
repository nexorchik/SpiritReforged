using MonoMod.RuntimeDetour;
using SpiritReforged.Common.UI.Misc;
using System.Reflection;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace SpiritReforged.Common.UI.ModIconAnimation;

internal class AnimateModIconHook : ILoadable
{
	private static Hook ModUIInitHook = null;
	private static FieldInfo IconInfo;
	private static PropertyInfo ModNameInfo;
	private static FieldInfo NameUIInfo;

	public void Load(Mod mod)
	{
		var type = typeof(Mod).Assembly.GetType("Terraria.ModLoader.UI.UIModItem");
		MethodInfo info = type.GetMethod("OnInitialize");
		ModUIInitHook = new Hook(info, HookModIcon, true);

		IconInfo = type.GetField("_modIcon", BindingFlags.NonPublic | BindingFlags.Instance);
		NameUIInfo = type.GetField("_modName", BindingFlags.NonPublic | BindingFlags.Instance);
		ModNameInfo = type.GetProperty("ModName", BindingFlags.Public | BindingFlags.Instance);
	}

	public static void HookModIcon(Action<object> orig, object self)
	{
		orig(self);

		string name = ModNameInfo.GetValue(self) as string;

		if (name == "SpiritReforged")
		{
			var icon = IconInfo.GetValue(self) as UIImage;

			var element = self as UIElement;
			element.RemoveChild(icon);

			var tex = SpiritReforgedMod.Instance.Assets.Request<Texture2D>("icon_animated", AssetRequestMode.ImmediateLoad);
			var scroll = SpiritReforgedMod.Instance.Assets.Request<Texture2D>("icon_scroll", AssetRequestMode.ImmediateLoad);

			if (DateTime.Now.Month == 4 && DateTime.Now.Day == 1)
			{
				tex = SpiritReforgedMod.Instance.Assets.Request<Texture2D>("Assets/Textures/AprilFools/FablesReforgedIcon", AssetRequestMode.ImmediateLoad);
				scroll = SpiritReforgedMod.Instance.Assets.Request<Texture2D>("Assets/Textures/AprilFools/FablesReforgedScroll2", AssetRequestMode.ImmediateLoad);
			}

			element.Append(new UIScrollingImage(tex, scroll, 0.3f));

			if (UIMenuThemeButton.CanExist()) //Add the menu theme button
			{
				var menuButton = new UIMenuThemeButton(SpiritReforgedMod.Instance.Assets.Request<Texture2D>("icon_small"));
				menuButton.Left.Set(426, 0);
				menuButton.Top.Set(42, 0);

				element.Append(menuButton);
			}

			var nameUI = NameUIInfo.GetValue(self) as UIText;
			nameUI.TextColor = new Color(255, 199, 130);

			if (DateTime.Now.Month == 4 && DateTime.Now.Day == 1)
			{
				nameUI.SetText("Fables Reforged v0.1");
				nameUI.TextColor = new Color(255, 215, 148);
			}
		}
	}

	public void Unload()
	{
		ModUIInitHook.Undo();
		ModUIInitHook = null;
		IconInfo = null;
		ModNameInfo = null;
	}
}
