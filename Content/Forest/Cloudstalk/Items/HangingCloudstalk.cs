using Terraria.DataStructures;
using Terraria.GameContent.Drawing;

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

public class HangingCloudstalkTile : ModTile
{
	private double windSway;

	public override void Load() => On_TileDrawing.Update += UpdateTileDrawing;

	private void UpdateTileDrawing(On_TileDrawing.orig_Update orig, TileDrawing self)
	{
		if (!Main.dedServ) //Wind speed calculation
		{
			double num = Math.Abs(Main.WindForVisuals);

			num = Utils.GetLerpValue(0.08f, 1.2f, (float)num, clamped: true);
			windSway += 1.0 / 420.0 + 1.0 / 420.0 * num * 5.0; //Sunflower speed
		}

		orig(self);
	}

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
		TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
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

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		Tile tile = Framing.GetTileSafely(i, j);
		var topLeft = new Point16(i - tile.TileFrameX % 36 / 18, j - tile.TileFrameY / 18);

		if (new Point16(i, j) != topLeft) //Draw only once on the multitile origin so we have control over iterations
			return false;

		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Matrix.Identity);
		//Restart the spritebatch so the tile isn't blurry

		float GetWindSway()
		{
			float rotation = Main.instance.TilesRenderer.GetWindCycle(topLeft.X, topLeft.Y, windSway);

			if (!WorldGen.InAPlaceWithWind(topLeft.X, topLeft.Y, 2, 3))
				rotation = 0f;

			return rotation + GetHighestWindGridPushComplex(topLeft.X, topLeft.Y, 2, 3, 60, 1.26f, 3, true);
			//Main.instance.TilesRenderer.GetWindGridPushComplex(topLeft.X, topLeft.Y, 60, 1.26f, 3, true);
		}

		float windCycle = GetWindSway();
		Texture2D texture = TextureAssets.Tile[Type].Value;
		Vector2 offset = Lighting.LegacyEngine.Mode > 1 && Main.GameZoomTarget == 1 ? Vector2.Zero : Vector2.One * 12;

		int _i = i;
		int _j = j;

		for (int y = 0; y < 3; y++)
			for (int x = 0; x < 2; x++)
			{
				(i, j) = (_i + x, _j + y);

				tile = Framing.GetTileSafely(i, j);

				var source = new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16);
				var drawPos = new Vector2((i + offset.X) * 16 - (int)Main.screenPosition.X, (j + offset.Y) * 16 - (int)Main.screenPosition.Y);

				var origin = new Vector2(16 - tile.TileFrameX % 36 / 18 * 16, -(tile.TileFrameY / 18 * 16));
				drawPos += origin;
				drawPos += new Vector2(TileObjectData.GetTileData(tile.TileType, 0).DrawXOffset, TileObjectData.GetTileData(tile.TileType, 0).DrawYOffset);

				float swing = 3f * (j - topLeft.Y + 1) / 54f;
				float rotation = -windCycle * MathHelper.Max(swing, .1f);

				drawPos += new Vector2(windCycle, Math.Abs(windCycle) * -4f * swing);

				spriteBatch.Draw(texture, drawPos, source, Lighting.GetColor(i, j), rotation, origin, 1, SpriteEffects.None, 0f);
			}

		spriteBatch.End();
		spriteBatch.Begin();

		return false;
	}

	private static float GetHighestWindGridPushComplex(int topLeftX, int topLeftY, int sizeX, int sizeY, int totalPushTime, float pushForcePerFrame, int loops, bool swapLoopDir) //Adapted from vanilla
	{
		float result = 0f;
		int num = int.MaxValue;
		for (int i = 0; i < 1; i++)
			for (int j = 0; j < sizeY; j++)
			{
				Main.instance.TilesRenderer.Wind.GetWindTime(topLeftX + i + sizeX / 2, topLeftY + j, totalPushTime, out var windTimeLeft, out var _, out var _);
				float windGridPushComplex = Main.instance.TilesRenderer.GetWindGridPushComplex(topLeftX + i, topLeftY + j, totalPushTime, pushForcePerFrame, loops, swapLoopDir);
				if (windTimeLeft < num && windTimeLeft != 0)
				{
					result = windGridPushComplex;
					num = windTimeLeft;
				}
			}

		return result;
	}
}
