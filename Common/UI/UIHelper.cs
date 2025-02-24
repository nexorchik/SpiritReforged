using Terraria.GameInput;

namespace SpiritReforged;

internal static class UIHelper
{
	/// <summary> Frequently used to adjust vanilla inventory elements. Mimics the value of non-public member 'Main.mH'. </summary>
	internal static int GetMapHeight()
	{
		if (!Main.mapEnabled)
			return 0;

		int value = Main.miniMapHeight;
		if (!Main.mapFullscreen && Main.mapStyle == 1)
			value = 256;

		PlayerInput.SetZoom_UI();
		if (value + Main.instance.RecommendedEquipmentAreaPushUp > Main.screenHeight)
			value = Main.screenHeight - Main.instance.RecommendedEquipmentAreaPushUp;

		return value; //(int)typeof(Main).GetField("mH", BindingFlags.Static | BindingFlags.GetField | BindingFlags.NonPublic).GetValue(null); //Use reflection to get the value
	}
}
