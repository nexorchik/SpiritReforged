using SpiritReforged.Common.TileCommon;
using SpiritReforged.Content.Forest.Stargrass.Tiles;

namespace SpiritReforged.Content.Forest.Stargrass;

internal class StargrassTreeGlowEffects : GlobalTile
{
	public override void NearbyEffects(int i, int j, int type, bool closer)
	{
		if (IsStargrassTree(i, j, type))
		{
			Lighting.AddLight(new Vector2(i, j).ToWorldCoordinates(), new Vector3(0.2f, 0.2f, 0.5f));
		}
	}

	public override void PostDraw(int i, int j, int type, SpriteBatch spriteBatch)
	{
		if (IsStargrassTree(i, j, type))
		{
			Texture2D tex = TextureAssets.Tile[type].Value;
			Tile tile = Main.tile[i, j];
			var frame = new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16);

			spriteBatch.Draw(tex, TileExtensions.DrawPosition(i, j), frame, Color.White);
		}
	}

	private static bool IsStargrassTree(int i, int j, int type)
	{
		if (type == TileID.Trees)
		{
			while (Main.tile[i, j].TileType == TileID.Trees)
			{
				j++;
			}

			if (Main.tile[i, j].TileType == ModContent.TileType<StargrassTile>())
			{
				return true;
			}
		}

		return false;
	}
}
