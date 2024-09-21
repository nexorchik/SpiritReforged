using System.Linq;
using Terraria.DataStructures;

namespace SpiritReforged.Content.Savanna;

public class SavannaGlobalNPC : GlobalNPC
{
	public override void ModifyActiveShop(NPC npc, string shopName, Item[] items)
	{
		if (npc.type == NPCID.Dryad && Main.LocalPlayer.InModBiome<Biome.SavannaBiome>())
		{
			var grassSeeds = items.Where(x => x != null && x.type == ItemID.GrassSeeds).FirstOrDefault();
			if (grassSeeds != default)
				grassSeeds.SetDefaults(ModContent.ItemType<Items.SavannaGrassSeeds>());
		}
	}

	public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
	{
		if (spawnInfo.Player.InModBiome<Biome.SavannaBiome>())
		{
			float odds = spawnInfo.Player.GetModPlayer<DustStorm.DustStormPlayer>().ZoneDustStorm ? .22f : .09f;
			pool[NPCID.Vulture] = odds;
		}
	}

	public override void OnSpawn(NPC npc, IEntitySource source)
	{
		if (npc.type == NPCID.Vulture && source is EntitySource_SpawnNPC)
		{
			//Move to an acacia treetop within 30 tiles when naturally spawned
			var nearby = Tiles.AcaciaTree.AcaciaTree.Platforms.Where(x => x.Distance(npc.Center) < 16 * 30).OrderBy(x => x.Distance(npc.Center)).FirstOrDefault();
			if (nearby != default)
			{
				npc.Center = nearby.Hitbox.ClosestPointInRect(npc.Center);
				npc.netUpdate = true;
			}
		}
	}
}
