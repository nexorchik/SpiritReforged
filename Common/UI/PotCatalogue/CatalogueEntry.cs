using SpiritReforged.Common.UI.System;
using SpiritReforged.Content.Underground.Pottery;
using Terraria.UI;

namespace SpiritReforged.Common.UI.PotCatalogue;

public class CatalogueEntry : UIElement
{
	public readonly TileRecord record;
	public readonly bool locked;

	public CatalogueEntry(TileRecord record, bool locked = true)
	{
		this.record = record;
		this.locked = locked;

		Width.Pixels = CatalogueUI.Back.Width();
		Height.Pixels = CatalogueUI.Back.Height();
	}

	protected override void DrawSelf(SpriteBatch spriteBatch)
	{
		const float scale = 1;

		base.DrawSelf(spriteBatch);

		var center = GetDimensions().Center().ToPoint().ToVector2(); //Round to avoid tearing
		var backColor = IsMouseHovering ? Color.LightGray : Color.White;

		spriteBatch.Draw(CatalogueUI.Back.Value, center, null, backColor, 0, CatalogueUI.Back.Size() / 2, scale, default, 0);
		DrawTile(spriteBatch, center, scale);
		spriteBatch.Draw(CatalogueUI.Front.Value, center, null, Color.White, 0, CatalogueUI.Front.Size() / 2, scale, default, 0);

		if (IsMouseHovering)
		{
			spriteBatch.Draw(CatalogueUI.Selection.Value, center, null, Color.White, 0, CatalogueUI.Front.Size() / 2, scale, default, 0);
			UISystem.GetState<CatalogueUI>().Select(this);
		}
	}

	private void DrawTile(SpriteBatch spriteBatch, Vector2 position, float scale)
	{
		var color = Color.White * (IsMouseHovering ? 1f : 0.3f);

		if (locked)
		{
			spriteBatch.Draw(CatalogueUI.Locked.Value, position, null, color * 0.15f, 0, CatalogueUI.Locked.Size() / 2, scale, default, 0);
			return;
		}

		record.DrawIcon(spriteBatch, position, color, scale);
	}
}