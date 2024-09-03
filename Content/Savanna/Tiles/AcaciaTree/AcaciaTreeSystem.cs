using SpiritReforged.Common.TileCommon.TileSway;
using Terraria.DataStructures;

namespace SpiritReforged.Content.Savanna.Tiles.AcaciaTree;

public class AcaciaTreeSystem : ModSystem
{
	public static float GetAcaciaSway(int i, int j) => Main.instance.TilesRenderer.GetWindCycle(i, j, TileSwaySystem.Instance.TreeWindCounter) * .4f;

	public static AcaciaTreeSystem Instance => ModContent.GetInstance<AcaciaTreeSystem>();

	public readonly List<Point16> treeTopPoints = [];

	public static void AddTopPoint(Point16 point)
	{
		if (Instance.treeTopPoints.Contains(point))
			return;

		Instance.treeTopPoints.Add(point);

		if (Main.netMode != NetmodeID.SinglePlayer)
		{
			ModPacket packet = SpiritReforgedMod.Instance.GetPacket(Common.Misc.ReforgedMultiplayer.MessageType.SendTreeTop, 3);
			packet.Write(point.X);
			packet.Write(point.Y);
			packet.Write(false);
			packet.Send(); //Used for platform logic in addition to acacia tree drawing, so send to server
		}
	}
}
