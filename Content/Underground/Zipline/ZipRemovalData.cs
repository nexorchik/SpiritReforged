using SpiritReforged.Common.Multiplayer;
using SpiritReforged.Content.Underground.Zipline;
using System.IO;

/// <summary> Syncs removal of all ziplines owned by the given player. </summary>
internal class ZipRemovalData : PacketData
{
	private readonly short _player;

	public ZipRemovalData() { }
	public ZipRemovalData(short player) => _player = player;

	public override void OnReceive(BinaryReader reader, int whoAmI)
	{
		short playerIndex = reader.ReadInt16();

		if (Main.netMode == NetmodeID.Server) //Relay to other clients
			new ZipRemovalData(playerIndex).Send(ignoreClient: whoAmI);

		RemoveZiplines(playerIndex);
	}

	public override void OnSend(ModPacket modPacket) => modPacket.Write(_player);

	/// <summary> Removes all ziplines owned by <paramref name="playerIndex"/>. </summary>
	public static void RemoveZiplines(short playerIndex)
	{
		foreach (var zipline in ZiplineHandler.ziplines)
		{
			if (zipline.Owner.whoAmI == playerIndex)
			{
				FX(zipline);
				ZiplineHandler.ziplines.Remove(zipline);
			}
		}

		static void FX(Zipline zipline)
		{
			foreach (var p in zipline.points)
				ZiplineProj.DeathEffects(p);
		}
	}
}