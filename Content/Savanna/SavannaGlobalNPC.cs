using SpiritReforged.Common.Misc;
using SpiritReforged.Content.Savanna.Tiles.Paintings;
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
	public override void ModifyShop(NPCShop shop)
	{
		if (shop.NpcType == NPCID.Painter)
		{
			shop.Add(Mod.Find<ModItem>(nameof(DustyFields) + "Item").Type, Condition.MoonPhaseFull, SpiritConditions.InSavanna);
			shop.Add(Mod.Find<ModItem>(nameof(DustyFields) + "Item").Type, Condition.MoonPhaseWaningGibbous, SpiritConditions.InSavanna);

			shop.Add(Mod.Find<ModItem>(nameof(OrangeSkies) + "Item").Type, Condition.MoonPhaseThirdQuarter, SpiritConditions.InSavanna);
			shop.Add(Mod.Find<ModItem>(nameof(OrangeSkies) + "Item").Type, Condition.MoonPhaseWaningCrescent, SpiritConditions.InSavanna);

			shop.Add(Mod.Find<ModItem>(nameof(StrongWinds) + "Item").Type, Condition.MoonPhaseNew, SpiritConditions.InSavanna);
			shop.Add(Mod.Find<ModItem>(nameof(StrongWinds) + "Item").Type, Condition.MoonPhaseWaxingCrescent, SpiritConditions.InSavanna);

			shop.Add(Mod.Find<ModItem>(nameof(WaningSun) + "Item").Type, Condition.MoonPhaseFirstQuarter, SpiritConditions.InSavanna);
			shop.Add(Mod.Find<ModItem>(nameof(WaningSun) + "Item").Type, Condition.MoonPhaseWaxingGibbous, SpiritConditions.InSavanna);

		}
	}
	public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
	{
		if (spawnInfo.Player.InModBiome<Biome.SavannaBiome>())
			pool[NPCID.DoctorBones] = 0.005f;
	}
}
