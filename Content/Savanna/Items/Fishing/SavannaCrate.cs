using Terraria.GameContent.ItemDropRules;
using SpiritReforged.Content.Savanna.Tiles;
using SpiritReforged.Common.ItemCommon;

namespace SpiritReforged.Content.Savanna.Items.Fishing;

public class SavannaCrate : ModItem
{
	public override void SetDefaults() => Item.DefaultToPlaceableTile(ModContent.TileType<SavannaCrateTile>());
	public override bool CanRightClick() => true;

	public override void ModifyItemLoot(ItemLoot itemLoot)
	{
		var main = ItemDropRule.OneFromOptions(1, ModContent.ItemType<HuntingRifle.HuntingRifle>(), ItemID.SandstorminaBottle, ItemID.AnkletoftheWind, 
			ItemID.MysticCoilSnake, ItemID.FeralClaws);

		CrateHelper.BiomeCrate(itemLoot, main, ItemDropRule.NotScalingWithLuck(ItemID.BambooBlock, 3, 20, 50), ItemDropRule.NotScalingWithLuck(ItemID.DesertFossil, 3, 20, 50));
	}
}
