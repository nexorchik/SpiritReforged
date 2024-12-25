using SpiritReforged.Common.ItemCommon;
using SpiritReforged.Common.PlayerCommon;
using SpiritReforged.Content.Desert.GildedScarab;

namespace SpiritReforged.Content.Vanilla.LeatherCloak;

internal class LeatherCloak : AccessoryItem
{
	public override void SetDefaults()
	{
		Item.width = 26;
		Item.height = 26;
		Item.rare = ItemRarityID.Blue;
		Item.accessory = true;
		Item.value = Item.sellPrice(0, 0, 5, 0);
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		if (player.ZoneDesert)
			player.buffImmune[BuffID.WindPushed] = true;
	}
	private class LeatherCloakPlayer : ModPlayer
	{
		public override void PostUpdateRunSpeeds()
		{
			if (Player.HasAccessory<LeatherCloak>())
			{
				Player.runAcceleration *= 1.15f;
				Player.maxRunSpeed += 0.1f;
				Player.accRunSpeed += 0.05f;
			}
		}
	}
}