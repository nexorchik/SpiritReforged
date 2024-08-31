using SpiritReforged.Common.TileCommon.TileSway;
using Terraria.Audio;
using Terraria.DataStructures;

namespace SpiritReforged.Content.Savanna.Tiles;

public class ElephantGrass : ModTile, ISwayInWind
{
	private static readonly List<Point16> DrawPoints = new();
	private static bool DrawingFront;

	public override void Load() => On_Main.DrawPlayers_AfterProjectiles += (On_Main.orig_DrawPlayers_AfterProjectiles orig, Main self) =>
	{
		orig(self);

		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
		DrawingFront = true;

		for (int i = DrawPoints.Count - 1; i >= 0; i--)
		{
			var tile = Framing.GetTileSafely(DrawPoints[i]);
			if (!tile.HasTile || tile.TileType != Type)
			{
				DrawPoints.Remove(DrawPoints[i]); //If the tile is invalid, prevent DrawFront from using it
				continue;
			}

			TileSwayGlobalTile.PreDrawInWind(DrawPoints[i], Main.spriteBatch, false);
		}

		DrawingFront = false;
		Main.spriteBatch.End();
	}; //Front layer drawing

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

	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var tile = Framing.GetTileSafely(i, j);
		var data = TileObjectData.GetTileData(tile);

		if (tile.TileFrameX % (data.Width * 18) == 0 && tile.TileFrameY % (data.Height * 18) == 0 && !DrawPoints.Contains(new Point16(i, j))) //Top left only
			DrawPoints.Add(new Point16(i, j)); //Add a point so DrawFront knows where to draw the tile
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

	public void DrawInWind(int i, int j, SpriteBatch spriteBatch, Vector2 offset, float rotation, Vector2 origin)
	{
		if (DrawingFront)
			DrawFront(i, j, spriteBatch, offset, rotation, origin);
		else
			DrawBack(i, j, spriteBatch, offset, rotation, origin);
	}

	public float SetWindSway(Point16 topLeft, ref float swayMult)
	{
		swayMult = 1.5f;

		var data = TileObjectData.GetTileData(Framing.GetTileSafely(topLeft));
		float rotation = Main.instance.TilesRenderer.GetWindCycle(topLeft.X, topLeft.Y, TileSwaySystem.Instance.GrassWindCounter * 2.25f);

		if (!WorldGen.InAPlaceWithWind(topLeft.X, topLeft.Y, data.Width, data.Height))
			rotation = 0f;

		return rotation + TileSwayHelper.GetHighestWindGridPushComplex(topLeft.X, topLeft.Y, data.Width, data.Height, 20, 3f, 1, true);
	}
}