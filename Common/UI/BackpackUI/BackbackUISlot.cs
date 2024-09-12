using SpiritReforged.Common.ItemCommon.Backpacks;
using System.Linq;
using Terraria.UI;

namespace SpiritReforged.Common.UI.BackpackUI;

public class BackpackUISlot : UIElement
{
	private Item[] _itemArray;
	private int _itemIndex;
	private int _itemSlotContext;

	public BackpackUISlot(Item[] itemArray, int itemIndex, int itemSlotContext)
	{
		_itemArray = itemArray;
		_itemIndex = itemIndex;
		_itemSlotContext = itemSlotContext;
		Width = new StyleDimension(48f, 0f);
		Height = new StyleDimension(48f, 0f);
	}

	private void HandleItemSlotLogic()
	{
		Item inv = _itemArray[_itemIndex];

		if (IsMouseHovering && CanClickItem(inv))
		{
			Main.LocalPlayer.mouseInterface = true;
			ItemSlot.OverrideHover(ref inv, _itemSlotContext);
			ItemSlot.LeftClick(ref inv, _itemSlotContext);
			ItemSlot.RightClick(ref inv, _itemSlotContext);
			ItemSlot.MouseHover(ref inv, _itemSlotContext);
			_itemArray[_itemIndex] = inv;
			Main.LocalPlayer.GetModPlayer<BackpackPlayer>().Backpack = !inv.IsAir ? inv : null;
		}
	}

	private static bool CanClickItem(Item currentItem)
	{
		if (!currentItem.IsAir && currentItem.ModItem is BackpackItem backpack && backpack.Items.Any(x => !x.IsAir))
			return false;

		Player plr = Main.LocalPlayer;
		return plr.HeldItem.ModItem is BackpackItem || plr.HeldItem.IsAir || Main.mouseItem.IsAir;
	}

	protected override void DrawSelf(SpriteBatch spriteBatch)
	{
		if (Main.EquipPage != 2)
			return;

		HandleItemSlotLogic();
		Item inv = _itemArray[_itemIndex];
		float oldScale = Main.inventoryScale;
		Main.inventoryScale = 0.85f;
		Vector2 position = GetDimensions().Center() + new Vector2(52f, 52f) * -0.5f * Main.inventoryScale;
		ItemSlot.Draw(spriteBatch, ref inv, _itemSlotContext, position);
		Main.inventoryScale = oldScale;
	}
}

