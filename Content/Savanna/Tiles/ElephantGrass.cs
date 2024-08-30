using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;

namespace SpiritReforged.Content.Savanna.Tiles;

public class ElephantGrass : ModTile
{
	private static readonly List<Point16> Points = new();
	private static bool DrawingFront;

	private double windSway;

	public override void Load()
	{
		On_TileDrawing.Update += (On_TileDrawing.orig_Update orig, TileDrawing self) =>
		{
			if (!Main.dedServ)
			{
				double num = Math.Abs(Main.WindForVisuals);

				num = Utils.GetLerpValue(0.08f, 1.2f, (float)num, clamped: true);
				windSway += 1.0 / 180.0 + 1.0 / 180.0 * num * 9.0; //Modified grass speed
			}

			orig(self);
		}; //Wind speed calculation

		On_Main.DrawPlayers_AfterProjectiles += (On_Main.orig_DrawPlayers_AfterProjectiles orig, Main self) =>
		{
			orig(self);

			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
			DrawingFront = true;

			for (int i = Points.Count - 1; i >= 0; i--)
				PreDraw(Points[i].X, Points[i].Y, Main.spriteBatch);

			DrawingFront = false;
			Main.spriteBatch.End();
		}; //Front layer drawing
	}

	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = false;
		Main.tileMergeDirt[Type] = false;
		Main.tileBlockLight[Type] = false;
		Main.tileFrameImportant[Type] = true;
		Main.tileCut[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
		TileObjectData.newTile.Height = 3;
		TileObjectData.newTile.CoordinateHeights = [16, 16, 18];
		TileObjectData.newTile.Origin = new(0, TileObjectData.newTile.Height);
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 0);
		TileObjectData.newTile.AnchorValidTiles = [ModContent.TileType<SavannaGrass>()];
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.RandomStyleRange = 5;
		TileObjectData.addTile(Type);

		AddMapEntry(new Color(21, 92, 19));
		DustType = DustID.Grass;
		HitSound = SoundID.Grass;
	}

	public override void NumDust(int i, int j, bool fail, ref int num) => num = 3;

	public override void NearbyEffects(int i, int j, bool closer)
	{
		//Play sounds when walking inside grass patches
		if (Main.LocalPlayer.velocity.Length() > 2f && Main.LocalPlayer.miscCounter % 3 == 0 && new Rectangle(i * 16, j * 16, 16, 16).Intersects(Main.LocalPlayer.getRect()))
			SoundEngine.PlaySound(SoundID.Grass with { Volume = .5f, PitchVariance = 1 }, Main.LocalPlayer.Center);
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var tile = Framing.GetTileSafely(i, j);
		var data = TileObjectData.GetTileData(tile);

		if (!tile.HasTile || tile.TileType != Type)
		{
			Points.Remove(new Point16(i, j)); //If the tile is invalid, prevent DrawFront from using it
			return false;
		}

		if (tile.TileFrameX % (data.Width * 18) != 0 || tile.TileFrameY % (data.Height * 18) != 0) //Top left only
			return false;

		if (!Points.Contains(new Point16(i, j)))
			Points.Add(new Point16(i, j)); //Add a point so DrawFront knows where to draw the tile

		if (!DrawingFront)
		{
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Matrix.Identity);
		}

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

				float swing = 1.5f * (data.Origin.Y - frameY + 1) / data.Height;
				float rotation = windCycle * MathHelper.Max(swing, .05f) * swing * .05f;

				var offset = origin + new Vector2(windCycle, Math.Abs(windCycle) * 2f * swing);

				if (!DrawingFront)
					DrawBack(i, j, spriteBatch, offset, rotation, origin);
				else
					DrawFront(i, j, spriteBatch, offset, rotation, origin);
			}
		}

		if (!DrawingFront)
		{
			spriteBatch.End();
			spriteBatch.Begin();
		}

		return false;
	}

	public void DrawFront(int i, int j, SpriteBatch spriteBatch, Vector2 offset, float rotation, Vector2 origin)
	{
		var tile = Framing.GetTileSafely(i, j);
		var texture = TextureAssets.Tile[Type].Value;

		int clusterOffX = -2 + (tile.TileFrameX / 18 + i) * 2 % 4; //edit
		var drawPos = new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y);
		var source = new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, (tile.TileFrameY > 18) ? 18 : 16);
		var effects = (i * (tile.TileFrameX / 18) % 2 == 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

		spriteBatch.Draw(texture, drawPos + offset + new Vector2(clusterOffX, 0), source, Lighting.GetColor(i, j), rotation, origin, 1, effects, 0f);
	}

	public void DrawBack(int i, int j, SpriteBatch spriteBatch, Vector2 offset, float rotation, Vector2 origin)
	{
		var tile = Framing.GetTileSafely(i, j);
		var texture = TextureAssets.Tile[Type].Value;

		Vector2 lightOffset = (Lighting.LegacyEngine.Mode > 1 && Main.GameZoomTarget == 1) ? Vector2.Zero : Vector2.One * 12;
		var drawPos = new Vector2((i + lightOffset.X) * 16 - (int)Main.screenPosition.X, (j + lightOffset.Y) * 16 - (int)Main.screenPosition.Y);

		for (int x = 0; x < (tile.TileFrameX / 18 + i) % 3 + 1; x++)
		{
			int clusterOffX = -2 + x * 2 % 4;
			float rotationOff = ((x + i + tile.TileFrameX / 18) % 5f - 2.5f) * .1f;

			var source = new Rectangle((tile.TileFrameX / 18 + i + x) % 5 * 18, tile.TileFrameY, 16, (tile.TileFrameY > 18) ? 18 : 16);
			var effects = (i + x * (tile.TileFrameX / 18) % 2 == 1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			var color = Color.Lerp(Color.White, Color.Goldenrod, .9f + (float)Math.Sin(i / 10f) * .1f);

			spriteBatch.Draw(texture, drawPos + offset + new Vector2(clusterOffX, 0), source, Lighting.GetColor(i, j).MultiplyRGB(color), rotation + rotationOff, origin, 1, effects, 0f);
		}
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

		return rotation + GetHighestWindGridPushComplex(topLeft.X, topLeft.Y, data.Width, data.Height, 20, 3f, 1, true);
	}
}