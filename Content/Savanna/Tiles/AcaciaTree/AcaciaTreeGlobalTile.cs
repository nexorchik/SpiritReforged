using Terraria.DataStructures;
using Terraria.GameContent.Drawing;

namespace SpiritReforged.Content.Savanna.Tiles.AcaciaTree;

/// <summary> Primarily handles custom drawing for acacia trees. </summary>
public class AcaciaTreeGlobalTile : GlobalTile
{
	public override void Load() => On_TileDrawing.DrawTrees += PreDrawAcaciaTrees;

	private void PreDrawAcaciaTrees(On_TileDrawing.orig_DrawTrees orig, TileDrawing self)
	{
		orig(self);

		var points = AcaciaTreeSystem.Instance.treeDrawPoints;

		for (int x = points.Count - 1; x >= 0; x--) //Iterate over all acacia tree tops
		{
			(int i, int j) = (points[x].X, points[x].Y);

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
		var position = new Vector2(i, j) * 16 + new Vector2(wavyOffset - 2, 0);

		if (drawingTop)
		{
			const int originOffsetX = -18;

			float rotation = AcaciaTreeSystem.GetAcaciaSway(i, j);
			var windOffset = new Vector2(rotation, Math.Abs(rotation)) * 2;

			spriteBatch.Draw(AcaciaTree.TopsTexture.Value, position - Main.screenPosition + windOffset, null, color, rotation * .08f, new Vector2(AcaciaTree.TopsTexture.Width() / 2 + originOffsetX, AcaciaTree.TopsTexture.Height()), 1, SpriteEffects.None, 0);

			//Ideally we don't initialize a gameplay element in drawing, but it's convenient here
			#region init platform
			var platform = new CustomPlatform(position + new Vector2(16, -AcaciaTree.TopsTexture.Height()), AcaciaTree.TopsTexture.Width(), new Vector2(i, j) * 16);
			AcaciaTreeSystem.AddPlatform(platform);
			#endregion
		}
		else
		{
			var frame = new Rectangle(tile.TileFrameX, 0, 20, 20);
			spriteBatch.Draw(AcaciaTree.Texture.Value, position - Main.screenPosition, frame, color, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
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
			AcaciaTreeSystem.AddTreeDrawPoint(new Point16(i, j));
			return false; //Don't draw the default tree
		}

		return true;
	}
}
