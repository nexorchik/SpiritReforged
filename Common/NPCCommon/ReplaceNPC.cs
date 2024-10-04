using System.Linq;
using Terraria.DataStructures;

namespace SpiritReforged.Common.NPCCommon;

public abstract class ReplaceNPC : ModNPC
{
	/// <summary> The types of NPCs to be replaced. </summary>
	public abstract int[] TypesToReplace { get; }

	public sealed override void SetStaticDefaults()
	{
		ReplaceGlobalNPC.types.Add(Type, TypesToReplace);
		StaticDefaults();
	}
	public virtual void StaticDefaults() { }

	/// <summary> Under what conditions this NPC should replace regular spawns. </summary>
	public abstract bool CanSpawn(Player player);
}

public class ReplaceGlobalNPC : GlobalNPC
{
	//Stores a modded NPC type and vanilla NPC types to replace, respectively
	internal static Dictionary<int, int[]> types = [];

	public override void OnSpawn(NPC npc, IEntitySource source)
	{
		if (Main.netMode == NetmodeID.MultiplayerClient || source is not EntitySource_SpawnNPC)
			return;

		var closest = Main.player[Player.FindClosest(npc.position, npc.width, npc.height)];
		var validT = types.Where(x => x.Value.Contains(npc.type) && (NPCLoader.GetNPC(x.Key) as ReplaceNPC).CanSpawn(closest)).ToArray();
		if (validT.Length != 0)
		{
			npc.Transform(validT[Main.rand.Next(validT.Length)].Key);
			npc.netUpdate = true;
		}
	}
}