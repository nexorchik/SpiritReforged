using SpiritReforged.Common.PlayerCommon;
using Terraria;
using Terraria.ModLoader;

namespace SpiritReforged.Common.ItemCommon;

/// <summary>Automatically provides equip flags for items in the MiscAccessoryPlayer.accessory instanced dictionary.</summary>
public abstract class AccessoryItem : ModItem
{
	public virtual string AccName => GetType().Name;

	public sealed override void UpdateAccessory(Player player, bool hideVisual)
	{
		player.GetModPlayer<MiscAccessoryPlayer>().accessory[AccName] = true;
		SafeUpdateAccessory(player, hideVisual);
	}

	public virtual void SafeUpdateAccessory(Player player, bool hideVisual)
	{
	}
}
