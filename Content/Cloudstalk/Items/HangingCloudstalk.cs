using Terraria.DataStructures;
using Terraria.GameContent.Drawing;

namespace SpiritReforged.Content.Cloudstalk.Items;

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

	public override void Load() => On_TileDrawing.Update += (On_TileDrawing.orig_Update orig, TileDrawing self) =>
	{
		if (!Main.dedServ) //Wind speed calculation
		{
			double num = Math.Abs(Main.WindForVisuals);

			num = Utils.GetLerpValue(0.08f, 1.2f, (float)num, clamped: true);
			windSway += 1.0 / 420.0 + 1.0 / 420.0 * num * 5.0; //Sunflower speed
		}

		orig(self);
	};

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
		var tile = Framing.GetTileSafely(i, j);
		var data = TileObjectData.GetTileData(tile);

		if (tile.TileFrameX % (data.Width * 18) != 0 || tile.TileFrameY % (data.Height * 18) != 0) //Top left tile only
			return false;

		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Matrix.Identity);
		//Restart the spritebatch so the tile isn't blurry

		var topLeft = new Point16(i, j);
		float windCycle = GetWindSway(topLeft, windSway);

		for (int y = 0; y < data.Height; y++)
		{
			for (int x = 0; x < data.Width; x++)
			{
				(i, j) = (topLeft.X + x, topLeft.Y + y);
				tile = Framing.GetTileSafely(i, j);

				if (!tile.HasTile || tile.TileType != Type)
					continue;

				int frameX = tile.TileFrameX % (data.Width * 18) / 18;
				int frameY = tile.TileFrameY % (data.Height * 18) / 18;
				var origin = new Vector2(-(frameX * 16) + data.Origin.X * 16, -(frameY * 16) + data.Origin.Y * 16);

				float swing = 3f * (data.Origin.Y - (frameY + 1)) / (data.Height * 18);
				float rotation = windCycle * MathHelper.Max(swing, .1f);

				var offset = origin + new Vector2(windCycle, Math.Abs(windCycle) * 4f * swing);

				DrawWithWindSway(i, j, spriteBatch, offset, -rotation, origin);
			}
		}

		spriteBatch.End();
		spriteBatch.Begin();

		return false;
	}

	private void DrawWithWindSway(int i, int j, SpriteBatch spriteBatch, Vector2 offset, float rotation, Vector2 origin)
	{
		var tile = Framing.GetTileSafely(i, j);
		var texture = TextureAssets.Tile[Type].Value;

		Vector2 lightOffset = (Lighting.LegacyEngine.Mode > 1 && Main.GameZoomTarget == 1) ? Vector2.Zero : Vector2.One * 12;
		var drawPos = new Vector2((i + lightOffset.X) * 16 - (int)Main.screenPosition.X, (j + lightOffset.Y) * 16 - (int)Main.screenPosition.Y);
		var source = new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16);

		spriteBatch.Draw(texture, drawPos + offset, source, Lighting.GetColor(i, j), rotation, origin, 1, SpriteEffects.None, 0f);
	}

	private static float GetWindSway(Point16 topLeft, double windSway)
	{
		static float GetHighestWindGridPushComplex(int topLeftX, int topLeftY, int sizeX, int sizeY, int totalPushTime, float pushForcePerFrame, int loops, bool swapLoopDir) //Adapted from vanilla
		{
			float result = 0f;
			int num = int.MaxValue;

			for (int i = 0; i < sizeX; i++)
			{
				for (int j = 0; j < sizeY; j++)
				{
					Main.instance.TilesRenderer.Wind.GetWindTime(topLeftX + i + sizeX / 2, topLeftY + j + sizeY / 2, totalPushTime, out int windTimeLeft, out _, out _);
					float windGridPushComplex = Main.instance.TilesRenderer.GetWindGridPushComplex(topLeftX + i, topLeftY + j, totalPushTime, pushForcePerFrame, loops, swapLoopDir);
					
					if (windTimeLeft < num && windTimeLeft != 0)
					{
						result = windGridPushComplex;
						num = windTimeLeft;
					}
				}
			}

			return result;
		}

		var tile = Framing.GetTileSafely(topLeft);
		var data = TileObjectData.GetTileData(tile);

		float rotation = Main.instance.TilesRenderer.GetWindCycle(topLeft.X, topLeft.Y, windSway);

		if (!WorldGen.InAPlaceWithWind(topLeft.X, topLeft.Y, data.Width, data.Height))
			rotation = 0f;

		return rotation + GetHighestWindGridPushComplex(topLeft.X, topLeft.Y, data.Width, data.Height, 60, 1.26f, 3, true);
	}
}
