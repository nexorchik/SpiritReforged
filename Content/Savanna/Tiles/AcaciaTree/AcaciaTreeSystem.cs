using SpiritReforged.Common.TileCommon.TileSway;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;

namespace SpiritReforged.Content.Savanna.Tiles.AcaciaTree;

public class AcaciaTreeSystem : ModSystem
{
	/// <summary> How much acacia tree tops sway in the wind. Used by the client for drawing and platform logic. </summary>
	public static float GetAcaciaSway(int i, int j) => Main.instance.TilesRenderer.GetWindCycle(i, j, TileSwaySystem.Instance.TreeWindCounter) * .4f;

	public static AcaciaTreeSystem Instance => ModContent.GetInstance<AcaciaTreeSystem>();

	public readonly List<Point16> treeDrawPoints = [];
	public readonly List<CustomPlatform> platforms = [];

	public override void Load() => On_TileDrawing.PreDrawTiles += (On_TileDrawing.orig_PreDrawTiles orig, TileDrawing self, bool solidLayer, bool forRenderTargets, bool intoRenderTargets) =>
	{
		orig(self, solidLayer, forRenderTargets, intoRenderTargets);

		bool flag = intoRenderTargets || Lighting.UpdateEveryFrame;
		if (!solidLayer && flag)
			Instance.treeDrawPoints.Clear();

	}; //Reset our treeDrawPoints

	public static void AddTreeDrawPoint(Point16 point)
	{
		if (!Instance.treeDrawPoints.Contains(point))
			Instance.treeDrawPoints.Add(point);
	}

	/// <summary> Add a platform to <see cref="platforms"/> with a duplicate check and handle syncing with the server. </summary>
	public static void AddPlatform(CustomPlatform platform)
	{
		if (Instance.platforms.Contains(platform))
			return;

		Instance.platforms.Add(platform);

		if (Main.netMode != NetmodeID.SinglePlayer && !Main.dedServ) //We don't want the server to sync to clients
		{
			ModPacket packet = SpiritReforgedMod.Instance.GetPacket(Common.Misc.ReforgedMultiplayer.MessageType.SendPlatform, 3);
			packet.WriteVector2(platform.center);
			packet.Write(platform.width);
			packet.Write(false);
			packet.Send();
		}
	}
}

public struct CustomPlatform(Vector2 center, int width, Vector2? origin = null)
{
	public Vector2 center = center;
	public int width = width;
	public const int height = 8;

	public Vector2 origin = (origin is null) ? center : origin.Value;

	public readonly Rectangle GetRect() => new((int)center.X - width / 2, (int)center.Y - height / 2, width, height);
}