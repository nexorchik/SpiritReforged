using SpiritReforged.Common.ItemCommon;
using SpiritReforged.Content.Ocean.Items.PoolNoodle;
using System.IO;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace SpiritReforged.Content.Forest.ArcaneNecklace;

public class ArcaneNecklaceItem : AccessoryItem
{
	private const int numStyles = 2;
	private byte style;

	public override void SetStaticDefaults()
	{
		Main.RegisterItemAnimation(Type, new DrawAnimationVertical(2, numStyles) { NotActuallyAnimating = true });
	}

	public override void SetDefaults()
	{
		Item.width = 26;
		Item.height = 34;
		Item.value = Item.sellPrice(0, 0, 25, 0);
		Item.rare = ItemRarityID.Blue;
		Item.accessory = true;
		style = (byte)Main.rand.Next(numStyles);
	}

	protected override bool CloneNewInstances => true;

	public override ModItem Clone(Item itemClone)
	{
		var myClone = (ArcaneNecklaceItem)base.Clone(itemClone);
		myClone.style = style;

		return myClone;
	}

	public override void SafeUpdateAccessory(Player player, bool hideVisual) => player.statManaMax2 += 20;

	public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
	{
		Texture2D texture = TextureAssets.Item[Type].Value;
		frame = texture.Frame(1, numStyles, 0, style, 0, 2);

		spriteBatch.Draw(texture, position, frame, Item.GetAlpha(drawColor), 0f, origin, scale, SpriteEffects.None, 0f);
		return false;
	}

	public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
	{
		Texture2D texture = TextureAssets.Item[Type].Value;
		Rectangle frame = texture.Frame(1, numStyles, 0, style, 0, 2);

		spriteBatch.Draw(texture, Item.Center - Main.screenPosition, frame, Item.GetAlpha(lightColor), rotation, frame.Size() / 2, scale, SpriteEffects.None, 0f);
		return false;
	}

	public override void SaveData(TagCompound tag) => tag[nameof(style)] = style;
	public override void LoadData(TagCompound tag) => style = tag.Get<byte>(nameof(style));
	public override void NetSend(BinaryWriter writer) => writer.Write(style);
	public override void NetReceive(BinaryReader reader) => style = reader.ReadByte();
}
