using SpiritReforged.Common.ItemCommon.Backpacks;
using SpiritReforged.Common.ItemCommon.Pins;
using SpiritReforged.Common.MapCommon;
using SpiritReforged.Common.NPCCommon;
using SpiritReforged.Common.PrimitiveRendering;
using SpiritReforged.Common.SimpleEntity;
using SpiritReforged.Common.WorldGeneration;
using SpiritReforged.Content.Forest.Safekeeper;
using SpiritReforged.Content.Ocean.Hydrothermal.Tiles;
using SpiritReforged.Content.Ocean.Items.Reefhunter.CascadeArmor;
using System.IO;
using Terraria.DataStructures;

namespace SpiritReforged.Common.Misc;

public static class ReforgedMultiplayer
{
	public enum MessageType : byte
	{
		SendVentEruption,
		SpawnTrail,
		SpawnSimpleEntity,
		KillSimpleEntity,
		AddPin,
		RemovePin,
		AskForPointsOfInterest,
		RemovePointOfInterest,
		RevealMap,
		MagmaGlowPoint,
		PackVisibility,
		SummonTag,
		BurnNPC,
		CascadeBubble,
	}

	public static void HandlePacket(BinaryReader reader, int whoAmI)
	{
		var id = (MessageType)reader.ReadByte();
		SpiritReforgedMod.Instance.Logger.Debug("[Synchronization] Reading incoming: " + id);

		switch (id)
		{
			case MessageType.SendVentEruption: //Normally should only be recieved by clients
				short i = reader.ReadInt16();
				short j = reader.ReadInt16();

				if (Main.netMode == NetmodeID.Server)
				{
					//If received by the server, send to all clients
					var packet = SpiritReforgedMod.Instance.GetPacket(MessageType.SendVentEruption, 2);
					packet.Write(i);
					packet.Write(j);
					packet.Send(ignoreClient: whoAmI);
				}

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

			case MessageType.AddPin:
				Vector2 cursorPos = reader.ReadVector2();
				string pinName = reader.ReadString();

				if (Main.netMode == NetmodeID.Server)
				{
					ModPacket packet = SpiritReforgedMod.Instance.GetPacket(MessageType.AddPin, 3);
					packet.WriteVector2(cursorPos);
					packet.Write(pinName);
					packet.Send();
				}

				ModContent.GetInstance<PinSystem>().SetPin(pinName, cursorPos);
				break;

			case MessageType.RemovePin:
				string removePinName = reader.ReadString();

				if (Main.netMode == NetmodeID.Server)
				{
					ModPacket packet = SpiritReforgedMod.Instance.GetPacket(MessageType.RemovePin, 1);
					packet.Write(removePinName);
					packet.Send();
				}

				ModContent.GetInstance<PinSystem>().RemovePin(removePinName);
				break;

			case MessageType.AskForPointsOfInterest:
				if (Main.netMode == NetmodeID.Server)
					PointOfInterestSystem.SendAllPoints(reader);
				else
					PointOfInterestSystem.RecieveAllPoints(reader);
				break;

			case MessageType.RemovePointOfInterest:
				var pointType = (InterestType)reader.ReadByte();
				var pointPos = new Point16(reader.ReadInt16(), reader.ReadInt16());

				if (Main.netMode == NetmodeID.Server)
				{
					ModPacket packet = SpiritReforgedMod.Instance.GetPacket(MessageType.RemovePointOfInterest, 3);
					packet.Write((byte)pointType);
					packet.Write(pointPos.X);
					packet.Write(pointPos.Y);
					packet.Send(-1, whoAmI);
				}

				PointOfInterestSystem.RemovePoint(pointPos, pointType, true);
				break;

			case MessageType.RevealMap:
				var syncType = (RevealMap.MapSyncId)reader.ReadByte();
				RevealMap.RecieveSync(syncType, reader);
				break;

			case MessageType.MagmaGlowPoint: //Received by clients only
				Magmastone.ToggleWireGlowPoint(reader.ReadInt16(), reader.ReadInt16());
				break;

			case MessageType.PackVisibility:
				{
					bool visibility = reader.ReadBoolean();
					byte player = reader.ReadByte();

					if (Main.netMode == NetmodeID.Server)
						BackpackPlayer.SendVisibilityPacket(visibility, player, whoAmI);

					Main.player[player].GetModPlayer<BackpackPlayer>().packVisible = visibility;
					break;
				}

			case MessageType.BurnNPC: //Sent from server to clients
				int npcIndex = reader.ReadInt32();
				UndeadNPC.BurnAway(Main.npc[npcIndex]);
				break;

			case MessageType.CascadeBubble:
				{
					float value = reader.ReadSingle();
					byte player = reader.ReadByte();

					if (Main.netMode == NetmodeID.Server)
						CascadeArmorPlayer.SendBubblePacket(value, player, whoAmI);

					Main.player[player].GetModPlayer<CascadeArmorPlayer>().bubbleStrength = value;
					break;
				}

			case MessageType.SummonTag:
				int npc = reader.ReadInt16();
				byte damage = reader.ReadByte();

				if (Main.netMode == NetmodeID.Server)
					SummontTagGlobalNPC.SendTagPacket(npc, damage, whoAmI);

				Main.npc[npc].ApplySummonTag(damage, false);
				break;
		}
	}
}
