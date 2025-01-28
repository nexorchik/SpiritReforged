using SpiritReforged.Common.ItemCommon;

namespace SpiritReforged.Common.PlayerCommon;

public class MiscAccessoryPlayer : ModPlayer
{
	public readonly Dictionary<string, bool> accessory = [];

	public override void Initialize()
	{
		accessory.Clear();

		foreach (var item in ModContent.GetContent<AccessoryItem>())
			accessory.Add(item.AccName, false);
	}

	public override void ResetEffects()
	{
		foreach (string item in accessory.Keys)
			accessory[item] = false;
	}

	public override void UpdateDead() => ResetEffects();
}
