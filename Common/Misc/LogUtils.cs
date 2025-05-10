using log4net;
using SpiritReforged.Common.UI.ErrorLog;

namespace SpiritReforged.Common.Misc;

internal class LogUtils
{
	public static ILog Logger => SpiritReforgedMod.Instance.Logger;

	internal static readonly List<string> Logs = [];

	/// <summary> Automatically formats IL logs using <paramref name="title"/> and <paramref name="message"/> and stores them in <see cref="Logs"/>. </summary>
	/// <param name="title"> The name of this edit. </param>
	/// <param name="message"> The message to log. </param>
	public static void LogIL(string title, string message)
	{
		Logger.Warn($"IL edit '{title}' failed! " + message);
		Logs.Add(title + " failed to load! " + message);

		MenuErrorPopup.CreatePopup();
	}
}