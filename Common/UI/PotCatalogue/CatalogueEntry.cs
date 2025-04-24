using SpiritReforged.Common.Misc;
using SpiritReforged.Common.UI.System;
using SpiritReforged.Content.Underground.Pottery;
using Terraria.Audio;
using Terraria.UI;

namespace SpiritReforged.Common.UI.PotCatalogue;

public class CatalogueEntry : UIElement
{
	public readonly TileRecord record;
	public readonly bool locked;
	public bool newAndShiny;

	public CatalogueEntry(TileRecord record, bool locked = true, bool newAndShiny = false)
	{
		this.record = record;
		this.locked = locked;
		this.newAndShiny = newAndShiny;

		Width.Pixels = CatalogueUI.Back.Width();
		Height.Pixels = CatalogueUI.Back.Height();
	}

	public override void MouseOver(UIMouseEvent evt)
	{
		base.MouseOver(evt);
		SoundEngine.PlaySound(SoundID.MenuTick);

		if (newAndShiny)
		{
			Main.LocalPlayer.GetModPlayer<RecordPlayer>().RemoveNew(record.name);
			newAndShiny = false;
		}
	}

	protected override void DrawSelf(SpriteBatch spriteBatch)
	{
		const float scale = 1;

		base.DrawSelf(spriteBatch);

		var center = GetDimensions().Center().ToPoint().ToVector2(); //Round to avoid tearing
		var backColor = IsMouseHovering ? Color.LightGray : Color.White;

		if (newAndShiny)
		{
			var pulseColor = Color.Lerp(Color.LightGray, Color.White, .5f + (float)Math.Sin(Main.timeForVisualEffects / 30f) / 2f);
			spriteBatch.Draw(CatalogueUI.Back.Value, center, null, pulseColor, 0, CatalogueUI.Back.Size() / 2, scale, default, 0);

			var bloom = AssetLoader.LoadedTextures["Bloom"].Value;
			spriteBatch.Draw(bloom, center, null, Color.SlateBlue.Additive() * .5f, 0, bloom.Size() / 2, .5f, default, 0);
		}
		else
		{
			spriteBatch.Draw(CatalogueUI.Back.Value, center, null, backColor, 0, CatalogueUI.Back.Size() / 2, scale, default, 0);
		}

		DrawTile(spriteBatch, center, scale);
		spriteBatch.Draw(CatalogueUI.Front.Value, center, null, Color.White, 0, CatalogueUI.Front.Size() / 2, scale, default, 0);

		if (IsMouseHovering)
		{
			spriteBatch.Draw(CatalogueUI.Selection.Value, center, null, Color.White, 0, CatalogueUI.Front.Size() / 2, scale, default, 0);

			if (Main.mouseLeft && Main.mouseLeftRelease)
				UISystem.GetState<CatalogueUI>().Select(this);
		}
	}

	private void DrawTile(SpriteBatch spriteBatch, Vector2 position, float scale)
	{
		var color = Color.White * (IsMouseHovering || newAndShiny ? 1f : 0.3f);

		if (locked)
		{
			spriteBatch.Draw(CatalogueUI.Locked.Value, position, null, color * 0.15f, 0, CatalogueUI.Locked.Size() / 2, scale, default, 0);
			return;
		}

		record.DrawIcon(spriteBatch, position, color);
	}
}