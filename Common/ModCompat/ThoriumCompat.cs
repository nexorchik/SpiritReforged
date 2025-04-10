
using SpiritReforged.Common.ItemCommon;
using SpiritReforged.Common.Misc;
using SpiritReforged.Content.Forest.Cloud.Items;
using SpiritReforged.Content.Ocean.Items;
using SpiritReforged.Content.Ocean.Items.JellyfishStaff;
using SpiritReforged.Content.Savanna.Items.Food;
using SpiritReforged.Content.Savanna.Tiles;
using SpiritReforged.Content.Savanna.Tiles.Paintings;
using SpiritReforged.Content.Underground.ExplorerTreads;
using SpiritReforged.Content.Vanilla.Food;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace SpiritReforged.Common.ModCompat;

internal class ThoriumCompat : ModSystem
{
	public static Mod Instance;
	public static bool Enabled => Instance != null;

	public override void Load()
	{
		Instance = null;
		if (!ModLoader.TryGetMod("ThoriumMod", out Instance))
			return;
	}
}

internal class ThoriumGlobalNPC : GlobalNPC
{
	public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
	{
		if (ThoriumCompat.Enabled)
		{
			if (ThoriumCompat.Instance.TryFind("RagingMinotaur", out ModNPC minotaur))
			{
				if (npc.type == minotaur.Type)
					npcLoot.AddCommon(ItemID.Leather, 1, 5, 10);
			}

			if (ThoriumCompat.Instance.TryFind("ManofWar", out ModNPC bigJellyfish))
			{
				if (npc.type == bigJellyfish.Type)
					npcLoot.AddCommon(ModContent.ItemType<JellyfishStaff>(), 50);
			}
		}
	}

	public override void ModifyShop(NPCShop shop)
	{
		if (ThoriumCompat.Enabled)
		{
			if (ThoriumCompat.Instance.TryFind("Cobbler", out ModNPC cobbler))
			{
				if (shop.NpcType == cobbler.Type)
					shop.Add(ModContent.ItemType<ExplorerTreadsItem>(), Condition.DownedEyeOfCthulhu);
			}

			if (ThoriumCompat.Instance.TryFind("Druid", out ModNPC druid))
			{
				if (shop.NpcType == druid.Type)
					shop.Add(ModContent.ItemType<CloudstalkSeed>(), Condition.DownedEyeOfCthulhu);
			}
		}
	}
}

internal class ThoriumGlobalTile : GlobalTile
{
	public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
	{
		if (ThoriumCompat.Enabled && ThoriumCompat.Instance.TryFind("LivingLeaf", out ModItem livingLeaf))
		{
			if (type == ModContent.TileType<LivingBaobabLeaf>() && Main.rand.NextBool(3))
				ItemMethods.NewItemSynced(new EntitySource_TileBreak(i, j), livingLeaf.Type, new Rectangle(i * 16, j * 16, 16, 16).Center(), true);
		}
	}
}