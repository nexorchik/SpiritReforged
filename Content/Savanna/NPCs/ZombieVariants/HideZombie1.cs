using SpiritReforged.Common.NPCCommon;
using SpiritReforged.Content.Savanna.Biome;
using SpiritReforged.Content.Savanna.Items.HuntingRifle;
using System.IO;
using Terraria.GameContent.Bestiary;

namespace SpiritReforged.Content.Savanna.NPCs.ZombieVariants;

public class HideZombie1 : ReplaceNPC
{
	float frameCounter;

	public override int[] TypesToReplace => [NPCID.Zombie, NPCID.BaldZombie,
		NPCID.PincushionZombie, NPCID.SlimedZombie, NPCID.SwampZombie, NPCID.TwiggyZombie];

	public override void StaticDefaults()
	{
		Main.npcFrameCount[Type] = Main.npcFrameCount[NPCID.Zombie];
		NPCID.Sets.Zombies[Type] = true;
	}

	public override void SetDefaults()
	{
		NPC.width = 28;
		NPC.height = 50;
		NPC.damage = 10;
		NPC.defense = 7;
		NPC.lifeMax = 51;
		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath2;
		NPC.value = 50f;
		NPC.knockBackResist = .45f;
		NPC.aiStyle = 3;
		AIType = NPCID.Zombie;
		AnimationType = NPCID.Zombie;
		Banner = Item.NPCtoBanner(NPCID.Zombie);
		BannerItem = Item.BannerToItem(Banner);
		SpawnModBiomes = [ModContent.GetInstance<SavannaBiome>().Type];
	}

	public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) => bestiaryEntry.AddInfo(this, "");

	public override void HitEffect(NPC.HitInfo hit)
	{
		for (int k = 0; k < 20; k++)
			Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, 2.5f * hit.HitDirection, -2.5f, 0, Color.White, 0.78f);

		if (NPC.life <= 0 && Main.netMode != NetmodeID.Server)
		{
			Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>(GetType().Name).Type, 1f);
			Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, 4, 1f);
			Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, 5, 1f);
		}
	}

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
		npcLoot.AddCommon(ItemID.Leather, 2, 1, 2);
		npcLoot.AddCommon(ItemID.Shackle, 50);
		npcLoot.AddCommon(ItemID.ZombieArm, 250);

	}

	public override bool CanSpawn(Player player) => player.InModBiome<Biome.SavannaBiome>();
}