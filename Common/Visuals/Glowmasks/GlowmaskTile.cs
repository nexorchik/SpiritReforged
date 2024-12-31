using Mono.Cecil.Cil;
using MonoMod.Cil;
using System.Linq;
using Terraria.GameContent.Drawing;

namespace SpiritReforged.Common.Visuals.Glowmasks;

internal class GlowmaskTile : ILoadable
{
	public static Dictionary<int, GlowmaskInfo> TileIdToGlowmask = [];

	public void Load(Mod mod) => IL_TileDrawing.GetTileDrawData += InjectGlowmaskData;

	private void InjectGlowmaskData(ILContext il)
	{
		ILCursor c = new(il);

		c.Index = c.Instrs.Count - 1; //Move to the end

		var p_typeCache = c.Method.Parameters.Where(x => x.Name == "typeCache").FirstOrDefault();
		var p_glowTexture = c.Method.Parameters.Where(x => x.Name == "glowTexture").FirstOrDefault();
		var p_glowColor = c.Method.Parameters.Where(x => x.Name == "glowColor").FirstOrDefault();
		var p_glowSourceRect = c.Method.Parameters.Where(x => x.Name == "glowSourceRect").FirstOrDefault();

		if (p_typeCache == default || p_glowTexture == default || p_glowColor == default || p_glowSourceRect == default)
		{
			SpiritReforgedMod.Instance.Logger.Info($"IL edit '{nameof(InjectGlowmaskData)}' failed; all required parameters not found.");
			return;
		}

		c.Emit(OpCodes.Ldarg_S, p_typeCache); //The tile type
		c.EmitLdarg1(); //i
		c.EmitLdarg2(); //j

		c.Emit(OpCodes.Ldarg_S, p_glowTexture); //Glowmask
		c.Emit(OpCodes.Ldarg_S, p_glowColor); //Glow color
		c.Emit(OpCodes.Ldarg_S, p_glowSourceRect); //Glow frame

		c.EmitDelegate(ModifyData);
	}

	private void ModifyData(int typeCache, int i, int j, ref Texture2D glowTexture, ref Color glowColor, ref Rectangle glowSourceRect)
	{
		if (TileIdToGlowmask.TryGetValue(typeCache, out GlowmaskInfo value) && value.DrawAutomatically)
		{
			glowTexture = TileIdToGlowmask[typeCache].Glowmask.Value;
			glowColor = TileIdToGlowmask[typeCache].GetDrawColor(new Point(i, j));

			var tile = Main.tile[i, j];
			int addFrameX = 0;
			int addFrameY = 0;

			TileLoader.SetAnimationFrame(typeCache, i, j, ref addFrameX, ref addFrameY);
			var source = new Rectangle(tile.TileFrameX, tile.TileFrameY + addFrameY, 16, 16);

			glowSourceRect = source;
		}
	}

	public void Unload() { }
}
