using System.Reflection;

namespace SpiritReforged;

internal static class UIHelper
{
	/// <summary> Frequently used to adjust vanilla inventory elements. Mimics the value of non-public member 'Main.mH'. </summary>
	internal static int GetMapHeight()
	{
		if (!Main.mapEnabled)
			return 0;

		return (int)typeof(Main).GetField("mH", BindingFlags.Static | BindingFlags.GetField | BindingFlags.NonPublic).GetValue(null); //Use reflection to get the value
	}
}
