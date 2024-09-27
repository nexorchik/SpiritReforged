using SpiritReforged.Common.UI.BackpackInterface;
using System.Linq;
using Terraria.ModLoader.IO;

namespace SpiritReforged.Common.ItemCommon.Backpacks;

internal abstract class BackpackItem : ModItem
{
	protected override bool CloneNewInstances => true;

	/// <summary>
	/// How many slots this backpack has.
	/// </summary>
	protected abstract int SlotCap { get; }

	public Item[] Items = [];

	public override ModItem Clone(Item newEntity)
	{
		ModItem clone = base.Clone(newEntity);
		(clone as BackpackItem).Items = Items;
		return clone;
	}

	public sealed override void SetDefaults()
	{
		Defaults();

		Items = new Item[SlotCap];

		for (int i = 0; i < SlotCap; i++)
			Items[i] = BackpackUIState.AirItem;
	}

	public virtual void Defaults() { }

	public override bool OnPickup(Player player)
	{
		player.GetModPlayer<BackpackPlayer>().Backpack = Item;
		BackpackUISystem.SetBackpack();
		return true;
	}

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
			else
				Items[i] = BackpackUIState.AirItem;
		}
	}
}
