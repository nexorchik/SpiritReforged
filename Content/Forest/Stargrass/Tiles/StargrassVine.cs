using SpiritReforged.Common.TileCommon;

namespace SpiritReforged.Content.Forest.Stargrass.Tiles;

internal class StargrassVine : ModTile
{
	private static Asset<Texture2D> _glow = null;

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

		_glow = ModContent.Request<Texture2D>(Texture + "_Glow");
	}

	public override void NumDust(int i, int j, bool fail, ref int num) => num = 3;

	public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
	{
		if (Main.tile[i, j + 1].TileType == Type)
			WorldGen.KillTile(i, j + 1, false, false, true);
	}

	public override void NearbyEffects(int i, int j, bool closer)
	{
		if (!Main.tile[i, j - 1].HasTile)
			WorldGen.KillTile(i, j);
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		Tile t = Framing.GetTileSafely(i, j);
		float sine = (float)Math.Sin((i + j) * MathHelper.ToRadians(20) + Main.GameUpdateCount * 0.02f) * 1f;

		if (Main.tile[i, j - 1].TileType != Type)
			sine = 0;
		else if (Main.tile[i, j - 2].TileType != Type)
			sine *= 0.33f;
		else if (Main.tile[i, j - 3].TileType != Type)
			sine *= 0.67f;

		var source = new Rectangle(t.TileFrameX, t.TileFrameY, 16, 16);
		Vector2 position = this.DrawPosition(i, j, new Vector2(sine, 0));
		spriteBatch.Draw(TextureAssets.Tile[Type].Value, position, source, Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
		spriteBatch.Draw(_glow.Value, position, source, StargrassTile.Glow(new Point(i, j)), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
		return false;
	}
}