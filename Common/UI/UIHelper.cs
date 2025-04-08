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

	/// <summary> Gets the pixel height of <paramref name="text"/> wrapped by <paramref name="bounds"/>. </summary>
	public static float GetTextHeight(string text, int bounds = 100)
	{
		float height = 0;
		string[] wrappingText = Utils.WordwrapString(text, FontAssets.MouseText.Value, bounds, 50, out int numLines);

		for (int i = 0; i < wrappingText.Length; i++)
		{
			string line = wrappingText[i];

			if (line is null)
				continue;

			height = FontAssets.MouseText.Value.MeasureString(line).Y / 2;
		}

		return (numLines + 1) * height;
	}
}
