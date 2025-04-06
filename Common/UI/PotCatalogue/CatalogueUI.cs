using SpiritReforged.Common.UI.System;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace SpiritReforged.Content.Underground.Pottery;

public class CatalogueUI : AutoUIState
{
	private UIPanel _panel;
	private CatalogueInfoBox _info;

	public void SetSelected(CatalogueEntry value) => _info.selected = value;

	public override void OnInitialize()
	{
		Width.Set(500, 0);
		Height.Set(400, 0);
		Left.Set(-Width.Pixels / 2, .5f);
		Top.Set(-Height.Pixels / 2 - 50, .5f);

		_panel = new();
		_panel.Width = Width;
		_panel.Height = Height;
		_panel.BackgroundColor = new Color(24, 35, 80) * .8f;

		_info = new();
		_info.Width.Set(110, 0);
		_info.Height = Height;
		_info.Left.Set(Width.Pixels - _info.Width.Pixels, 0);

		Append(_panel);
		Append(_info);
	}

	public override void OnActivate()
	{
		foreach (var value in CatalogueHandler.Records)
		{
			bool locked = !Main.LocalPlayer.GetModPlayer<RecordPlayer>().IsValidated(value.key);
			AddEntry(new(value, locked));
		}
	}

	public override void OnDeactivate() => ClearEntries();

	public void AddEntry(CatalogueEntry e)
	{
		Append(e);
		RecalculateEntries();
	}

	public void ClearEntries()
	{
		HashSet<CatalogueEntry> queued = [];
		foreach (var e in Children)
		{
			if (e is CatalogueEntry cata)
				queued.Add(cata);
		}

		foreach (var cata in queued)
			RemoveChild(cata);
	}

	private void RecalculateEntries()
	{
		const int padding = 6;

		int count = 0;
		float panelWidth = GetDimensions().Width - _info?.Width.Pixels ?? 0;

		foreach (var e in Children)
		{
			if (e is not CatalogueEntry)
				continue;

			int fullWidth = (int)(e.Width.Pixels + padding);
			int fullHeight = (int)(e.Width.Pixels + padding);

			int space = (int)((float)panelWidth / fullWidth);

			float x = padding + fullWidth * (count % space);
			float y = padding + fullHeight * (count / space);

			e.Left.Pixels = x;
			e.Top.Pixels = y;

			count++;
		}
	}

	public override void Recalculate()
	{
		RecalculateEntries();
		base.Recalculate();
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

public class CatalogueInfoBox : UIElement
{
	private static Asset<Texture2D> Divider;
	private static Asset<Texture2D> StarDim;
	private static Asset<Texture2D> StarLight;

	public CatalogueEntry selected;

	public CatalogueInfoBox()
	{
		Divider = Main.Assets.Request<Texture2D>("Images/UI/Divider");

		const string common = "Images/UI/Bestiary/";

		StarDim = Main.Assets.Request<Texture2D>(common + "Icon_Rank_Dim");
		StarLight = Main.Assets.Request<Texture2D>(common + "Icon_Rank_Light");
	}

	protected override void DrawSelf(SpriteBatch spriteBatch)
	{
		base.DrawSelf(spriteBatch);

		var bounds = GetDimensions().ToRectangle();
		var source = new Rectangle(0, 0, (int)Height.Pixels - 8, 4);

		spriteBatch.Draw(Divider.Value, bounds.Left(), source, Color.White, MathHelper.PiOver2, source.Size() / 2, 1, default, 0);

		if (selected is not CatalogueEntry entry || entry.locked)
			return;

		//Draw name
		string name = selected.record.Name;
		var namePos = bounds.Top() + new Vector2(0, 20);
		Utils.DrawBorderString(spriteBatch, name, namePos, Main.MouseTextColorReal, .9f, .5f, .5f);

		source = new Rectangle(0, 0, (int)Width.Pixels - 8, 4);
		spriteBatch.Draw(Divider.Value, namePos + new Vector2(0, FontAssets.MouseText.Value.MeasureString(name).Y / 2), source, Color.White, 0, source.Size() / 2, 1, default, 0);

		//Draw star rating
		const int count = 5;
		int space = StarDim.Width() + 2;

		for (int i = 0; i < count; i++)
		{
			var texture = ((i < entry.record.Rating) ? StarLight : StarDim).Value;

			var position = bounds.Top() + new Vector2(space * i - space * 2, 50);
			spriteBatch.Draw(texture, position, null, Color.White, 0, texture.Size() / 2, 1, default, 0);
		}

		//Draw description
		string[] wrappingText = Utils.WordwrapString(selected.record.Description, FontAssets.MouseText.Value, bounds.Width, 20, out _);

		for (int i = 0; i < wrappingText.Length; i++)
		{
			string text = wrappingText[i];

			if (text is null)
				continue;

			float height = FontAssets.MouseText.Value.MeasureString(text).Y / 2;
			Utils.DrawBorderString(spriteBatch, text, bounds.Top() + new Vector2(0, 60 + height * i), Main.MouseTextColorReal, .8f, .5f, 0);
		}
	}

	public override void OnDeactivate() => selected = null;
}

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

		Width.Pixels = Height.Pixels = 70;

		const string common = "Images/UI/Bestiary/";

		Front = Main.Assets.Request<Texture2D>(common + "Slot_Front");
		Back = Main.Assets.Request<Texture2D>(common + "Slot_Back");
		Selection = Main.Assets.Request<Texture2D>(common + "Slot_Selection");
		Locked = Main.Assets.Request<Texture2D>(common + "Icon_Locked");
	}

	protected override void DrawSelf(SpriteBatch spriteBatch)
	{
		const float scale = 1;

		base.DrawSelf(spriteBatch);

		var center = GetDimensions().Center().ToPoint().ToVector2(); //Round to avoid tearing

		spriteBatch.Draw(Back.Value, center, null, Color.White, 0, Back.Size() / 2, scale, default, 0);
		DrawTile(spriteBatch, center, scale);
		spriteBatch.Draw(Front.Value, center, null, Color.White, 0, Front.Size() / 2, scale, default, 0);

		if (IsMouseHovering)
		{
			spriteBatch.Draw(Selection.Value, center, null, Color.White, 0, Front.Size() / 2, scale, default, 0);

			if (Parent is CatalogueUI cata)
				cata.SetSelected(this);
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