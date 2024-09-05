using Terraria.DataStructures;
using Terraria.GameContent.Drawing;

namespace SpiritReforged.Content.Savanna.Tiles.AcaciaTree;

/// <summary> Primarily handles custom drawing for acacia trees. </summary>
public class AcaciaTreeGlobalTile : GlobalTile
{
	private static Asset<Texture2D> LightTexture;

	public override void Load()
	{
		if (!Main.dedServ)
			LightTexture = Mod.Assets.Request<Texture2D>("Content/Savanna/Tiles/AcaciaTree/AcaciaTree_Tops_Light");

		On_TileDrawing.DrawTrees += PreDrawAcaciaTrees;
	}

	private void PreDrawAcaciaTrees(On_TileDrawing.orig_DrawTrees orig, TileDrawing self)
	{
		static void ApplyFullTint(Effect shader, float mult = 1)
		{
			shader.Parameters["tint"].SetValue(Color.Lerp(Color.White, Color.SlateGray, mult).ToVector3());
			shader.CurrentTechnique.Passes[0].Apply();
		}

		static void ClearTint(Effect shader) //Effectively removes the tint without removing the shader
		{
			shader.Parameters["tint"].SetValue(Color.White.ToVector3());
			shader.CurrentTechnique.Passes[0].Apply();
		}

		orig(self);

		float dayTime = (Main.time < 27000.0) ? (float)(Main.time / 54000.0) : 1f - (float)(Main.time / 54000.0);
		if (!Main.dayTime)
			dayTime = 0;

		float shadowLength = 5 * dayTime + .4f;
		var points = AcaciaTreeSystem.Instance.treeDrawPoints;

		var shader = AssetLoader.LoadedShaders["shade"];
		shader.Parameters["length"].SetValue(shadowLength % 1 * 2.5f);

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

		for (int x = points.Count - 1; x >= 0; x--) //Iterate over all acacia tree tops
		{
			(int i, int j) = (points[x].X, points[x].Y);
			ApplyFullTint(shader, dayTime);

			while (Main.tile[i, j].TileType == TileID.PalmTree) //Iterate down the whole height of the tree
			{
				#region shader settings
				int counter = j - points[x].Y;

				if (counter == (int)shadowLength)
					shader.CurrentTechnique.Passes[1].Apply(); //Applies a gradient
				else if (counter == (int)shadowLength + 1)
					ClearTint(shader);
				#endregion

				DrawAcaciaTree(i, j, Main.spriteBatch);
				j++;
			}

			ApplyFullTint(shader, dayTime);
			DrawAcaciaTree(points[x].X, points[x].Y, Main.spriteBatch, drawingTop: true);
			ClearTint(shader);
			DrawAcaciaTree(points[x].X, points[x].Y, Main.spriteBatch, drawingLight: true); //Draw the unshaded top of the tree
		}

		Main.spriteBatch.End();
		Main.spriteBatch.Begin();
	}

	private static void DrawAcaciaTree(int i, int j, SpriteBatch spriteBatch, bool drawingTop = false, bool drawingLight = false)
	{
		var tile = Main.tile[i, j];

		var color = Lighting.GetColor(i, j);
		int wavyOffset = tile.TileFrameY;
		var position = new Vector2(i, j) * 16 + new Vector2(wavyOffset - 2, 0);

		if (drawingTop || drawingLight)
		{
			const int originOffsetX = -18;

			float rotation = AcaciaTreeSystem.GetAcaciaSway(i, j);
			var windOffset = new Vector2(rotation, Math.Abs(rotation)) * 2;
			var texture = drawingLight ? LightTexture.Value : AcaciaTree.TopsTexture.Value;

			spriteBatch.Draw(texture, position - Main.screenPosition + windOffset, null, color, rotation * .08f, new Vector2(texture.Width / 2 + originOffsetX, texture.Height), 1, SpriteEffects.None, 0);

			//Ideally we don't initialize a gameplay element in drawing, but it's convenient here
			#region init platform
			var platform = new CustomPlatform(position + new Vector2(16, -texture.Height), texture.Width, new Vector2(i, j) * 16);
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
