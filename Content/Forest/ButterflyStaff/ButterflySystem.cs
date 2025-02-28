using System.IO;
using System.Linq;
using Terraria.ModLoader.IO;

namespace SpiritReforged.Content.Forest.ButterflyStaff;

internal class ButterflySystem : ModSystem
{
	public HashSet<Rectangle> butterflyZones = [];

	public override void SaveWorldData(TagCompound tag)
	{
		if (butterflyZones.Count != 0)
			tag[nameof(butterflyZones)] = butterflyZones.ToList();
	}

	public override void LoadWorldData(TagCompound tag)
	{
		var list = tag.GetList<Rectangle>(nameof(butterflyZones));
		butterflyZones = [.. list];

		if (Main.netMode != NetmodeID.MultiplayerClient)
		{
			foreach (var rect in butterflyZones)
				Populate(rect);
		}
	}

	/// <summary> Spawns butterfly NPCs within <paramref name="rect"/>. </summary>
	private static void Populate(Rectangle rect)
	{
		const int tries = 20;
		int randomCount = Main.rand.Next(3, 6);

		for (int i = 0; i < randomCount; i++)
		{
			var pos = Vector2.Zero;
			for (int t = 0; t < tries; t++)
			{
				pos = Main.rand.NextVector2FromRectangle(rect).ToWorldCoordinates();
				if (!Collision.SolidCollision(pos, 8, 8))
					break;
			}

			NPC.NewNPCDirect(null, pos, ModContent.NPCType<ButterflyCritter>()); //Withheld by PersistentNPCSystem
		}
	}

	public override void NetSend(BinaryWriter writer)
	{
		writer.Write((short)butterflyZones.Count);

		foreach (var zone in butterflyZones)
		{
			writer.Write((short)zone.Location.X);
			writer.Write((short)zone.Location.Y);
			writer.Write((short)zone.Width);
			writer.Write((short)zone.Height);
		}
	}

	public override void NetReceive(BinaryReader reader)
	{
		butterflyZones.Clear();
		short count = reader.ReadInt16();

		for (int i = 0; i < count; ++i)
			butterflyZones.Add(new(reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16()));
	}
}
