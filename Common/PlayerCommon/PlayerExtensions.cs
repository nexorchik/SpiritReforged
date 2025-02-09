using SpiritReforged.Common.BuffCommon;
using SpiritReforged.Common.ItemCommon;

namespace SpiritReforged.Common.PlayerCommon;

internal static class PlayerExtensions
{
	public static bool HasAccessory(this Player player, Item item) => item.ModItem is AccessoryItem acc && player.GetModPlayer<MiscAccessoryPlayer>().accessory[acc.AccName];
	public static bool HasAccessory(this Player player, ModItem item) => item is AccessoryItem acc && player.GetModPlayer<MiscAccessoryPlayer>().accessory[acc.AccName];
	public static bool HasAccessory<TItem>(this Player player) where TItem : AccessoryItem => player.GetModPlayer<MiscAccessoryPlayer>().accessory[ModContent.GetInstance<TItem>().AccName];
	public static bool HasAccessory(this Player player, int itemId) => HasAccessory(player, ContentSamples.ItemsByType[itemId]);

	/// <summary> Checks whether the player is in the corruption, crimson, or hallow. </summary>
	public static bool ZoneEvil(this Player player) => player.ZoneCorrupt || player.ZoneCrimson || player.ZoneHallow;
	public static bool FallThrough(this Player player) => player.GetModPlayer<CollisionPlayer>().FallThrough();
	public static bool UsedQuickBuff(this Player player) => player.GetModPlayer<BuffPlayer>().usedQuickBuff;
}
