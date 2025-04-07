using SpiritReforged.Common.UI.System;
using Terraria.GameContent.UI.Elements;

namespace SpiritReforged.Content.Underground.Pottery;

public class CatalogueUI : AutoUIState
{
	private UIPanel _panel;
	private CatalogueInfoBox _info;
	private CatalogueList _entryList;

	public void SetSelected(CatalogueEntry value) => _info.selected = value;

	public override void OnInitialize()
	{
		const float infoMargin = .3f;

		Width.Set(580, 0);
		Height.Set(400, 0);
		Left.Set(-Width.Pixels / 2, .5f);
		Top.Set(-Height.Pixels / 2 - 50, .5f);

		_panel = new();
		_panel.Width = Width;
		_panel.Height = Height;
		_panel.BackgroundColor = new Color(24, 35, 80) * .8f;

		_entryList = new();
		_entryList.Width.Set(Width.Pixels - Width.Pixels * infoMargin, 0);
		_entryList.Height.Set(Height.Pixels - 6, 0);
		_entryList.Top.Set(3, 0);
		_entryList.AddScrollbar(new UIScrollbar());

		_info = new();
		_info.Width.Set(Width.Pixels * infoMargin, 0);
		_info.Height = Height;
		_info.Left.Set(Width.Pixels - _info.Width.Pixels, 0);

		Append(_panel);
		Append(_entryList);
		Append(_info);
	}

	public override void OnActivate()
	{
		_entryList.ClearEntries();

		foreach (var value in RecordHandler.Records)
		{
			bool locked = !Main.LocalPlayer.GetModPlayer<RecordPlayer>().IsValidated(value.key);
			_entryList.AddEntry(new(value, locked));
		}
	}

	public override void Update(GameTime gameTime)
	{
		base.Update(gameTime);

		if (ContainsPoint(Main.MouseScreen))
			Main.LocalPlayer.mouseInterface = true;

		if (!Main.playerInventory)
			UISystem.SetInactive<CatalogueUI>();
	}
}