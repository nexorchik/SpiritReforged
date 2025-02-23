using Terraria.GameContent.ItemDropRules;
using SpiritReforged.Content.Savanna.Tiles;
using SpiritReforged.Common.ItemCommon;

namespace SpiritReforged.Content.Savanna.Items.Fishing;

public class HardmodeSavannaCrate : ModItem
{
	public override void SetStaticDefaults() => ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<SavannaCrate>();
	public override void SetDefaults() => Item.DefaultToPlaceableTile(ModContent.TileType<SavannaCrateHardmodeTile>());
	public override bool CanRightClick() => true;

	public override void ModifyItemLoot(ItemLoot itemLoot)
	{
		var main = ItemDropRule.OneFromOptions(1, ModContent.ItemType<HuntingRifle.HuntingRifle>(), ItemID.SandstorminaBottle, ItemID.AnkletoftheWind, 
			ItemID.MysticCoilSnake, ItemID.FeralClaws);

		CrateHelper.HardmodeBiomeCrate(itemLoot, main, ItemDropRule.NotScalingWithLuck(ItemID.BambooBlock, 3, 20, 50), 
			ItemDropRule.NotScalingWithLuck(ItemID.DesertFossil, 3, 20, 50), ItemDropRule.NotScalingWithLuck(ItemID.Leather, 3, 5, 10));
	}
}
