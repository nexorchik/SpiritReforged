using SpiritReforged.Common.ItemCommon.Backpacks;
using Steamworks;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace SpiritReforged.Common.UI.BackpackInterface;

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

	private BackpackUISlot _slotItem;
	private BackpackUISlot _vanityItem;

	internal Item HeldBackpack => _backSlots[0];
	internal BackpackItem HeldModBackpack => _backSlots[0].ModItem as BackpackItem;
	internal Item VanityBackpack => _backSlots[1];
	internal BackpackItem VanityModBackpack => _backSlots[1].ModItem as BackpackItem;
	internal bool HasBackpack => HeldBackpack.ModItem is BackpackItem;
	internal bool HasVanityBackpack => VanityBackpack.ModItem is BackpackItem;

	private readonly Item[] _backSlots = [AirItem, AirItem];

	private bool _lastHasBackpack = false;
	private bool _setBackpack = false;
	private bool _lastVanityBackpack = false;

	public void SetBackpack(Item item) => _backSlots[0] = item;
	public void SetVanityBackpack(Item item) => _backSlots[1] = item;

	public override void Update(GameTime gameTime)
	{
		if (!Main.playerInventory)
			return;

		if (!_setBackpack && Main.LocalPlayer is not null)
		{
			Item backpack = Main.LocalPlayer.GetModPlayer<BackpackPlayer>().Backpack;

			if (backpack is not null)
				SetBackpack(backpack);

			Item vanity = Main.LocalPlayer.GetModPlayer<BackpackPlayer>().VanityBackpack;

			if (vanity is not null)
				SetBackpack(vanity);

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

				if (yOff >= 4)
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

		_slotItem = new BackpackUISlot(_backSlots, 0, false)
		{
			Left = new StyleDimension(-180, 1),
			Top = new StyleDimension(437, 0),
			Width = StyleDimension.FromPixels(32),
			Height = StyleDimension.FromPixels(32)
		};

		Append(_slotItem);

		_vanityItem = new BackpackUISlot(_backSlots, 1, true)
		{
			Left = new StyleDimension(-228, 1),
			Top = new StyleDimension(437, 0),
			Width = StyleDimension.FromPixels(32),
			Height = StyleDimension.FromPixels(32)
		};

		Append(_vanityItem);
	}

	protected override void DrawChildren(SpriteBatch spriteBatch)
	{
		float lastScale = Main.inventoryScale;
		Main.inventoryScale = 0.6f;
		base.DrawChildren(spriteBatch);
		Main.inventoryScale = lastScale;
	}
}
