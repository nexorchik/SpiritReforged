using System.IO;
using System.Linq;

namespace SpiritReforged.Common.Multiplayer;

internal class MultiplayerHandler : ILoadable
{
	internal static readonly Dictionary<byte, PacketData> PacketTypes = [];

	/// <summary> Loads all packet definitions into a static lookup (<see cref="PacketTypes"/>). Must be ordered consistently between clients. </summary>
	public void Load(Mod mod)
	{
		byte count = 0;
		var packets = mod.Code.GetTypes().Where(x => x.IsSubclassOf(typeof(PacketData)) && !x.IsAbstract).OrderBy(x => x.Name, StringComparer.InvariantCulture);

		foreach (var packet in packets)
		{
			PacketTypes.Add(count, (PacketData)Activator.CreateInstance(packet));
			count++;
		}
	}

	public void Unload() => PacketTypes.Clear();

	public static void HandlePacket(BinaryReader reader, int whoAmI)
	{
		byte id = reader.ReadByte();
		SpiritReforgedMod.Instance.Logger.Debug("[Synchronization] Reading incoming: " + PacketTypes[id].GetType().Name);

		if (PacketTypes.TryGetValue(id, out var packet))
			packet.OnReceive(reader, whoAmI);
	}
}
