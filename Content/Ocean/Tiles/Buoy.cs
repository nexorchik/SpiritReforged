using SpiritReforged.Common.TileCommon;
using SpiritReforged.Common.TileCommon.TileSway;
using Terraria.Audio;
using Terraria.DataStructures;

namespace SpiritReforged.Content.Ocean.Tiles;

public class Buoy : ModTile, IAutoloadTileItem, ISwayInWind
{
	public Asset<Texture2D> GlowTexture { get; private set; }

	public static int GetWaterHeight(int i, int j)
	{
		var tile = Framing.GetTileSafely(i, j);
		var data = TileObjectData.GetTileData(tile);

		(i, j) = (i + (data.Width - 1 - tile.TileFrameX / 18), j + (data.Height - 1 - tile.TileFrameY / 18));
		return (int)(Framing.GetTileSafely(i, j).LiquidAmount / 16f);
	}

	public override void Load()
	{
		if (!Main.dedServ)
			GlowTexture = ModContent.Request<Texture2D>(Texture + "_Glow");
	}

	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileSolid[Type] = false;
		Main.tileNoAttach[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.LavaDeath = true;
		TileObjectData.newTile.WaterDeath = false;
		TileObjectData.newTile.CoordinatePadding = 2;
		TileObjectData.newTile.CoordinateWidth = 16;
		TileObjectData.newTile.CoordinateHeights = [24];
		TileObjectData.newTile.DrawYOffset = -6;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.WaterPlacement = LiquidPlacement.OnlyInLiquid;
		TileObjectData.newTile.UsesCustomCanPlace = true;
		TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
		TileObjectData.addTile(Type);

		DustType = DustID.Iron;
		HitSound = SoundID.Dig;

		AddMapEntry(new Color(250, 67, 74));
	}

	public override void PlaceInWorld(int i, int j, Item item)
	{
		SoundEngine.PlaySound(SoundID.Splash, new Vector2(i, j) * 16);
		var data = TileObjectData.GetTileData(Type, 0);

		for (int x = 0; x < 10 * data.Width; x++)
			Dust.NewDustDirect(new Vector2(i, j) * 16, data.Width * 16, 16, Dust.dustWater(), 0, 0, 150, new Color(), Main.rand.NextFloat(1f, 2f)).velocity = Vector2.UnitY * -Main.rand.NextFloat();
	}

	public override bool CanPlace(int i, int j)
	{
		var data = TileObjectData.GetTileData(Type, 0);

		for (int x = 0; x < data.Width; x++)
		{
			Tile tile = Framing.GetTileSafely(i + x, j);
			Tile above = Framing.GetTileSafely(i + x, j - 1);

			if (tile.LiquidAmount < 50 || tile.LiquidType == LiquidID.Lava || WorldGen.SolidOrSlopedTile(above) || above.LiquidAmount > 0)
				return false;
		}

		return true;
	}

	public override void NumDust(int i, int j, bool fail, ref int num) => num = 2;

	public void DrawInWind(int i, int j, SpriteBatch spriteBatch, Vector2 offset, float rotation, Vector2 origin)
	{
		var tile = Framing.GetTileSafely(i, j);
		var texture = TextureAssets.Tile[Type].Value;

		var drawPos = new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y);
		var source = new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, TileObjectData.GetTileData(Type, 0).CoordinateHeights[tile.TileFrameY / 18]);

		offset.Y += TileObjectData.GetTileData(Type, 0).DrawYOffset - GetWaterHeight(i, j);

		spriteBatch.Draw(texture, drawPos + offset, source, Lighting.GetColor(i, j), rotation, origin, 1, SpriteEffects.None, 0f);

		var glowColor = Color.White * (1f - (float)Math.Sin((Main.timeForVisualEffects + i) / 10f) * .3f);
		spriteBatch.Draw(GlowTexture.Value, drawPos + offset, source, glowColor, rotation, origin, 1, SpriteEffects.None, 0f);
	}

	public void ModifyRotation(int i, int j, ref float rotation)
	{
		var tile = Framing.GetTileSafely(i, j);
		(i, j) = (i - tile.TileFrameX / 18, j - tile.TileFrameY / 18);

		//Remove per-segment sway
		rotation = SetWindSway(new Point16(i, j)) * .1f;
	}

	public float SetWindSway(Point16 topLeft)
	{
		//This tile ignores whether the current location has wind
		var data = TileObjectData.GetTileData(Framing.GetTileSafely(topLeft));
		float rotation = Main.instance.TilesRenderer.GetWindCycle(topLeft.X, topLeft.Y, TileSwaySystem.Instance.SunflowerWindCounter);

		return rotation + TileSwayHelper.GetHighestWindGridPushComplex(topLeft.X, topLeft.Y, data.Width, data.Height, 60, 2f, 3, true);
	}
}