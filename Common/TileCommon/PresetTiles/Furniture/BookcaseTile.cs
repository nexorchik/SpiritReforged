using Terraria.DataStructures;

namespace SpiritReforged.Common.TileCommon.PresetTiles;

public abstract class BookcaseTile : FurnitureTile
{
	public override void SetItemDefaults(ModItem item) => item.Item.value = Item.sellPrice(copper: 60);

	public override void AddItemRecipes(ModItem item)
	{
		if (CoreMaterial != ItemID.None)
			item.CreateRecipe()
			.AddIngredient(CoreMaterial, 20)
			.AddIngredient(ItemID.Book, 10)
			.AddTile(TileID.Sawmill)
			.Register();
	}

	public override void StaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
		TileObjectData.newTile.Origin = new Point16(2, 3);
		TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 18];
		TileObjectData.addTile(Type);

		AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);
		AddMapEntry(new Color(100, 100, 60), Language.GetText("ItemName.Bookcase"));
		AdjTiles = [TileID.Bookcases];
		DustType = -1;
	}
}
