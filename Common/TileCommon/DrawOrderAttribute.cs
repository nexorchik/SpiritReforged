using MonoMod.Cil;
using SpiritReforged.Common.TileCommon.TileSway;
using System.Linq;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using static SpiritReforged.Common.TileCommon.DrawOrderAttribute;

namespace SpiritReforged.Common.TileCommon;

[AttributeUsage(AttributeTargets.Class)]
internal class DrawOrderAttribute(params Layer[] layers) : Attribute
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

internal class DrawOrderSystem : ModSystem
{
	private static readonly Dictionary<int, Layer[]> drawOrderTypes = []; //Store tile types & defined layer pairs
	public static readonly Dictionary<Point16, Layer[]> specialDrawPoints = []; //Store drawing coordinates with layer data so our detours know where to draw

	/// <summary> Used in conjunction with <see cref="DrawOrderAttribute"/> to tell whether a tile is drawing as a result of the attribute and on what layer. </summary>
	internal static Layer Order = Layer.Default;

	public static bool TryGetLayers(int type, out Layer[] layers)
	{
		if (drawOrderTypes.TryGetValue(type, out Layer[] value))
		{
			layers = value;
			return true;
		}

		layers = null;
		return false;
	}

	public override void Load()
	{
		static void Draw(Layer layer)
		{
			Order = layer;
			var above = specialDrawPoints.Where(x => x.Value.Contains(Order));
			foreach (var set in above)
			{
				var p = set.Key;
				TileLoader.PreDraw(p.X, p.Y, Framing.GetTileSafely(p.X, p.Y).TileType, Main.spriteBatch);
			}

			Order = Layer.Default;
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

	public override void PostSetupContent()
	{
		var modTiles = ModContent.GetContent<ModTile>();
		foreach (var tile in modTiles)
		{
			var tag = (DrawOrderAttribute)Attribute.GetCustomAttribute(tile.GetType(), typeof(DrawOrderAttribute), false);

			if (tag is not null)
				drawOrderTypes.Add(tile.Type, tag.Layers);
			else if (tile is ISwayTile sway && sway.Style == -1) //If no layers are defined for this ISwayTile, automatically add a valid layer for sway drawing
				drawOrderTypes.Add(tile.Type, [Layer.NonSolid]);
		}
	}
}

internal class DrawOrderGlobalTile : GlobalTile
{
	public override bool PreDraw(int i, int j, int type, SpriteBatch spriteBatch)
	{
		if (!DrawOrderSystem.TryGetLayers(type, out var value))
			return true;

		if (DrawOrderSystem.Order == Layer.Default)
		{
			DrawOrderSystem.specialDrawPoints.Add(new Point16(i, j), value);
			return value.Contains(Layer.Default);
		}

		return true;
	}
}
