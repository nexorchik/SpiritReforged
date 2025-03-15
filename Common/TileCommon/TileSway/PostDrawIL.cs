/*using Mono.Cecil.Cil;
using MonoMod.Cil;
using System.Linq;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;

namespace SpiritReforged.Common.TileCommon.TileSway;

/// <summary> Allows tile sway from vanilla methods to be modified by <see cref="ISwayTile.Physics"/>. </summary>
internal class PostDrawIL : ILoadable
{
	public void Load(Mod mod)
	{
		IL_TileDrawing.DrawMultiTileGrassInWind += EditMultiTile;
		IL_TileDrawing.DrawMultiTileVinesInWind += EditMultiTile;
	}

	private static void EditMultiTile(ILContext il)
	{
		ILCursor c = new(il);

		var p_topLeftY = il.Method.Parameters.Where(x => x.Name == "topLeftY").FirstOrDefault();

		if (p_topLeftY == default)
		{
			SpiritReforgedMod.Instance.Logger.Info($"IL edit '{nameof(EditMultiTile)}' failed; required parameter not found.");
			return;
		}

		c.GotoNext(MoveType.After, x => x.MatchCall<TileDrawing>("GetWindCycle"));

		c.EmitLdarg0();
		c.EmitLdarg3();
		c.Emit(OpCodes.Ldarg_S, p_topLeftY);
		c.EmitDelegate(ModifyWind);
	}

	private static void ModifyWind(ref float wind, int topLeftX, int topLeftY)
	{
		int type = Main.tile[topLeftX, topLeftY].TileType;
		if (TileLoader.GetTile(type) is ISwayTile sway)
		{
			float physics = sway.Physics(new Point16(topLeftX, topLeftY));
			if (physics != 0)
				wind = physics;
		}
	}

	public void Unload() { }
}*/