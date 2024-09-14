using SpiritReforged.Common.TileCommon;
using SpiritReforged.Common.TileCommon.TileSway;
using SpiritReforged.Common.WorldGeneration;
using Terraria.GameContent.Metadata;

namespace SpiritReforged.Content.Forest.Stargrass.Tiles;

public class StargrassFlowers : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileCut[Type] = true;
		Main.tileSolid[Type] = false;
		Main.tileLighted[Type] = true;

		DustType = DustID.Grass;
		HitSound = SoundID.Grass;

		TileMaterials.SetForTileId(Type, TileMaterials._materialsByName["Plant"]);

		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.LavaDeath = true;
		TileObjectData.newTile.WaterDeath = false;
		TileObjectData.newTile.CoordinatePadding = 2;
		TileObjectData.newTile.CoordinateWidth = 16;
		TileObjectData.newTile.CoordinateHeights = [20];
		TileObjectData.newTile.DrawYOffset = -2;
		TileObjectData.newTile.Style = 0;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.UsesCustomCanPlace = true;
		TileObjectData.newTile.AnchorValidTiles = [ModContent.TileType<StargrassTile>()];
		TileObjectData.newTile.AnchorAlternateTiles = [TileID.ClayPot, TileID.PlanterBox];

		for (int i = 0; i < 25; i++)
		{
			TileObjectData.newSubTile.CopyFrom(TileObjectData.newTile);
			TileObjectData.addSubTile(TileObjectData.newSubTile.Style);
		}

		TileObjectData.addTile(Type);
	}

	public override void NumDust(int i, int j, bool fail, ref int num) => num = 2;

	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
	{
		int frame = Main.tile[i, j].TileFrameX / 18;
		if (frame >= 6)
			(r, g, b) = (0.025f, 0.1f, 0.25f);
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		TileSwaySystem.DrawGrassSway(spriteBatch, TextureAssets.Tile[Type].Value, i, j, Lighting.GetColor(i, j));
		return false;
	}

	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		const float MinBrightness = 0.4f;
		const float MaxDist = 140 * 140;

		float dist = Main.player[Player.FindClosest(new Vector2(i, j) * 16, 16, 16)].DistanceSQ(new Vector2(i, j) * 16 + new Vector2(8));
		float strength = MinBrightness;

		if (dist < MaxDist)
			strength = MathHelper.Lerp(MinBrightness, 1f, 1 - dist / MaxDist);

		TileSwaySystem.DrawGrassSway(spriteBatch, Texture + "_Glow", i, j, StargrassTile.Glow(new Point(i, j)) * strength);
	}
}