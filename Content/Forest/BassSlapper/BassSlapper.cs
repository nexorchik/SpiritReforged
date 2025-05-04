using SpiritReforged.Common.ItemCommon;

namespace SpiritReforged.Content.Forest.BassSlapper;

public class BassSlapper : ClubItem
{
	internal override float DamageScaling => 1.5f;

	public override void SafeSetDefaults()
	{
		Item.damage = 28;
		Item.knockBack = 8;
		ChargeTime = 40;
		SwingTime = 30;
		Item.width = 60;
		Item.height = 60;
		Item.crit = 4;
		Item.value = Item.sellPrice(0, 0, 60, 0);
		Item.rare = ItemRarityID.Blue;
		Item.shoot = ModContent.ProjectileType<BassSlapperProj>();
	}
}