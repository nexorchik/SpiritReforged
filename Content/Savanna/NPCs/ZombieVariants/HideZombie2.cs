using SpiritReforged.Common.NPCCommon;
using SpiritReforged.Content.Savanna.Biome;
using SpiritReforged.Content.Savanna.Items.HuntingRifle;
using System.IO;
using Terraria.GameContent.Bestiary;

namespace SpiritReforged.Content.Savanna.NPCs.ZombieVariants;

public class HideZombie2 : HideZombie1
{
	public override void SetDefaults()
	{
		NPC.width = 28;
		NPC.height = 48;
		NPC.damage = 12;
		NPC.defense = 5;
		NPC.lifeMax = 43;
		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath2;
		NPC.value = 46f;
		NPC.knockBackResist = .52f;
		NPC.aiStyle = 3;
		AIType = NPCID.Zombie;
		AnimationType = NPCID.Zombie;
		Banner = Item.NPCtoBanner(NPCID.Zombie);
		BannerItem = Item.BannerToItem(Banner);
		SpawnModBiomes = [ModContent.GetInstance<SavannaBiome>().Type];
	}
}