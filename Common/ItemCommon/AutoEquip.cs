/*namespace SpiritReforged.Common.ItemCommon;

/// <summary> Calls <see cref="FrameEffects"/> on this ModItem when it is equipped. </summary>
internal interface IFrameEffects
{
	/// <inheritdoc cref="ModPlayer.FrameEffects"/>.
	public void FrameEffects(Player player);
}

internal class AutoEquipItem : GlobalItem
{
	public override bool AppliesToEntity(Item entity, bool lateInstantiation) => AutoEquipPlayer.IsActiveType(entity.type);

	public override void UpdateEquip(Item item, Player player) => AutoEquipPlayer.SetActive(item.type);
	public override void UpdateVanity(Item item, Player player) => AutoEquipPlayer.SetActive(item.type);
}

internal class AutoEquipPlayer : ModPlayer
{
	private static Dictionary<int, bool> Actives = [];

	public static bool SetActive(int itemType) => Actives[itemType] = true;
	public static bool IsActiveType(int itemType) => Actives.ContainsKey(itemType);

	public override void SetStaticDefaults()
	{
		foreach (var item in Mod.GetContent<ModItem>())
		{
			if (item is IFrameEffects)
				Actives.Add(item.Type, false);
		}
	}

	public override void ResetEffects()
	{
		if (Main.gamePaused)
			return;

		Dictionary<int, bool> clear = [];
		foreach (var pair in Actives)
			clear.Add(pair.Key, false);

		Actives = clear;
	}

	public override void FrameEffects()
	{
		foreach (int type in Actives.Keys)
		{
			if (Actives[type] || InArmorVanity(type))
				(ItemLoader.GetItem(type) as IFrameEffects).FrameEffects(Player);
		}
	}

	/// <summary> Armor vanity can't be checked with a hook in <see cref="AutoEquipItem"/> (or anywhere at all), so do it manually. </summary>
	private bool InArmorVanity(int type)
	{
		for (int i = 10; i < 13; i++)
		{
			if (Player.armor[i]?.ModItem is IFrameEffects && Player.armor[i].type == type)
				return true;
		}

		return false;
	}
}*/