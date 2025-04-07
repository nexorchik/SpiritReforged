using SpiritReforged.Common.UI.System;
using Terraria.UI;

namespace SpiritReforged.Content.Underground.Pottery;

public class CatalogueEntry : UIElement
{
	private static Asset<Texture2D> Front;
	private static Asset<Texture2D> Back;
	private static Asset<Texture2D> Selection;
	private static Asset<Texture2D> Locked;

	public readonly TileRecord record;
	public readonly bool locked;

	public CatalogueEntry(TileRecord record, bool locked = true)
	{
		this.record = record;
		this.locked = locked;

		const string common = "Images/UI/Bestiary/";

		Front = Main.Assets.Request<Texture2D>(common + "Slot_Front");
		Back = Main.Assets.Request<Texture2D>(common + "Slot_Back");
		Selection = Main.Assets.Request<Texture2D>(common + "Slot_Selection");
		Locked = Main.Assets.Request<Texture2D>(common + "Icon_Locked");

		Width.Pixels = Back.Width();
		Height.Pixels = Back.Height();
	}

	protected override void DrawSelf(SpriteBatch spriteBatch)
	{
		const float scale = 1;

		base.DrawSelf(spriteBatch);

		var center = GetDimensions().Center().ToPoint().ToVector2(); //Round to avoid tearing
		var backColor = IsMouseHovering ? Color.LightGray : Color.White;

		spriteBatch.Draw(Back.Value, center, null, backColor, 0, Back.Size() / 2, scale, default, 0);
		DrawTile(spriteBatch, center, scale);
		spriteBatch.Draw(Front.Value, center, null, Color.White, 0, Front.Size() / 2, scale, default, 0);

		if (IsMouseHovering)
		{
			spriteBatch.Draw(Selection.Value, center, null, Color.White, 0, Front.Size() / 2, scale, default, 0);
			UISystem.GetState<CatalogueUI>().SetSelected(this);
		}
	}

	private void DrawTile(SpriteBatch spriteBatch, Vector2 position, float scale)
	{
		var color = Color.White * (IsMouseHovering ? 1f : 0.3f);

		if (locked)
		{
			spriteBatch.Draw(Locked.Value, position, null, color * 0.15f, 0, Locked.Size() / 2, scale, default, 0);
			return;
		}

		record.DrawIcon(spriteBatch, position, color, scale);
	}
}