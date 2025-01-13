using SpiritReforged.Common.TileCommon.TileSway;
using static Terraria.GameContent.Drawing.TileDrawing;

namespace SpiritReforged.Content.Savanna.Tiles;

public class SavannaVine : ModTile, ISwayTile
{
	public int Style => (int)TileCounterType.Vine;

	public override void SetStaticDefaults()
	{
		Main.tileBlockLight[Type] = true;
		Main.tileCut[Type] = true;
		Main.tileNoFail[Type] = true;
		Main.tileLavaDeath[Type] = true;

		TileID.Sets.IsVine[Type] = true;
		TileID.Sets.ReplaceTileBreakDown[Type] = true;

		AddMapEntry(new Color(24, 135, 28));

		DustType = DustID.JunglePlants;
		HitSound = SoundID.Grass;
	}

	public override void NumDust(int i, int j, bool fail, ref int num) => num = 3;
}