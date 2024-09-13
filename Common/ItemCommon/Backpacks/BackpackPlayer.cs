using SpiritReforged.Common.UI.BackpackInterface;
using Terraria.ModLoader.IO;

namespace SpiritReforged.Common.ItemCommon.Backpacks;

internal class BackpackPlayer : ModPlayer
{
	public Item Backpack = BackpackUIState.AirItem;
	public Item VanityBackpack = BackpackUIState.AirItem;

	public override void SaveData(TagCompound tag)
	{
		if (Backpack is not null)
			tag.Add("backpack", ItemIO.Save(Backpack));

		if (VanityBackpack is not null)
			tag.Add("vanity", ItemIO.Save(VanityBackpack));
	}

	public override void LoadData(TagCompound tag)
	{
		if (tag.TryGet("backpack", out TagCompound item))
			Backpack = ItemIO.Load(item);

		if (tag.TryGet("vanity", out TagCompound vanity))
			Backpack = ItemIO.Load(vanity);
	}
}
