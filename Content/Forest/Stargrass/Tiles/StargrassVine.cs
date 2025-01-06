using SpiritReforged.Common.Visuals.Glowmasks;

namespace SpiritReforged.Content.Forest.Stargrass.Tiles;

[AutoloadGlowmask("Method:Content.Forest.Stargrass.Tiles.StargrassTile Glow")] //Use Stargrass' glow
public class StargrassVine : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileBlockLight[Type] = false;
		Main.tileCut[Type] = true;
		Main.tileNoFail[Type] = true;
		Main.tileLavaDeath[Type] = true;

		TileID.Sets.IsVine[Type] = true;
		TileID.Sets.VineThreads[Type] = true;
		TileID.Sets.ReplaceTileBreakDown[Type] = true;

		AddMapEntry(new Color(24, 135, 28));

		DustType = DustID.Grass;
		HitSound = SoundID.Grass;
	}

	public override void NumDust(int i, int j, bool fail, ref int num) => num = 3;

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		if (Main.LightingEveryFrame)
			Main.instance.TilesRenderer.CrawlToTopOfVineAndAddSpecialPoint(j, i);

		return false;
	}
}