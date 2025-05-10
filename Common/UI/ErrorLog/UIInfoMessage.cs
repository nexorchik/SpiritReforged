using SpiritReforged.Common.Misc;
using System.IO;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using Terraria.UI;
using Terraria.UI.Gamepad;

namespace SpiritReforged.Common.UI.ErrorLog;

internal class UIInfoMessage : UIState, IHaveBackButtonCommand
{
	public static UIInfoMessage Instance = new();
	public UIState PreviousUIState { get; set; }

	private UIPanel _panel;
	private UIList _list;
	private UITextPanel<string> _infoPanel;
	private UITextPanel<string> _back;
	private UITextPanel<string> _logs;

	private UIState _returnState;
	private int _returnMode;

	public override void OnInitialize()
	{
		Width = Height = StyleDimension.Fill;

		_panel = new();
		_panel.Left.Set(0, 0.25f);
		_panel.Top.Set(0, 0.25f);
		_panel.Width.Set(0, 0.5f);
		_panel.Height.Set(0, 0.5f);
		Append(_panel);

		_infoPanel = new("Errors may have resulted from conflicts with other mods, Terraria or tModLoader updates.");
		_infoPanel.VAlign = 1;
		_infoPanel.Width.Set(0, 1f);
		_infoPanel.Height.Set(0, 0.15f);
		_panel.Append(_infoPanel);

		_list = [];
		_list.Width.Set(0, 1);
		_list.Height.Set(0, 0.85f);
		_list.SetScrollbar(new UIScrollbar());
		_panel.Append(_list);

		_back = new UITextPanel<string>(Language.GetTextValue("tModLoader.Exit"), 0.7f, large: true);
		_back.VAlign = 0.75f;
		_back.Left.Set(0, 0.25f);
		_back.Top.Set(40, 0);
		_back.Width.Set(0, _panel.Width.Percent * 0.25f);
		_back.Height.Set(50, 0);
		_back.WithFadedMouseOver();
		_back.OnLeftClick += (UIMouseEvent evt, UIElement listeningElement) => HandleBackButtonUsage();
		Append(_back);

		_logs = new UITextPanel<string>(Language.GetTextValue("tModLoader.Logs"), 0.7f, large: true);
		_logs.VAlign = 0.75f;
		_logs.Left.Set(_back.GetDimensions().Width + 4, 0.25f);
		_logs.Top.Set(40, 0);
		_logs.Width.Set(0, _panel.Width.Percent * 0.25f);
		_logs.Height.Set(50, 0);
		_logs.WithFadedMouseOver();
		_logs.OnLeftClick += (UIMouseEvent evt, UIElement listeningElement) => OpenLogsFolder();
		Append(_logs);
	}

	public override void OnActivate()
	{
		_list.Clear();

		foreach (string log in LogUtils.Logs)
			_list.Add(new SingleLog(log));
	}

	internal void ShowMessage()
	{
		if (!Program.IsMainThread)
		{
			Main.QueueMainThreadAction(ShowMessage);
			return;
		}

		int returnMode = Main.menuMode;
		var returnState = Main.MenuUI.CurrentState;

		_returnMode = returnMode;
		_returnState = returnState;

		Activate();
		Main.MenuUI.SetState(this);
	}

	public void HandleBackButtonUsage()
	{
		SoundEngine.PlaySound(in SoundID.MenuOpen);
		Main.menuMode = _returnMode;

		if (_returnState != null)
			Main.MenuUI.SetState(_returnState);
	}

	private static void OpenLogsFolder()
	{
		if (!Directory.Exists(Logging.LogDir))
			return;

		Utils.OpenFolder(Logging.LogDir);
	}

	protected override void DrawSelf(SpriteBatch spriteBatch)
	{
		base.DrawSelf(spriteBatch);
		UILinkPointNavigator.Shortcuts.BackButtonCommand = 7;
	}
}