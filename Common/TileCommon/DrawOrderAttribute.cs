using System.Linq;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;

namespace SpiritReforged.Common.TileCommon;

[AttributeUsage(AttributeTargets.Class)]
public class DrawOrderAttribute(DrawOrderAttribute.Layer layer, bool drawDefault = false) : Attribute
{
	public Layer layer = layer;
	public bool drawDefault = drawDefault;

	public enum Layer : byte //Draws above the given layer
	{
		NonSolid,
		Solid,
		OverPlayers,
		Default
	}
}

public class DrawOrderHandler : ILoadable
{
	public static readonly Dictionary<Point16, DrawOrderAttribute.Layer> specialDrawPoints = [];

	/// <summary> Used in conjunction with <see cref="DrawOrderAttribute"/> to tell whether a tile is drawing as a result of the attribute. <br/>
	/// Especially useful if <see cref="DrawOrderAttribute.drawDefault"/> is true. </summary>
	internal static bool drawingInOrder = false;

	public void Load(Mod mod)
	{
		static void Draw(Point16 p)
			=> TileLoader.PreDraw(p.X, p.Y, Framing.GetTileSafely(p.X, p.Y).TileType, Main.spriteBatch);

		On_TileDrawing.PreDrawTiles += ClearDrawPoints;
		On_Main.DoDraw_Tiles_Solid += (On_Main.orig_DoDraw_Tiles_Solid orig, Main self) =>
		{
			orig(self);

			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

			drawingInOrder = true;
			var above = specialDrawPoints.Where(x => x.Value is DrawOrderAttribute.Layer.Solid);
			foreach (var set in above)
				Draw(set.Key);

			drawingInOrder = false;
			Main.spriteBatch.End();
		};

		On_Main.DoDraw_Tiles_NonSolid += (On_Main.orig_DoDraw_Tiles_NonSolid orig, Main self) =>
		{
			orig(self);

			drawingInOrder = true;
			var above = specialDrawPoints.Where(x => x.Value is DrawOrderAttribute.Layer.NonSolid);
			foreach (var set in above)
				Draw(set.Key);

			drawingInOrder = false;
		};

		On_Main.DrawPlayers_AfterProjectiles += (On_Main.orig_DrawPlayers_AfterProjectiles orig, Main self) =>
		{
			orig(self);

			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
			
			drawingInOrder = true;
			var above = specialDrawPoints.Where(x => x.Value is DrawOrderAttribute.Layer.OverPlayers);
			foreach (var set in above)
				Draw(set.Key);

			drawingInOrder = false;
			Main.spriteBatch.End();
		};
	}

	private void ClearDrawPoints(On_TileDrawing.orig_PreDrawTiles orig, TileDrawing self, bool solidLayer, bool forRenderTargets, bool intoRenderTargets)
	{
		orig(self, solidLayer, forRenderTargets, intoRenderTargets);

		bool flag = intoRenderTargets || Lighting.UpdateEveryFrame;
		if (!solidLayer && flag)
			specialDrawPoints.Clear();
	}

	public void Unload() { }
}

public class DrawOrderGlobalTile : GlobalTile
{
	private static DrawOrderAttribute Tag(int type)
	{
		if (TileLoader.GetTile(type) is ModTile mTile)
			return (DrawOrderAttribute)Attribute.GetCustomAttribute(mTile.GetType(), typeof(DrawOrderAttribute));

		return null;
	}

	public override bool PreDraw(int i, int j, int type, SpriteBatch spriteBatch)
	{
		if (Tag(type) is not DrawOrderAttribute tag)
			return true;

		if (!DrawOrderHandler.drawingInOrder)
		{
			DrawOrderHandler.specialDrawPoints.Add(new Point16(i, j), tag.layer);
			return tag.drawDefault;
		}

		return true;
	}
}
