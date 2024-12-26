using SpiritReforged.Common.UI.BackpackInterface;
using Terraria.ModLoader.IO;

namespace SpiritReforged.Common.ItemCommon.Backpacks;

internal class BackpackPlayer : ModPlayer
{
	public Item Backpack = BackpackUIState.AirItem;
	public Item VanityBackpack = BackpackUIState.AirItem;
	public bool backpackVisible = true;

	private int _lastSelectedEquipPage = 0;
	private bool _hadBackpack = false;

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

	public override void UpdateEquips()
	{
		if (VanityBackpack is not null && !VanityBackpack.IsAir)
			ApplyEquip(VanityBackpack);
		else if (Backpack is not null && !Backpack.IsAir && backpackVisible)
			ApplyEquip(Backpack);

		if (Player.HeldItem.ModItem is BackpackItem)
		{
			if (!_hadBackpack)
				_lastSelectedEquipPage = Main.EquipPageSelected;

			Main.EquipPageSelected = 2;
		}
		else
		{
			if (_hadBackpack)
				Main.EquipPageSelected = _lastSelectedEquipPage;
		}

		_hadBackpack = Player.HeldItem.ModItem is BackpackItem;
	}

	private void ApplyEquip(Item backpack)
	{
		Player.back = EquipLoader.GetEquipSlot(Mod, backpack.ModItem.Name, EquipType.Back);
		Player.front = EquipLoader.GetEquipSlot(Mod, backpack.ModItem.Name, EquipType.Front);
	}
}
