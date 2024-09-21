using System.Linq;

namespace SpiritReforged.Content.Savanna;

public class SavannaGlobalNPC : GlobalNPC
{
	public override void ModifyActiveShop(NPC npc, string shopName, Item[] items)
	{
		if (npc.type == NPCID.Dryad && Main.LocalPlayer.InModBiome<Biome.SavannaBiome>())
		{
			var grassSeeds = items.FirstOrDefault(x => x != null && x.type == ItemID.GrassSeeds);

			if (grassSeeds != default)
				grassSeeds.SetDefaults(ModContent.ItemType<Items.SavannaGrassSeeds>());
		}
	}

	public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
	{
		if (spawnInfo.Player.InModBiome<Biome.SavannaBiome>())
			pool[NPCID.DoctorBones] = 0.005f;
	}
}
