namespace SpiritReforged.Content.Forest.Botanist.Items;

internal class BotanistGlobalTile : GlobalTile
{
	private readonly int[] VanillaHerbs = [TileID.BloomingHerbs, TileID.MatureHerbs];
	private static int[] SpiritHerbs => [ModContent.TileType<Cloudstalk.Items.CloudstalkTile>()];

	public List<int> AllHerbs = null;

	public override bool PreDraw(int i, int j, int type, SpriteBatch spriteBatch)
	{
		if (AllHerbs is null)
		{
			var herbs = new List<int>();
			herbs.AddRange(SpiritHerbs);
			herbs.AddRange(VanillaHerbs);
			AllHerbs = herbs;
		}

		if (AllHerbs.Contains(type) && Main.LocalPlayer.GetModPlayer<BotanistPlayer>().active)
		{
			Tile tile = Main.tile[i, j];
			Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
			float darkness = (1.2f - Lighting.Brightness(i, j)) / 1.2f;

			spriteBatch.Draw(TextureAssets.Tile[type].Value, new Vector2(i, j) * 16 - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 20), Color.Lerp(Lighting.GetColor(i, j), Color.DeepPink, darkness));
			return false;
		}

		return true;
	}
}
