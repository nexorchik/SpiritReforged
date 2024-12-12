using SpiritReforged.Content.Common.PlayerCommon;

namespace SpiritReforged.Common.ItemCommon;

/// <summary> A class that handles boilerplate code that every minion accessory (accessories that spawn a guy) has. 
/// Needs each Minion Accessory to provide damage and a projectile type
public abstract class MinionAccessory : AccessoryItem
{
	public virtual int ProjType { get; }
	public virtual int Damage { get; }

	public override void UpdateAccessory(Player player, bool hideVisual) => player.GetModPlayer<MinionAccessoryPlayer>().MinionProjectileData[ProjType] = Damage;
}
