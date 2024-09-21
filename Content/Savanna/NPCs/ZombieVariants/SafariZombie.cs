using Terraria.GameContent.Bestiary;
using SpiritReforged.Content.Ocean.Items;
using SpiritReforged.Content.Savanna.Items.Vanity;

namespace SpiritReforged.Content.Savanna.NPCs.ZombieVariants;

public class SafariZombie : Common.NPCCommon.ZombieNPC
{
	public override void StaticDefaults() => Main.npcFrameCount[NPC.type] = Main.npcFrameCount[NPCID.Zombie];

	public override void SetDefaults()
	{
		NPC.width = 28;
		NPC.height = 42;
		NPC.damage = 12;
		NPC.defense = 7;
		NPC.lifeMax = 46;
		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath2;
		NPC.value = 50f;
		NPC.knockBackResist = .45f;
		NPC.aiStyle = 3;
		AIType = NPCID.Zombie;
		AnimationType = NPCID.Zombie;
		Banner = Item.NPCtoBanner(NPCID.Zombie);
		BannerItem = Item.BannerToItem(Banner);
	}

	//public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) => bestiaryEntry.AddInfo(this, "NightTime Savanna");

	public override void HitEffect(NPC.HitInfo hit)
	{
		for (int k = 0; k < 20; k++)
		{
			Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, 2.5f * hit.HitDirection, -2.5f, 0, Color.White, 0.78f);
		}

		if (NPC.life <= 0 && Main.netMode != NetmodeID.Server)
			for (int i = 1; i < 4; ++i)
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("SafariZombie" + i).Type, 1f);
	}

	float frameCounter;

	public override void FindFrame(int frameHeight)
	{
		if (NPC.IsABestiaryIconDummy)
		{
			frameCounter += .1f;
			frameCounter %= Main.npcFrameCount[Type];
			NPC.frame.Y = frameHeight * (int)frameCounter;
		}
	}

	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
		npcLoot.AddCommon(ItemID.Shackle, 50);
		npcLoot.AddCommon(ItemID.ZombieArm, 250);
		npcLoot.AddOneFromOptions(75, ModContent.ItemType<SafariHat>(), ModContent.ItemType<SafariVest>(), ModContent.ItemType<SafariShorts>());
	}

	public override bool SpawnConditions(Player player) => player.ZoneBeach;
}