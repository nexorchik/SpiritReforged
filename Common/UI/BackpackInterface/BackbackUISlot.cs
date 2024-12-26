using SpiritReforged.Common.ItemCommon.Backpacks;
using System.Linq;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.UI;

namespace SpiritReforged.Common.UI.BackpackInterface;

public class BackpackUISlot : UIElement
{
	public const int context = ItemSlot.Context.ChestItem;
	public const float scale = .85f;

	private static Asset<Texture2D> icon;

	private readonly Item[] _itemArray;
	private readonly int _itemIndex;
	private readonly bool _isVanity;

	public BackpackUISlot(Item[] itemArray, int itemIndex, bool isVanity)
	{
		_itemArray = itemArray;
		_itemIndex = itemIndex;
		_isVanity = isVanity;

		Width = Height = new StyleDimension(52 * scale, 0f);
	}

	public override void OnInitialize() => icon = ModContent.Request<Texture2D>("SpiritReforged/Common/UI/BackpackInterface/BackpackIcon");

	private void HandleItemSlotLogic()
	{
		var invItem = _itemArray[_itemIndex];

		if (IsMouseHovering)
		{
			Main.LocalPlayer.mouseInterface = true;
			ItemSlot.OverrideHover(ref invItem, context);
			ItemSlot.MouseHover(ref invItem, context);
			
			if (CanClickItem(invItem) && (Main.mouseLeft && Main.mouseLeftRelease || Main.mouseRight && Main.mouseRightRelease))
			{
				ItemSlot.LeftClick(ref invItem, context);
				ItemSlot.RightClick(ref invItem, context);
				
				_itemArray[_itemIndex] = invItem;
				var mPlayer = Main.LocalPlayer.GetModPlayer<BackpackPlayer>();

				if (_isVanity)
					mPlayer.VanityBackpack = !invItem.IsAir ? invItem : null;
				else
					mPlayer.Backpack = !invItem.IsAir ? invItem : null;
			}

			if (invItem.IsAir)
			{
				Main.mouseText = true;
				Main.hoverItemName = Language.GetTextValue("Mods.SpiritReforged.SlotContexts.Backpack");
			}
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

		base.DrawSelf(spriteBatch);

		Item inv = _itemArray[_itemIndex];
		float oldScale = Main.inventoryScale;
		Main.inventoryScale = scale;

		ItemSlot.Draw(spriteBatch, ref inv, context, GetDimensions().ToRectangle().TopLeft());

		if (inv.IsAir) //Draw slot icons when empty
		{
			Texture2D texture;
			Rectangle source;

			if (_isVanity)
			{
				texture = TextureAssets.Extra[54].Value;
				source = texture.Frame(3, 6, 2, 0, -2, -2);
			}
			else
			{
				texture = icon.Value;
				source = texture.Frame();
			}

			spriteBatch.Draw(texture, GetDimensions().Center(), source, Color.White * .35f, 0, source.Size() / 2, Main.inventoryScale, SpriteEffects.None, 0);
		}

		if (!DrawVisibility(spriteBatch))
			HandleItemSlotLogic();

		Main.inventoryScale = oldScale;
	}

	/// <returns> Whether an interaction occured. </returns>
	private bool DrawVisibility(SpriteBatch spriteBatch)
	{
		if (_isVanity)
			return false;

		var mPlayer = Main.LocalPlayer.GetModPlayer<BackpackPlayer>();
		var visTexture = (mPlayer.backpackVisible ? TextureAssets.InventoryTickOn : TextureAssets.InventoryTickOff).Value;

		var point = new Point((int)(GetDimensions().X + 34), (int)GetDimensions().Y - 2);
		var area = new Rectangle(point.X, point.Y, visTexture.Width, visTexture.Height);
		
		spriteBatch.Draw(visTexture, area.Center(), null, Color.White * .8f, 0, visTexture.Size() / 2, 1, SpriteEffects.None, 0);

		if (area.Contains(Main.MouseScreen.ToPoint()) && !PlayerInput.IgnoreMouseInterface)
		{
			Main.HoverItem = new Item();
			Main.hoverItemName = Lang.inter[59].Value;

			Main.LocalPlayer.mouseInterface = true;
			if (Main.mouseLeft && Main.mouseLeftRelease)
			{
				mPlayer.backpackVisible = !mPlayer.backpackVisible;
				SoundEngine.PlaySound(SoundID.MenuTick);

				if (Main.netMode == NetmodeID.MultiplayerClient)
					NetMessage.SendData(MessageID.SyncPlayer, -1, -1, null, Main.LocalPlayer.whoAmI);
			}

			Main.HoverItem = new Item();
			Main.hoverItemName = Lang.inter[mPlayer.backpackVisible ? 59 : 60].Value;

			return true;
		}

		return false;
	}
}
