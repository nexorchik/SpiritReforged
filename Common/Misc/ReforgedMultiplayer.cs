using System.IO;
using Terraria.DataStructures;

namespace SpiritReforged.Common.Misc;

public static class ReforgedMultiplayer
{
	public enum MessageType : byte
	{
		SendVentPoint,
		SendTreeTop
	}

	public static void HandlePacket(BinaryReader reader, int whoAmI)
	{
		var id = (MessageType)reader.ReadByte();

		switch (id)
		{
			case MessageType.SendVentPoint:
				{
					int i = reader.ReadInt32();
					int j = reader.ReadInt32();

					if (Main.netMode == NetmodeID.Server)
					{
						ModPacket packet = SpiritReforgedMod.Instance.GetPacket(MessageType.SendVentPoint, 2);
						packet.Write(i);
						packet.Write(j);
						packet.Send(ignoreClient: whoAmI); //Relay to other clients
					}

					Content.Ocean.Tiles.VentSystem.VentPoints.Add(new Point16(i, j));
					break;
				}
			case MessageType.SendTreeTop:
				{
					//Should only be recieved by the server
					int i = reader.ReadInt32();
					int j = reader.ReadInt32();
					bool remove = reader.ReadBoolean();

					var points = Content.Savanna.Tiles.AcaciaTree.AcaciaTreeSystem.Instance.treeTopPoints;

					if (remove)
						points.Remove(new Point16(i, j));
					else
						points.Add(new Point16(i, j));

					break;
				}
		}
	}
}
