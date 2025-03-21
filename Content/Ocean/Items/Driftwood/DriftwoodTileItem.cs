using SpiritReforged.Common.ItemCommon;
using SpiritReforged.Common.Misc;
using Terraria.GameContent.ItemDropRules;

namespace SpiritReforged.Content.Ocean.Items.Driftwood;

public class DriftwoodTileItem : ModItem
{
	public override void SetStaticDefaults()
	{
		Recipes.AddToGroup(RecipeGroupID.Wood, Type);

		CrateDatabase.AddCrateRule(ItemID.OceanCrate, ItemDropRule.Common(Type, 5, 10, 30));
		CrateDatabase.AddCrateRule(ItemID.OceanCrateHard, ItemDropRule.Common(Type, 5, 10, 30));
	}

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<DriftwoodTile>());
		Item.width = Item.height = 16;
		Item.rare = ItemRarityID.White;
		Item.maxStack = Item.CommonMaxStack;
	}
}

public class DriftwoodTile : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileBlockLight[Type] = true;
		Main.tileBrick[Type] = true;
		Main.tileMergeDirt[Type] = true;

		AddMapEntry(new Color(138, 79, 45));
	}
}