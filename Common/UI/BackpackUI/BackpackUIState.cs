using SpiritReforged.Common.ItemCommon.Backpacks;
using Steamworks;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace SpiritReforged.Common.UI.BackpackUI;

internal class BackpackUIState : UIState
{
	public static Item AirItem
	{
		get
		{
			var item = new Item();
			item.TurnToAir();
			return item;
		}
	}

	BackpackUISlot SlotItem;

	internal Item HeldBackpack => _backSlot[0];
	internal BackpackItem HeldModBackpack => _backSlot[0].ModItem as BackpackItem;
	internal bool HasBackpack => HeldBackpack.ModItem is BackpackItem;

	private readonly Item[] _backSlot = [AirItem];

	private bool _lastHasBackpack = false;
	private bool _setBackpack = false;

	public void SetBackpack(Item item) => _backSlot[0] = item;

	public override void Update(GameTime gameTime)
	{
		if (!Main.playerInventory)
			return;

		if (!_setBackpack && Main.LocalPlayer is not null)
		{
			Item backpack = Main.LocalPlayer.GetModPlayer<BackpackPlayer>().Backpack;

			if (backpack is not null)
				SetBackpack(backpack);

			_setBackpack = true;
		}

		base.Update(gameTime);

		if (!_lastHasBackpack && HasBackpack)
		{
			const int BaseX = 570;

			int xOff = 0;
			int yOff = 0;

			Append(new UIText("Pack", 0.725f, false)
			{
				Left = new StyleDimension(BaseX, 0),
				Top = new StyleDimension(86, 0),
				Width = StyleDimension.FromPixels(32),
				Height = StyleDimension.FromPixels(32),
				TextColor = Color.White * 0.95f,
				ShadowColor = Color.Transparent
			});

			for (int i = 0; i < HeldModBackpack.Items.Length; ++i)
			{
				var newSlot = new UIItemSlot(HeldModBackpack.Items, i, ItemSlot.Context.ChestItem)
				{
					Left = new StyleDimension(BaseX + xOff * 32, 0),
					Top = new StyleDimension(105 + yOff * 33, 0),
					Width = StyleDimension.FromPixels(32),
					Height = StyleDimension.FromPixels(32)
				};

				yOff++;

				if (yOff > 4)
				{
					xOff++;
					yOff = 0;
				}

				Append(newSlot);
			}
		}

		if (_lastHasBackpack && !HasBackpack)
		{
			List<UIElement> removals = [];

			foreach (var item in Children)
				if (item is UIItemSlot or UIText)
					removals.Add(item);

			foreach (var item in removals)
				RemoveChild(item);
		}

		_lastHasBackpack = HasBackpack;
	}

	public override void OnInitialize()
	{
		Width = StyleDimension.Fill;
		Height = StyleDimension.Fill;

		SlotItem = new BackpackUISlot(_backSlot, 0, ItemSlot.Context.ChestItem)
		{
			Left = new StyleDimension(-230, 1),
			Top = new StyleDimension(-174, 1),
			Width = StyleDimension.FromPixels(32),
			Height = StyleDimension.FromPixels(32)
		};

		Append(SlotItem);
	}

	protected override void DrawChildren(SpriteBatch spriteBatch)
	{
		float lastScale = Main.inventoryScale;
		Main.inventoryScale = 0.6f;
		base.DrawChildren(spriteBatch);
		Main.inventoryScale = lastScale;
	}
}
