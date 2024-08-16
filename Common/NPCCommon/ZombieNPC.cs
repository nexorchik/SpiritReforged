using System.Linq;
using Terraria.DataStructures;

namespace SpiritReforged.Common.NPCCommon;

public abstract class ZombieNPC : ModNPC
{
	public override LocalizedText DisplayName => Language.GetText("NPCName.Zombie"); //Zombie

	public sealed override void SetStaticDefaults()
	{
		ZombieSpawnNPC.Types.Add(Type);

		StaticDefaults();
	}

	public virtual void StaticDefaults() { }

	/// <summary> Under what conditions this zombie should replace regular zombie spawns. </summary>
	public abstract bool SpawnConditions(Player player);

	/// <summary> Whether this zombie should replaced armed variants. </summary>
	public virtual bool ArmedZombie() => false;
}

public class ZombieSpawnNPC : GlobalNPC
{
	internal static List<int> Types = new();

	public override void OnSpawn(NPC npc, IEntitySource source)
	{
		bool Armed() => npc.type is >= NPCID.ArmedZombie and <= NPCID.ArmedZombieCenx;

		if (Main.netMode == NetmodeID.MultiplayerClient || !(npc.type is NPCID.Zombie or NPCID.BaldZombie or NPCID.PincushionZombie or NPCID.SlimedZombie or NPCID.SwampZombie or NPCID.TwiggyZombie || Armed()))
			return;

		Player closest = Main.player[Player.FindClosest(npc.position, npc.width, npc.height)];
		int[] typesToSpawn = Types.Where(x => NPCLoader.GetNPC(x) is ZombieNPC zomb && zomb.SpawnConditions(closest) && zomb.ArmedZombie() == Armed()).ToArray();

		if (typesToSpawn.Length > 0)
		{
			int seed = (int)Main.GameUpdateCount; //Pseudorandom spawn rate to avoid any multiplayer syncing

			for (int t = 0; t < typesToSpawn.Length; t++)
			{
				if (seed % typesToSpawn.Length == t)
					npc.Transform(typesToSpawn[t]);
			}
		}
	}
}