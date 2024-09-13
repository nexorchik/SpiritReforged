using System.Linq;

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
}
