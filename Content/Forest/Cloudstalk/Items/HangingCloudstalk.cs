using SpiritReforged.Common.TileCommon;
using SpiritReforged.Common.TileCommon.TileSway;
using Terraria.DataStructures;

namespace SpiritReforged.Content.Forest.Cloudstalk.Items;

public class HangingCloudstalk : ModItem
{
	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<HangingCloudstalkTile>());
		Item.width = 36;
		Item.height = 28;
		Item.value = Item.sellPrice(0, 0, 1, 50);
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient(ItemID.PotSuspended);
		recipe.AddIngredient(ModContent.ItemType<Cloudstalk>());
		recipe.Register();
	}
}

[DrawOrder(DrawOrderAttribute.Layer.NonSolid)]
public class HangingCloudstalkTile : ModTile, ISwayInWind
{
	public override void SetStaticDefaults()
	{
		Main.tileTable[Type] = true;
		Main.tileSolidTop[Type] = false;
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;
		Main.tileLighted[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
		TileObjectData.newTile.Width = 2;
		TileObjectData.newTile.Height = 3;
		TileObjectData.newTile.CoordinateHeights = [16, 16, 16];
		TileObjectData.newTile.DrawYOffset = -2;
		TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.AnchorTop = new AnchorData(TileObjectData.newTile.AnchorTop.type, 2, 0);
		TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
		TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
		TileObjectData.addAlternate(1);
		TileObjectData.addTile(Type);

		AddMapEntry(new Color(28, 138, 72));
		RegisterItemDrop(ModContent.ItemType<HangingCloudstalk>());

		DustType = -1;
	}

	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) => (r, g, b) = (0.2f, 0.2f, 0.4f);

	public void ModifyRotation(int i, int j, ref float rotation)
	{
		var tile = Framing.GetTileSafely(i, j);
		if (tile.TileFrameY == 0)
			rotation *= .5f;
	}

	public float SetWindSway(Point16 topLeft)
	{
		var data = TileObjectData.GetTileData(Framing.GetTileSafely(topLeft));
		float rotation = Main.instance.TilesRenderer.GetWindCycle(topLeft.X, topLeft.Y, TileSwaySystem.Instance.SunflowerWindCounter);

		if (!WorldGen.InAPlaceWithWind(topLeft.X, topLeft.Y, data.Width, data.Height))
			rotation = 0f;

		return rotation + TileSwayHelper.GetHighestWindGridPushComplex(topLeft.X, topLeft.Y, data.Width, data.Height, 50, 1.15f, 3, true);
	}
}
