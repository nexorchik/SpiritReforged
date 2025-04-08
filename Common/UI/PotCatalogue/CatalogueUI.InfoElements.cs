using SpiritReforged.Common.UI.System;
using SpiritReforged.Content.Underground.Tiles;
using Terraria.GameContent.ItemDropRules;

namespace SpiritReforged.Common.UI.PotCatalogue;

public partial class CatalogueUI : AutoUIState
{
	#region textures
	private static Asset<Texture2D> StarDim;
	private static Asset<Texture2D> StarLight;

	private static Asset<Texture2D> Border;
	private static Asset<Texture2D> Background;

	internal static Asset<Texture2D> Divider;
	internal static Asset<Texture2D> Panel;

	internal static Asset<Texture2D> Front;
	internal static Asset<Texture2D> Back;
	internal static Asset<Texture2D> Selection;
	internal static Asset<Texture2D> Locked;

	public static void LoadAssets()
	{
		const string common = "Images/UI/Bestiary/";

		StarDim = Main.Assets.Request<Texture2D>(common + "Icon_Rank_Dim");
		StarLight = Main.Assets.Request<Texture2D>(common + "Icon_Rank_Light");

		Border = Main.Assets.Request<Texture2D>("Images/UI/PanelBorder");
		Background = Main.Assets.Request<Texture2D>("Images/UI/PanelBackground");

		Divider = Main.Assets.Request<Texture2D>("Images/UI/Divider");
		Panel = Main.Assets.Request<Texture2D>("Images/UI/Bestiary/Stat_Panel");

		Front = Main.Assets.Request<Texture2D>(common + "Slot_Front");
		Back = Main.Assets.Request<Texture2D>(common + "Slot_Back");
		Selection = Main.Assets.Request<Texture2D>(common + "Slot_Selection");
		Locked = Main.Assets.Request<Texture2D>(common + "Icon_Locked");
	}
	#endregion

	public void RecalculateInfo()
	{
		float width = _info.AvailableWidth;

		_info.ClearEntries();

		if (Selected.locked)
			return;

		//Name
		var info = new CatalogueInfo();
		info.Width.Pixels = width;
		info.Height.Set(30, 0);
		info.Action += NameInfo_Action;

		_info.AddEntry(info);

		//Description & star rating
		info = new CatalogueInfo();
		info.Width.Pixels = width;
		info.Height.Set(32 + UIHelper.GetTextHeight(Selected.record.description, (int)info.Width.Pixels), 0);
		info.Action += DescInfo_Action;

		_info.AddEntry(info);

		//Loot table for ILootTiles
		if (TileLoader.GetTile(Selected.record.type) is ILootTile loot)
		{
			var lootTable = loot.AddLoot(Selected.record.styles[0]);

			List<DropRateInfo> list = [];
			DropRateInfoChainFeed ratesInfo = new(1f);

			foreach (IItemDropRule item in lootTable.Get())
				item.ReportDroprates(list, ratesInfo);

			foreach (var rateInfo in list)
			{
				info = new CatalogueItemInfo(rateInfo);
				info.Width.Pixels = width;
				info.Height.Set(30, 0);

				_info.AddEntry(info);
			}
		}
	}

	private bool NameInfo_Action(SpriteBatch spriteBatch, Rectangle bounds)
	{
		string name = Selected.record.name;
		var namePos = bounds.Center();

		Utils.DrawBorderString(spriteBatch, name, namePos, Main.MouseTextColorReal, .9f, .5f, .5f);

		return true;
	}

	private bool DescInfo_Action(SpriteBatch spriteBatch, Rectangle bounds)
	{
		//Draw star rating
		const int count = 5;
		int space = StarDim.Width() + 2;

		for (int i = 0; i < count; i++)
		{
			var texture = ((i < Selected.record.rating) ? StarLight : StarDim).Value;

			var position = bounds.Top() + new Vector2(space * i - space * 2, 10);
			spriteBatch.Draw(texture, position, null, Color.White, 0, texture.Size() / 2, 1, default, 0);
		}

		//Draw description
		float height = 0;
		string desc = Selected.record.description;
		string[] wrappingText = Utils.WordwrapString(desc, FontAssets.MouseText.Value, bounds.Width, 20, out int numLines);

		for (int i = 0; i < wrappingText.Length; i++)
		{
			string text = wrappingText[i];

			if (text is null)
				continue;

			height = FontAssets.MouseText.Value.MeasureString(text).Y / 2;
			Utils.DrawBorderString(spriteBatch, text, bounds.Top() + new Vector2(0, 20 + height * i), Main.MouseTextColorReal, .8f, .5f, 0);
		}

		bounds.Height = 32 + (numLines + 1) * (int)height;

		return true;
	}
}