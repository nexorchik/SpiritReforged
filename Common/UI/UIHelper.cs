using System.Reflection;

namespace SpiritReforged;

internal static class UIHelper
{
	/// <summary> Frequently used to adjust vanilla inventory elements. </summary>
	internal static int AdjustY => (int)typeof(Main).GetField("mH", BindingFlags.Static | BindingFlags.GetField | BindingFlags.NonPublic).GetValue(null);
}
