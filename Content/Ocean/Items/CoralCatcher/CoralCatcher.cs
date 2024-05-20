namespace SpiritReforged.Content.Ocean.Items.CoralCatcher;

public class CoralCatcher : ModItem
{
	public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

	public override void SetDefaults()
    {
        Item.CloneDefaults(ItemID.GoldenFishingRod); 
        Item.fishingPole = 16;  
        Item.value = Item.sellPrice(silver: 25);
        Item.rare = ItemRarityID.Blue;  
        Item.shoot = ModContent.ProjectileType<CoralCatcherHook>();
        Item.shootSpeed = 14f;
    }

	public override void HoldItem(Player player) => player.sonarPotion = true;
}