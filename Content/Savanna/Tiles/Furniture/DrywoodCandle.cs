using SpiritReforged.Common.TileCommon;
using Terraria.GameContent.Drawing;

namespace SpiritReforged.Content.Savanna.Tiles.Furniture;

public class DrywoodCandle : ModTile, IAutoloadTileItem
{
	private static Asset<Texture2D> GlowTexture;

	public void AddItemRecipes(ModItem item) => item.CreateRecipe()
		.AddIngredient<Items.Drywood.Drywood>(4)
		.AddIngredient(ItemID.Torch)
		.AddTile(TileID.WorkBenches)
		.Register();

	public override void Load()
	{
		if (!Main.dedServ)
			GlowTexture = ModContent.Request<Texture2D>(Texture + "_Glow");
	}

	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLighted[Type] = true;
		Main.tileLavaDeath[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.StyleOnTable1x1);
		TileObjectData.addTile(Type);

		RegisterItemDrop(Mod.Find<ModItem>(Name + "Item").Type);
		AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
		AddMapEntry(new Color(100, 100, 60), Language.GetText("ItemName.Candle"));
		AdjTiles = [TileID.Candles];
		DustType = -1;
	}

	public override void HitWire(int i, int j)
	{
		var tile = Framing.GetTileSafely(i, j);
		tile.TileFrameX = (short)((tile.TileFrameX == 0) ? 18 : 0);

		NetMessage.SendTileSquare(-1, i, j);
	}

	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
	{
		var tile = Framing.GetTileSafely(i, j);
		var color = Color.Orange * .75f;

		if (tile.TileFrameX < 18)
			(r, g, b) = (color.R / 255f, color.G / 255f, color.B / 255f);
	}

	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var tile = Framing.GetTileSafely(i, j);
		if (!TileDrawing.IsVisible(tile))
			return;

		var texture = GlowTexture.Value;
		var source = new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16);
		var zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
		var position = new Vector2(i, j) * 16 - Main.screenPosition + zero;

		spriteBatch.Draw(texture, position, source, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0f);

		if (tile.TileFrameY == 0 && tile.TileFrameX == 0)
			DrawFlames(i, j, spriteBatch);
	}

	private static void DrawFlames(int i, int j, SpriteBatch spriteBatch)
	{
		var zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
		var texture = TextureAssets.Flames[0].Value;
		var origin = new Rectangle(0, 0, 22, 20);
		ulong randSeed = Main.TileFrameSeed ^ (ulong)((long)j << 32 | (uint)i);

		for (int c = 0; c < 7; c++)
		{
			float shakeX = Utils.RandomInt(ref randSeed, -10, 11) * 0.15f;
			float shakeY = Utils.RandomInt(ref randSeed, -10, 1) * 0.35f;
			var offset = new Vector2(shakeX + 8, shakeY + 22);

			var position = new Vector2(i, j) * 16 - Main.screenPosition + offset + zero;
			spriteBatch.Draw(texture, position, origin, new Color(100, 100, 100, 0), 0, origin.Bottom(), 1, SpriteEffects.None, 0f);
		}

		if (Main.rand.NextBool(100))
			Dust.NewDustPerfect(new Vector2(i, j) * 16 + new Vector2(8), DustID.Torch, (Vector2.UnitY * -Main.rand.NextFloat(2f)).RotatedByRandom(.5f));
	}
}
