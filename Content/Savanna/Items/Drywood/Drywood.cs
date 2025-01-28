using SpiritReforged.Common.Misc;
using SpiritReforged.Content.Savanna.Tiles;

namespace SpiritReforged.Content.Savanna.Items.Drywood;

public class Drywood : ModItem
{
	public override void SetStaticDefaults()
	{
		ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.Wood;
		Recipes.AddToGroup(RecipeGroupID.Wood, Type);
	}

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<DrywoodTile>());
		Item.width = 14;
		Item.height = 14;
	}
}
