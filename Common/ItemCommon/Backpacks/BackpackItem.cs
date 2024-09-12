using Terraria.ModLoader.IO;

namespace SpiritReforged.Common.ItemCommon.Backpacks;

internal abstract class BackpackItem : ModItem
{
	protected abstract int SlotCap { get; }

	private Item[] Items = [];

	public sealed override void SetDefaults()
	{
		Defaults();

		Items = new Item[SlotCap];
	}

	public virtual void Defaults() { }

	public override void SaveData(TagCompound tag)
	{
		for (int i = 0; i < Items.Length; i++)
		{
			if (Items[i] is not null)
				tag.Add("item" + i, Items[i]);
		}
	}

	public override void LoadData(TagCompound tag)
	{
		Items = new Item[SlotCap];

		for (int i = 0; i < Items.Length; i++)
		{
			if (Items[i] is not null && tag.TryGet("item" + i, out TagCompound itemTag))
				Items[i] = ItemIO.Load(itemTag);

		}
	}
}
