using Terraria.DataStructures;
using Terraria.GameContent.Drawing;

namespace SpiritReforged.Content.Savanna.Tiles.AcaciaTree;

public class AcaciaTreeGlobalTile : GlobalTile
{
	public override void Load() => On_TileDrawing.DrawTrees += PreDrawAcaciaTrees;

	private void PreDrawAcaciaTrees(On_TileDrawing.orig_DrawTrees orig, TileDrawing self)
	{
		orig(self);

		var points = AcaciaTreeSystem.Instance.treeTopPoints;

		for (int x = points.Count - 1; x >= 0; x--) //Iterate over all acacia tree tops
		{
			(int i, int j) = (points[x].X, points[x].Y);

			if (Main.tile[i, j].TileType != TileID.PalmTree)
			{
				if (points.Remove(points[x]) && Main.netMode != NetmodeID.SinglePlayer)
				{
					ModPacket packet = SpiritReforgedMod.Instance.GetPacket(Common.Misc.ReforgedMultiplayer.MessageType.SendTreeTop, 3);
					packet.Write(points[x].X);
					packet.Write(points[x].Y);
					packet.Write(true);
					packet.Send(); //Notify the server for platform logic
				}

				continue;
			}

			while (Main.tile[i, j].TileType == TileID.PalmTree) //Iterate down the whole height of the tree
			{
				DrawAcaciaTree(i, j, Main.spriteBatch);
				j++;
			}

			DrawAcaciaTree(points[x].X, points[x].Y, Main.spriteBatch, drawingTop: true);
		} //Draw the trees normally
	}

	private static void DrawAcaciaTree(int i, int j, SpriteBatch spriteBatch, bool drawingTop = false)
	{
		var tile = Main.tile[i, j];

		var color = Lighting.GetColor(i, j);
		int wavyOffset = tile.TileFrameY;

		if (drawingTop)
		{
			int originOffsetX = -18;
			float rotation = AcaciaTreeSystem.GetAcaciaSway(i, j);
			var windOffset = new Vector2(rotation, Math.Abs(rotation)) * 2;

			spriteBatch.Draw(AcaciaTree.TopsTexture.Value, new Vector2(i, j) * 16 - Main.screenPosition + new Vector2(wavyOffset - 2, 0) + windOffset, null, color, rotation * .08f, new Vector2(AcaciaTree.TopsTexture.Width() / 2 + originOffsetX, AcaciaTree.TopsTexture.Height()), 1, SpriteEffects.None, 0);
		}
		else
		{
			var frame = new Rectangle(tile.TileFrameX, 0, 20, 20);
			spriteBatch.Draw(AcaciaTree.Texture.Value, new Vector2(i, j) * 16 - Main.screenPosition + new Vector2(wavyOffset - 2, 0), frame, color, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
		}
	}

	public override bool PreDraw(int i, int j, int type, SpriteBatch spriteBatch)
	{
		static bool IsAcaciaTree(int i, int j)
		{
			int y = j;
			while (true)
			{
				if (Main.tile[i, y].HasTile && Main.tile[i, y].TileType == TileID.PalmTree)
					y++;
				else
					return Main.tile[i, y].HasTile && Main.tile[i, y].TileType == ModContent.TileType<SavannaGrass>();
			}
		}

		if (type == TileID.PalmTree && IsAcaciaTree(i, j) && (!Main.tile[i, j - 1].HasTile || Main.tile[i, j - 1].TileType != TileID.PalmTree))
		{
			AcaciaTreeSystem.AddTopPoint(new Point16(i, j));
			return false; //Don't draw the default tree
		}

		return true;
	}
}
