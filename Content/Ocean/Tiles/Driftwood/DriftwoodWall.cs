using SpiritReforged.Common.WallCommon;

namespace SpiritReforged.Content.Ocean.Tiles.Driftwood;

public class DriftwoodWall : ModWall, IAutoloadWallItem
{
	public void AddItemRecipes(ModItem item)
	{
		item.CreateRecipe(4)
			.AddIngredient(ModContent.ItemType<Items.Driftwood.DriftwoodTileItem>())
			.AddTile(TileID.WorkBenches)
			.Register();

		//Allow wall items to be crafted back into base materials
		Recipe.Create(ModContent.ItemType<Items.Driftwood.DriftwoodTileItem>())
			.AddIngredient(item.Type, 4)
			.AddTile(TileID.WorkBenches)
			.Register();
	}

	public override void SetStaticDefaults()
	{
		Main.wallHouse[Type] = true;
		AddMapEntry(new Color(87, 61, 44));
	}
}