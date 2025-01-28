using System.IO;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace SpiritReforged.Common.ItemCommon;

internal class VariantGlobalItem : GlobalItem
{
	public override bool InstancePerEntity => true;

	private struct VarData(Point[] sizes, bool inventory)
	{
		public Point[] sizes = sizes;
		public bool inventory = inventory;
	}

	private static readonly Dictionary<int, VarData> variantData = []; //Type, frame sizes
	private int subID = -1;

	/// <summary> Registers the given item <paramref name="type"/> as having variants of <paramref name="amount"/>.<para/>
	/// The item texture must be a vertical sheet with 2px padding to work correctly. </summary>
	/// <param name="type"> The item type. </param>
	/// <param name="amount"> The number of variants. </param>
	/// <param name="appliesToInventory"> Whether this item can use variants in the inventory. </param>
	public static void AddVariants(int type, int amount, bool appliesToInventory = false)
	{
		variantData.Add(type, new VarData(null, appliesToInventory));
		Main.RegisterItemAnimation(type, new DrawAnimationVertical(2, amount) { NotActuallyAnimating = true });
	}

	/// <summary><inheritdoc cref="AddVariants(int, int, bool)"/></summary>
	/// <param name="type"> The item type. </param>
	/// <param name="sizes"> The dimensions of each individual frame. Also determines the frame count based on length. </param>
	/// <param name="appliesToInventory"> Whether this item can use variants in the inventory. </param>
	public static void AddVariants(int type, Point[] sizes, bool appliesToInventory = false)
	{
		variantData.Add(type, new VarData(sizes, appliesToInventory));
		Main.RegisterItemAnimation(type, new DrawAnimationVertical(2, sizes.Length) { NotActuallyAnimating = true });
	}

	/// <returns> Which variant this item is using. </returns>
	public static int GetVariant(Item item) => item.TryGetGlobalItem(out VariantGlobalItem vItem) ? vItem.subID : 0;

	/// <summary> Gets the draw frame of this varianted <paramref name="item"/>. </summary>
	/// <param name="item"> The item. </param>
	/// <returns> <see cref="Rectangle.Empty"/> if the item is invalid. </returns>
	public static Rectangle GetSource(Item item)
	{
		if (item.TryGetGlobalItem(out VariantGlobalItem vItem))
			return vItem.GetSource(item.type);

		return Rectangle.Empty;
	}

	private Rectangle GetSource(int type)
	{
		int frameCount = Main.itemAnimations[type].FrameCount;
		int frame = Math.Max(subID, 0);

		var texture = TextureAssets.Item[type].Value;
		var rectangle = texture.Frame(1, frameCount, 0, frame, 0, -2);
		var sizes = variantData[type].sizes;

		if (sizes != null && frame < sizes.Length)
		{
			rectangle.Width = sizes[frame].X;
			rectangle.Height = sizes[frame].Y;
		}

		return rectangle;
	}

	public override bool AppliesToEntity(Item entity, bool lateInstantiation) => variantData.ContainsKey(entity.type);

	public override void SetDefaults(Item item)
	{
		if (variantData[item.type].inventory && Main.netMode != NetmodeID.MultiplayerClient)
		{
			int frameCount = Main.itemAnimations[item.type].FrameCount;
			subID = Main.rand.Next(frameCount);

			if (Main.netMode != NetmodeID.SinglePlayer)
				NetMessage.SendData(MessageID.SyncItem, number: item.whoAmI);
		}
	}

	public override bool PreDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
	{
		int frameCount = Main.itemAnimations[item.type].FrameCount;

		if (!variantData[item.type].inventory && subID == -1)
			subID = Main.rand.Next(frameCount);

		var texture = TextureAssets.Item[item.type].Value;
		var source = GetSource(item.type);

		spriteBatch.Draw(texture, item.position + item.Size - source.Size() - Main.screenPosition, source, GetAlpha(item, lightColor) ?? lightColor, rotation, Vector2.Zero, scale, SpriteEffects.None, 0);
		return false;
	}

	public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
	{
		if (!variantData[item.type].inventory)
			subID = -1;

		var texture = TextureAssets.Item[item.type].Value;
		var source = GetSource(item.type);

		//Scale the item according to 'source' instead of 'frame'
		ItemSlot.DrawItem_GetColorAndScale(item, Main.inventoryScale, ref drawColor, 32, ref source, out _, out scale);

		spriteBatch.Draw(texture, position, source, drawColor, 0, source.Size() / 2, scale, SpriteEffects.None, 0);
		return false;
	}

	public override void SaveData(Item item, TagCompound tag)
	{
		if (variantData[item.type].inventory)
			tag[nameof(subID)] = (byte)subID;
	}

	public override void LoadData(Item item, TagCompound tag)
	{
		if (variantData[item.type].inventory)
			subID = tag.Get<byte>(nameof(subID));
	}

	public override void NetSend(Item item, BinaryWriter writer)
	{
		if (variantData[item.type].inventory)
			writer.Write((byte)subID);
	}

	public override void NetReceive(Item item, BinaryReader reader)
	{
		if (variantData[item.type].inventory)
			subID = reader.ReadByte();
	}
}
