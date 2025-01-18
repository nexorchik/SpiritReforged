using SpiritReforged.Common.ItemCommon;

namespace SpiritReforged.Content.Jungle.Misc.DyeCrate;

internal class DyeCrateItem : ModItem
{
	public override void SetStaticDefaults() => Item.ResearchUnlockCount = 10;

	public override void SetDefaults()
	{
		Item.CloneDefaults(ItemID.HerbBag);
		Item.Size = new Vector2(32, 36);
	}

	public override bool CanRightClick() => true;

	public override void ModifyItemLoot(ItemLoot itemLoot)
	{
		Range[] ranges = [2..5, 1..3, 1..3, 1..3, 1..3, 1..3, 1..3, 1..3, 1..3, 1..3, 1..3, 1..3];
		int[] ids = [ItemID.BlueBerries, ItemID.CyanHusk, ItemID.RedHusk, ItemID.VioletHusk,
			ItemID.GreenMushroom, ItemID.LimeKelp, ItemID.OrangeBloodroot, ItemID.PinkPricklyPear, ItemID.PurpleMucos, ItemID.SkyBlueFlower, ItemID.TealMushroom,
			ItemID.YellowMarigold];

		itemLoot.Add(new DropRules.LootPoolDrop(ranges, 4, 1, 1, ids));

		Range[] strangePlantRanges = [1..3, 1..3, 1..3, 1..3];
		int[] strangePlants = [ItemID.StrangePlant1, ItemID.StrangePlant2, ItemID.StrangePlant3, ItemID.StrangePlant4];
		itemLoot.Add(new DropRules.LootPoolDrop(strangePlantRanges, 1, 10, 1, ids));
	}
}
