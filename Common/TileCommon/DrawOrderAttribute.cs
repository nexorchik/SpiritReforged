using MonoMod.Cil;
using System.Linq;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using static SpiritReforged.Common.TileCommon.DrawOrderAttribute;

namespace SpiritReforged.Common.TileCommon;

[AttributeUsage(AttributeTargets.Class)]
public class DrawOrderAttribute(params Layer[] layers) : Attribute
{
	public Layer[] Layers { get; private set; }  = layers;

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
	public static readonly Dictionary<Point16, Layer[]> specialDrawPoints = [];
	/// <summary> Used in conjunction with <see cref="DrawOrderAttribute"/> to tell whether a tile is drawing as a result of the attribute and on what layer. </summary>
	internal static Layer order = Layer.Default;

	public void Load(Mod mod)
	{
		static void Draw(Layer layer)
		{
			order = layer;
			var above = specialDrawPoints.Where(x => x.Value.Contains(order));
			foreach (var set in above)
			{
				var p = set.Key;
				TileLoader.PreDraw(p.X, p.Y, Framing.GetTileSafely(p.X, p.Y).TileType, Main.spriteBatch);
			}

			order = Layer.Default;
		}

		On_TileDrawing.PreDrawTiles += ClearDrawPoints;
		IL_Main.DoDraw_Tiles_Solid += (ILContext il) =>
		{
			var c = new ILCursor(il);
			for (int i = 0; i < 2; i++)
			{
				if (!c.TryGotoNext(x => x.MatchCallvirt<SpriteBatch>("End")))
				{
					SpiritReforgedMod.Instance.Logger.Debug("Failed goto SpriteBatch.End; Index: " + i);
					return;
				}
			}

			c.EmitDelegate(() => Draw(Layer.Solid)); //Emit a delegate so we can draw just before the spritebatch ends
		};

		On_Main.DoDraw_Tiles_NonSolid += (On_Main.orig_DoDraw_Tiles_NonSolid orig, Main self) =>
		{
			orig(self);
			Draw(Layer.NonSolid);
		};

		On_Main.DrawPlayers_AfterProjectiles += (On_Main.orig_DrawPlayers_AfterProjectiles orig, Main self) =>
		{
			orig(self);

			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
			Draw(Layer.OverPlayers);
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

		if (DrawOrderHandler.order == Layer.Default)
		{
			DrawOrderHandler.specialDrawPoints.Add(new Point16(i, j), tag.Layers);
			return tag.Layers.Contains(Layer.Default);
		}

		return true;
	}
}
