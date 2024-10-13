using SpiritReforged.Common.PrimitiveRendering;
using SpiritReforged.Common.SimpleEntity;
using SpiritReforged.Content.Ocean.Hydrothermal.Tiles;
using System.IO;

namespace SpiritReforged.Common.Misc;

public static class ReforgedMultiplayer
{
	public enum MessageType : byte
	{
		SendVentEruption,
		SpawnTrail,
		SpawnSimpleEntity,
		KillSimpleEntity
	}

	public static void HandlePacket(BinaryReader reader, int whoAmI)
	{
		var id = (MessageType)reader.ReadByte();

		switch (id)
		{
			case MessageType.SendVentEruption: //Normally should only be recieved by clients
				short i = reader.ReadInt16();
				short j = reader.ReadInt16();

				HydrothermalVent.Erupt(i, j);
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
