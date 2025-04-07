using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.UI;

namespace SpiritReforged.Content.Underground.Pottery;

public class CatalogueList : UIElement
{
	private const int ListPadding = 6;
	private UIScrollbar _scrollbar;

	public CatalogueList() => OverflowHidden = true;

	public void AddScrollbar(UIScrollbar scrollbar)
	{
		_scrollbar = scrollbar;
		_scrollbar.Left.Set(Width.Pixels - _scrollbar.Width.Pixels, 0);
		_scrollbar.Height.Set(Height.Pixels - 12, 0);
		_scrollbar.Top.Set(6, 0);
		_scrollbar.SetView(1f, 2f);

		Append(_scrollbar);
	}

	public void ClearEntries()
	{
		List<UIElement> queued = [];

		foreach (var e in Children)
		{
			if (e is CatalogueEntry)
				queued.Add(e);
		}

		foreach (var e in queued)
			RemoveChild(e);
	}

	public void AddEntry(CatalogueEntry e)
	{
		Append(e);
		RecalculateEntries();
	}

	public void RecalculateEntries()
	{
		int count = 0;

		int overflow = 0;
		float scrollbarInfluence = (_scrollbar is null) ? 0 : _scrollbar.GetValue() * -94f;

		foreach (var e in Children)
		{
			if (e is not CatalogueEntry)
				continue;

			int fullWidth = (int)(e.Width.Pixels + ListPadding);
			int fullHeight = (int)(e.Height.Pixels + ListPadding);

			int space = (int)GetDimensions().Width / fullWidth;

			float x = ListPadding + fullWidth * (count % space);
			float y = ListPadding + fullHeight * (count / space);

			e.Left.Pixels = x;
			e.Top.Pixels = y + scrollbarInfluence;

			count++;
			overflow = fullHeight / (int)GetDimensions().Height;
		}

		_scrollbar?.SetView(1f, Math.Max(overflow - 1, 1f)); //Recalculate maximum scrollbar view
	}

	public override void Recalculate()
	{
		base.Recalculate();
		RecalculateEntries();
	}

	public override void ScrollWheel(UIScrollWheelEvent evt)
	{
		if (_scrollbar != null)
			_scrollbar.ViewPosition -= evt.ScrollWheelValue;
	}

	public override void Update(GameTime gameTime)
	{
		if (IsMouseHovering)
			PlayerInput.LockVanillaMouseScroll("ModLoader/UIList");
	}
}