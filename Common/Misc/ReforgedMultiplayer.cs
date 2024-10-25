using SpiritReforged.Common.PrimitiveRendering;
using SpiritReforged.Common.SimpleEntity;
using System.IO;
using Terraria.DataStructures;

namespace SpiritReforged.Common.Misc;

public static class ReforgedMultiplayer
{
	public enum MessageType : byte
	{
		SendVentPoint,
		SpawnTrail,
		SpawnSimpleEntity,
		KillSimpleEntity
    }

	public static void HandlePacket(BinaryReader reader, int whoAmI)
	{
		var id = (MessageType)reader.ReadByte();

		switch (id)
		{
			case MessageType.SendVentPoint:
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

			case MessageType.SpawnTrail:
				int proj = reader.ReadInt32();

				if (Main.netMode == NetmodeID.Server)
				{
					//If received by the server, send to all clients instead
					ModPacket packet = SpiritReforgedMod.Instance.GetPacket(MessageType.SpawnTrail, 1);
					packet.Write(proj);
					packet.Send();
					break;
				}

				if (Main.projectile[proj].ModProjectile is IManualTrailProjectile trailProj)
					trailProj.DoTrailCreation(AssetLoader.VertexTrailManager);
				break;

			case MessageType.SpawnSimpleEntity:
				int entityType = reader.ReadInt32();
				Vector2 position = reader.ReadVector2();

				if (Main.netMode == NetmodeID.Server)
				{
					ModPacket packet = SpiritReforgedMod.Instance.GetPacket(MessageType.SpawnSimpleEntity, 2);
					packet.Write(entityType);
					packet.WriteVector2(position);
					packet.Send(ignoreClient: whoAmI); //Relay to other clients
				}

				SimpleEntitySystem.NewEntity(entityType, position);
				break;

			case MessageType.KillSimpleEntity:
				int entityWhoAmI = reader.ReadInt32();

				if (Main.netMode == NetmodeID.Server)
				{
					ModPacket packet = SpiritReforgedMod.Instance.GetPacket(MessageType.KillSimpleEntity, 1);
					packet.Write(whoAmI);
					packet.Send(ignoreClient: whoAmI); //Relay to other clients
				}

				SimpleEntitySystem.entities[entityWhoAmI].Kill();
				break;
		}
	}
}
