using SpiritReforged.Common.UI.Misc;
using Terraria.UI;

namespace SpiritReforged.Common.UI.BackpackInterface;

internal class PackInventorySlot(Item[] items, int index) : BasicItemSlot(items, index, ItemSlot.Context.ChestItem, .6f)
{
	private static Asset<Texture2D> favourite;
	private Asset<Texture2D> Favourite
	{
		get
		{
			if (favourite?.IsLoaded == true)
				return favourite;
			else
			{
				favourite = ModContent.Request<Texture2D>((GetType().Namespace + '.' + "Slot_Favourite").Replace('.', '/'));
				return TextureAssets.InventoryBack5; //Return a placeholder for one tick because loading is deferred
			}
		}
	}

	protected override void DrawSelf(SpriteBatch spriteBatch)
	{
		if (_items[_index].favorited && Favourite.IsLoaded) //Draw a unique favourite texture
		{
			var oldTexture = TextureAssets.InventoryBack10;
			TextureAssets.InventoryBack10 = Favourite;

			base.DrawSelf(spriteBatch);

			TextureAssets.InventoryBack10 = oldTexture;
			return;
		}

		base.DrawSelf(spriteBatch);
	}
}