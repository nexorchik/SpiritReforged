using SpiritReforged.Common.ItemCommon;
using SpiritReforged.Common.TileCommon;
using SpiritReforged.Common.TileCommon.PresetTiles;

namespace SpiritReforged.Content.Ocean.Tiles.Furniture;

public class DriftwoodBarrel : ChestTile
{
	public override void AddItemRecipes(ModItem item) => item.CreateRecipe()
		.AddIngredient(ItemMethods.AutoItemType<Driftwood>(), 9)
		.AddRecipeGroup(RecipeGroupID.IronBar)
		.AddTile(TileID.Sawmill)
		.Register();

	public override void StaticDefaults()
	{
		base.StaticDefaults();
		DustType = DustID.BorealWood;
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var tile = Framing.GetTileSafely(i, j);
		var texture = TextureAssets.Tile[Type].Value;
		var source = new Rectangle(tile.TileFrameX, tile.TileFrameY % 36, 16, tile.TileFrameY > 0 ? 18 : 16);
		var drawPos = new Vector2(i, j) * 16 - Main.screenPosition + TileExtensions.TileOffset;

		spriteBatch.Draw(texture, drawPos, source, Lighting.GetColor(i, j), 0, Vector2.Zero, 1, SpriteEffects.None, 0);

		if (Main.InSmartCursorHighlightArea(i, j, out bool actuallySelected))
			spriteBatch.Draw(TextureAssets.HighlightMask[Type].Value, drawPos, source, actuallySelected ? Color.Yellow : Color.Gray, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

		return false;
	}
}
