using SpiritReforged.Common.TileCommon;
using SpiritReforged.Common.WorldGeneration;

namespace SpiritReforged.Content.Forest.Stargrass.Tiles;

internal class StargrassTile : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileMerge[Type][Type] = true;
		Main.tileBlockLight[Type] = true;
		Main.tileLighted[Type] = true;
		Main.tileMerge[TileID.Dirt][Type] = true;
		Main.tileMerge[Type][TileID.Dirt] = true;
		Main.tileMerge[TileID.Grass][Type] = true;
		Main.tileMerge[Type][TileID.Grass] = true;

		TileID.Sets.Grass[Type] = true;
		TileID.Sets.Conversion.Grass[Type] = true;
		TileID.Sets.CanBeDugByShovel[Type] = true;

		AddMapEntry(new Color(28, 216, 151));
		DustType = DustID.Flare_Blue;
	}

	public override bool CanExplode(int i, int j)
	{
		WorldGen.KillTile(i, j, false, false, true); //Makes the tile completely go away instead of reverting to dirt
		return true;
	}

	public override void RandomUpdate(int i, int j)
	{
		if (!Framing.GetTileSafely(i, j - 1).HasTile && Main.rand.NextBool(4))
		{
			int style = Main.rand.Next(12);
			WorldGen.PlaceObject(i, j - 1, ModContent.TileType<StargrassFlowers>(), true, style);
			NetMessage.SendObjectPlacement(-1, i, j - 1, ModContent.TileType<StargrassFlowers>(), style, 0, -1, -1);
		}

		if (SpreadHelper.Spread(i, j, Type, 4, TileID.Dirt) && Main.netMode != NetmodeID.SinglePlayer)
			NetMessage.SendTileSquare(-1, i, j, 3, TileChangeType.None);
	}

	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var tex = ModContent.Request<Texture2D>(Texture + "_Glow", AssetRequestMode.AsyncLoad).Value;
		Color colour = Color.White * MathHelper.Lerp(0.2f, 1f, (float)((Math.Sin(NoiseSystem.Perlin(i * 1.2f, j * 0.2f) * 3f + Main.GlobalTimeWrappedHourly * 1.3f) + 1f) * 0.5f));
		this.DrawSlopedGlowMask(i, j, tex, colour, Vector2.Zero, false);
	}

	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) => (r, g, b) = (0.05f, 0.2f, 0.5f);

	public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
	{
		if (!fail)
		{
			fail = true;
			Framing.GetTileSafely(i, j).TileType = TileID.Dirt;
		}
	}
}
