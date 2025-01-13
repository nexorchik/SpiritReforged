using SpiritReforged.Common.Misc;
using System.IO;
using System.Linq;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace SpiritReforged.Common.WorldGeneration;

/// <summary>
/// Handles marking any points of interest, such as the Shimmer and Hives.
/// This is currently for use in Hiker's mapping system.
/// </summary>
internal class PointOfInterestSystem : ModSystem
{
	public static PointOfInterestSystem Instance => ModContent.GetInstance<PointOfInterestSystem>();

	public Dictionary<InterestType, HashSet<Point16>> PointsOfInterestByPosition = [];
	public HashSet<InterestType> TakenInterestTypes = [];

	/// <summary>
	/// Stores the points of interests that were found on world gen only.
	/// This should be immutable aside from world gen; stored so the world always knows all <see cref="InterestType"/>s that were found.
	/// </summary>
	public Dictionary<InterestType, HashSet<Point16>> WorldGen_PointsOfInterestByPosition = [];
	public HashSet<InterestType> WorldGen_TakenInterestTypes = [];

	public static bool HasInterestType(InterestType type) => Instance.TakenInterestTypes.Contains(type);
	public static bool AddInterestType(InterestType type) => Instance.TakenInterestTypes.Add(type);
	public static bool HasAnyInterests() => Instance.TakenInterestTypes.Count > 0;

	/// <summary>
	/// Gets a random point of the given type.
	/// </summary>
	/// <param name="type">The type of interest point to get.</param>
	/// <param name="random">The random to use. Defaults to <see cref="Main.rand"/>.</param>
	public static Point16 GetPoint(InterestType type, UnifiedRandom random = null) => (random ?? Main.rand).Next([.. Instance.PointsOfInterestByPosition[type]]);
	public static void AddPoint(Point16 position, InterestType type) => Instance.InstancedAddPoint(position, type);

	public static void RemovePoint(Point16 position, InterestType type, bool fromNet = false)
	{
		Instance.PointsOfInterestByPosition[type].Remove(position);

		if (Instance.PointsOfInterestByPosition[type].Count == 0)
		{
			Instance.PointsOfInterestByPosition.Remove(type);
			Instance.TakenInterestTypes.Remove(type);
		}

		if (!fromNet && Main.netMode != NetmodeID.SinglePlayer)
		{
			ModPacket packet = SpiritReforgedMod.Instance.GetPacket(ReforgedMultiplayer.MessageType.RemovePointOfInterest, 3);
			packet.Write((byte)type);
			packet.Write(position.X);
			packet.Write(position.Y);
			packet.Send();
		}
	}

	public void InstancedAddPoint(Point16 position, InterestType type)
	{
		if (!PointsOfInterestByPosition.TryGetValue(type, out HashSet<Point16> list))
			PointsOfInterestByPosition.Add(type, [position]);
		else
			list.Add(position);

		AddInterestType(type);
	}

	public override void SaveWorldData(TagCompound tag)
	{
		SavePoints(tag, PointsOfInterestByPosition, "");
		SavePoints(tag, WorldGen_PointsOfInterestByPosition, "WorldGen");
	}

	private static void SavePoints(TagCompound tag, Dictionary<InterestType, HashSet<Point16>> dictionary, string keyPrefix)
	{
		int typesCount = dictionary.Count;
		tag.Add("typesCount" + keyPrefix, typesCount);

		for (int i = 0; i < typesCount; ++i)
		{
			var pair = dictionary.ElementAt(i);
			tag.Add("type" + keyPrefix + i, (byte)pair.Key);
			tag.Add("points" + keyPrefix + i, pair.Value.ToArray());
		}
	}

	public override void LoadWorldData(TagCompound tag)
	{
		PointsOfInterestByPosition = [];
		WorldGen_PointsOfInterestByPosition = [];

		TakenInterestTypes = [];
		WorldGen_TakenInterestTypes = [];

		LoadPoints(tag, PointsOfInterestByPosition, TakenInterestTypes, "");
		LoadPoints(tag, WorldGen_PointsOfInterestByPosition, WorldGen_TakenInterestTypes, "WorldGen");
	}

	private static void LoadPoints(TagCompound tag, Dictionary<InterestType, HashSet<Point16>> dictionary, HashSet<InterestType> takenTypes, string keyPrefix)
	{
		int count = tag.GetInt("typesCount" + keyPrefix);

		for (int i = 0; i < count; ++i)
		{
			var type = (InterestType)tag.GetByte("type" + keyPrefix + i);
			HashSet<Point16> points = new(tag.Get<Point16[]>("points" + keyPrefix + i));

			dictionary.Add(type, points);
			takenTypes.Add(type);
		}
	}

	internal static void SendAllPoints(BinaryReader reader)
	{
		bool isWorldGen = reader.ReadBoolean();
		byte fromWho = reader.ReadByte();
		Dictionary<InterestType, HashSet<Point16>> dictionary = isWorldGen ? Instance.WorldGen_PointsOfInterestByPosition : Instance.PointsOfInterestByPosition;
		int packetSize = 2 + dictionary.Count * 2 + dictionary.Sum(x => x.Value.Count) * 2;

		ModPacket packet = SpiritReforgedMod.Instance.GetPacket(ReforgedMultiplayer.MessageType.AskForPointsOfInterest, packetSize);
		packet.Write(isWorldGen);
		packet.Write((short)dictionary.Count);

		for (int i = 0; i < dictionary.Count; ++i)
		{
			var pair = dictionary.ElementAt(i);
			packet.Write((byte)pair.Key);
			packet.Write((short)pair.Value.Count);

			for (int j = 0; j < pair.Value.Count; ++j)
			{
				var pos = pair.Value.ElementAt(j);
				packet.Write(pos.X);
				packet.Write(pos.Y);
			}
		}

		packet.Send(fromWho);
	}

	internal static void RecieveAllPoints(BinaryReader reader)
	{
		bool isWorldGen = reader.ReadBoolean();
		Dictionary<InterestType, HashSet<Point16>> dictionary = isWorldGen ? Instance.WorldGen_PointsOfInterestByPosition : Instance.PointsOfInterestByPosition;
		dictionary.Clear();

		short count = reader.ReadInt16();

		for (int i = 0; i < count; ++i)
		{
			var type = (InterestType)reader.ReadByte();
			int subCount = reader.ReadInt16();

			for (int j = 0; j < subCount; ++j)
			{
				int x = reader.ReadInt16();
				int y = reader.ReadInt16();

				AddPoint(new Point16(x, y), type);
			}
		}
	}

	public class PoIPlayer : ModPlayer
	{
		public override void OnEnterWorld()
		{
			if (Main.netMode != NetmodeID.SinglePlayer)
			{
				ModPacket worldGenPacket = SpiritReforgedMod.Instance.GetPacket(ReforgedMultiplayer.MessageType.AskForPointsOfInterest, 2);
				worldGenPacket.Write(true);
				worldGenPacket.Write((byte)Main.myPlayer);
				worldGenPacket.Send();

				ModPacket normalPacket = SpiritReforgedMod.Instance.GetPacket(ReforgedMultiplayer.MessageType.AskForPointsOfInterest, 2);
				normalPacket.Write(false);
				normalPacket.Write((byte)Main.myPlayer);
				normalPacket.Send();
			}
		}
	}
}
