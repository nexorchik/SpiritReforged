using SpiritReforged.Common.Multiplayer;
using System.IO;
using System.Linq;
using Terraria.DataStructures;

namespace SpiritReforged.Common.WorldGeneration.PointOfInterest;

/// <summary> Instantly requests points of interest upon joining a server. </summary>
internal class PoIPlayer : ModPlayer
{
	public override void OnEnterWorld()
	{
		if (Main.netMode != NetmodeID.SinglePlayer)
		{
			new AskForPoIData(true, (byte)Main.myPlayer).Send();
			new AskForPoIData(false, (byte)Main.myPlayer).Send();
		}
	}
}

internal class AskForPoIData : PacketData
{
	public Dictionary<InterestType, HashSet<Point16>> _pointsByPos;

	private static bool _fromWorldGen;
	private static byte _player;

	public AskForPoIData() { }

	/// <summary> Used by clients specifically. </summary>
	public AskForPoIData(bool fromWorldGen, byte player)
	{
		_fromWorldGen = fromWorldGen;
		_player = player;
	}

	/// <summary> Used by the server specifically. </summary>
	private AskForPoIData(bool fromWorldGen, Dictionary<InterestType, HashSet<Point16>> pointsByPos)
	{
		_fromWorldGen = fromWorldGen;
		_pointsByPos = pointsByPos;
	}

	/// <summary> The server sends all points of interest corresponding to <see cref="_fromWorldGen"/> to a given client (<see cref="_player"/>). </summary>
	public override void OnReceive(BinaryReader reader, int whoAmI)
	{
		if (Main.netMode == NetmodeID.Server)
		{
			bool isWorldGen = reader.ReadBoolean();
			byte fromWho = reader.ReadByte();

			var inst = PointOfInterestSystem.Instance;
			var dict = isWorldGen ? inst.WorldGen_PointsOfInterestByPosition : inst.PointsOfInterestByPosition;

			new AskForPoIData(isWorldGen, dict).Send(fromWho);
		}
		else
		{
			bool isWorldGen = reader.ReadBoolean();
			short count = reader.ReadInt16();

			var inst = PointOfInterestSystem.Instance;
			(isWorldGen ? inst.WorldGen_PointsOfInterestByPosition : inst.PointsOfInterestByPosition).Clear();

			for (int i = 0; i < count; ++i)
			{
				var type = (InterestType)reader.ReadByte();
				short subCount = reader.ReadInt16();

				for (int j = 0; j < subCount; ++j)
					PointOfInterestSystem.AddPoint(reader.ReadPoint16(), type);
			}
		}
	}

	public override void OnSend(ModPacket modPacket)
	{
		if (Main.netMode == NetmodeID.Server)
		{
			var dictionary = _pointsByPos;

			modPacket.Write(_fromWorldGen);
			modPacket.Write((short)dictionary.Count);

			for (int i = 0; i < dictionary.Count; ++i)
			{
				var pair = dictionary.ElementAt(i);

				modPacket.Write((byte)pair.Key);
				modPacket.Write((short)pair.Value.Count);

				for (int j = 0; j < pair.Value.Count; ++j)
					modPacket.WritePoint16(pair.Value.ElementAt(j));
			}
		}
		else
		{
			modPacket.Write(_fromWorldGen);
			modPacket.Write(_player);
		}
	}
}

internal class RemovePoIData : PacketData
{
	private readonly byte _pointType;
	private readonly Point16 _point;

	public RemovePoIData() { }
	public RemovePoIData(byte type, Point16 position)
	{
		_pointType = type;
		_point = position;
	}

	public override void OnReceive(BinaryReader reader, int whoAmI)
	{
		byte pointType = reader.ReadByte();
		Point16 point = reader.ReadPoint16();

		if (Main.netMode == NetmodeID.Server)
			new RemovePoIData(pointType, point).Send(ignoreClient: whoAmI);

		PointOfInterestSystem.RemovePoint(point, (InterestType)pointType, true);
	}

	public override void OnSend(ModPacket modPacket)
	{
		modPacket.Write(_pointType);
		modPacket.WritePoint16(_point);
	}
}
