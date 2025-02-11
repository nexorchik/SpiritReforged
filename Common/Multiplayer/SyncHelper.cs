using System.IO;
using Terraria.DataStructures;

namespace SpiritReforged.Common.Multiplayer;

internal static class SyncHelper
{
	public static void WritePoint16(this ModPacket packet, Point16 point)
	{
		packet.Write(point.X);
		packet.Write(point.Y);
	}

	public static Point16 ReadPoint16(this BinaryReader reader) => new(reader.ReadInt16(), reader.ReadInt16());
}
