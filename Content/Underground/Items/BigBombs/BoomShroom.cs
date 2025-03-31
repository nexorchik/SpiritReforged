using SpiritReforged.Common.ItemCommon;
using SpiritReforged.Common.PlayerCommon;

namespace SpiritReforged.Content.Underground.Items.BigBombs;

public class BoomShroom : AccessoryItem
{
	public override void SetDefaults()
	{
		Item.width = 28;
		Item.height = 20;
		Item.value = Item.sellPrice(0, 3, 0, 0);
		Item.rare = ItemRarityID.Blue;
		Item.accessory = true;
	}
}

internal class BoomShroomPlayer : ModPlayer
{
	public static readonly Dictionary<int, int> OriginalTypes = [];

	public override void ModifyShootStats(Item item, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
	{
		if (Player.HasAccessory<BoomShroom>() && OriginalTypes.TryGetValue(type, out int t))
			type = t;
	}
}