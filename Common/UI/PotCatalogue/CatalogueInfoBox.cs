using Terraria.UI;

namespace SpiritReforged.Content.Underground.Pottery;

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
		string name = selected.record.name;
		var namePos = bounds.Top() + new Vector2(0, 20);
		Utils.DrawBorderString(spriteBatch, name, namePos, Main.MouseTextColorReal, .9f, .5f, .5f);

		source = new Rectangle(0, 0, (int)Width.Pixels - 8, 4);
		spriteBatch.Draw(Divider.Value, namePos + new Vector2(0, FontAssets.MouseText.Value.MeasureString(name).Y / 2), source, Color.White, 0, source.Size() / 2, 1, default, 0);

		//Draw star rating
		const int count = 5;
		int space = StarDim.Width() + 2;

		for (int i = 0; i < count; i++)
		{
			var texture = ((i < entry.record.rating) ? StarLight : StarDim).Value;

			var position = bounds.Top() + new Vector2(space * i - space * 2, 50);
			spriteBatch.Draw(texture, position, null, Color.White, 0, texture.Size() / 2, 1, default, 0);
		}

		//Draw description
		string[] wrappingText = Utils.WordwrapString(selected.record.description, FontAssets.MouseText.Value, bounds.Width, 20, out _);

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