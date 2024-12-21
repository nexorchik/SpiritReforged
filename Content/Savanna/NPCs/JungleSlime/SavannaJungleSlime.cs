using SpiritReforged.Content.Savanna.Biome;
using SpiritReforged.Content.Savanna.DustStorm;
using SpiritReforged.Content.Vanilla.Items.Food;
using Terraria.GameContent.Bestiary;

namespace SpiritReforged.Content.Savanna.NPCs.JungleSlime;

public class SavannaJungleSlime : ModNPC
{
	public override void SetStaticDefaults() => Main.npcFrameCount[Type] = 2;

	public override void SetDefaults()
	{
		// I'm not cloning JungleSLime defaults because the color the Jungle Slime draws looks ugly. Doing it manually.
		NPC.width = NPC.height = 30;
		NPC.lifeMax = 60;
		NPC.damage = 18;
		NPC.defense = 6;
		NPC.knockBackResist = 1f;
		NPC.aiStyle = 1;
		AIType = NPCID.JungleSlime;
		AnimationType = NPCID.BlueSlime;
		Banner = Item.NPCtoBanner(NPCID.JungleSlime);
		BannerItem = Item.BannerToItem(Banner);
		NPC.alpha = 60;
		SpawnModBiomes = [ModContent.GetInstance<SavannaBiome>().Type];
	}

	public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) => bestiaryEntry.AddInfo(this, "Sandstorm");

	public override void HitEffect(NPC.HitInfo hit)
	{
		for (int k = 0; k < 20; k++)
			Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.JungleGrass, 2.5f * hit.HitDirection, -2.5f, 0, default, 0.78f);
	}

	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
		npcLoot.AddCommon(ItemID.Gel, 1, 1, 3);
		npcLoot.AddCommon(ItemID.SlimeStaff, 10000);
	}

	public override float SpawnChance(NPCSpawnInfo spawnInfo)
	{
		var player = spawnInfo.Player;
		return player.InModBiome<SavannaBiome>() && player.ZoneJungle ? 0.1f : 0;
	}
}