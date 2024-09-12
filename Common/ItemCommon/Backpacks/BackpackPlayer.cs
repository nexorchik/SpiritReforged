using SpiritReforged.Common.UI.BackpackUI;
using Terraria.GameContent.Items;
using Terraria.ModLoader.IO;

namespace SpiritReforged.Common.ItemCommon.Backpacks;

internal class BackpackPlayer : ModPlayer
{
	public Item Backpack = BackpackUIState.AirItem;

	public override void SaveData(TagCompound tag)
	{
		if (Backpack is not null)
			tag.Add("backpack", ItemIO.Save(Backpack));
	}

	public override void LoadData(TagCompound tag)
	{
		if (tag.TryGet("backpack", out TagCompound item))
			Backpack = ItemIO.Load(item);
	}
}
