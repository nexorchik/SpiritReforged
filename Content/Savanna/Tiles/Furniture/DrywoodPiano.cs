using SpiritReforged.Common.TileCommon;
using Terraria.DataStructures;

namespace SpiritReforged.Content.Savanna.Tiles.Furniture;

public class DrywoodPiano : ModTile, IAutoloadTileItem
{
	public void SetItemDefaults(ModItem item) => item.Item.value = Item.sellPrice(copper: 60);

	public void AddItemRecipes(ModItem item) => item.CreateRecipe()
		.AddIngredient(ItemID.Bone, 4)
		.AddIngredient<Items.Drywood.Drywood>(15)
		.AddIngredient(ItemID.Book)
		.AddTile(TileID.Sawmill)
		.Register();

	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
		TileObjectData.newTile.Origin = new Point16(2, 1);
		TileObjectData.newTile.CoordinateHeights = [16, 16];
		TileObjectData.addTile(Type);

		AddMapEntry(new Color(100, 100, 60), Language.GetText("ItemName.Piano"));
		AdjTiles = [TileID.Pianos];
		DustType = -1;
	}
}
