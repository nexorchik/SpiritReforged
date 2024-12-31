using SpiritReforged.Common.Visuals.Glowmasks;

namespace SpiritReforged.Content.Forest.Stargrass.Tiles;

[AutoloadGlowmask("Method:Content.Forest.Stargrass.Tiles.StargrassTile Glow")] //Use Stargrass' glow
internal class StargrassVine : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = false;
		Main.tileCut[Type] = true;
		Main.tileMergeDirt[Type] = false;
		Main.tileBlockLight[Type] = false;

		TileID.Sets.IsVine[Type] = true;
		TileID.Sets.VineThreads[Type] = true;

		AddMapEntry(new Color(24, 135, 28));

		DustType = DustID.Grass;
		HitSound = SoundID.Grass;
	}

	public override void NumDust(int i, int j, bool fail, ref int num) => num = 3;

	public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
	{
		if (!Main.tile[i, j - 1].HasTile)
			WorldGen.KillTile(i, j);

		return true;
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		if (Main.LightingEveryFrame)
			Main.instance.TilesRenderer.CrawlToTopOfVineAndAddSpecialPoint(j, i);

		return false;
	}
}