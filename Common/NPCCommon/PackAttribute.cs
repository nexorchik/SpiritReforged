using Terraria.DataStructures;

namespace SpiritReforged.Common.NPCCommon;

/// <summary> Facilitates spawning NPCs in packs. </summary>
[AttributeUsage(AttributeTargets.Class)]
public class SpawnPackAttribute : Attribute
{
	public SpawnPackAttribute(int size) => (MinSize, MaxSize) = (size, size);
	public SpawnPackAttribute(int minSize, int maxSize) => (MinSize, MaxSize) = (minSize, maxSize);

	public int MinSize { get; private set; }
	public int MaxSize { get; private set; }
}

internal class PackGlobalNPC : GlobalNPC
{
	private static SpawnPackAttribute Tag(int type)
	{
		if (NPCLoader.GetNPC(type) is ModNPC mNPC)
			return (SpawnPackAttribute)Attribute.GetCustomAttribute(mNPC.GetType(), typeof(SpawnPackAttribute));

		return null;
	}

	public override void OnSpawn(NPC npc, IEntitySource source)
	{
		if (Main.netMode == NetmodeID.MultiplayerClient || source is EntitySource_Parent/*|| source is not EntitySource_SpawnNPC*/ || Tag(npc.type) is not SpawnPackAttribute atr)
			return;

		int packSize = Main.rand.Next(atr.MinSize, atr.MaxSize + 1) - 1;
		for (int i = 0; i < packSize; i++)
		{
			var randomPos = npc.Center + new Vector2(25 * Main.rand.NextFloat(-1f, 1f), 0);
			NPC.NewNPCDirect(new EntitySource_Parent(npc), randomPos, npc.type);
		}
	}
}
