using SpiritReforged.Content.Vanilla.Items.Food;
using Terraria.GameContent.Bestiary;

namespace SpiritReforged.Content.Ocean.NPCs.OceanSlime;
 
public class OceanSlime : ModNPC
{
	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[NPC.type] = Main.npcFrameCount[NPCID.BlueSlime];
		//NPCHelper.ImmuneTo(this, BuffID.Poisoned, BuffID.Venom);
	}

	public override void SetDefaults()
	{
		NPC.width = 22;
		NPC.height = 22;
		NPC.damage = 17;
		NPC.defense = 5;
		NPC.lifeMax = 45;
		NPC.HitSound = SoundID.NPCHit2;
		NPC.DeathSound = SoundID.NPCDeath1;
		NPC.value = 12f;
		NPC.knockBackResist = .45f;
		NPC.aiStyle = 1;

		AIType = NPCID.BlueSlime;
		AnimationType = NPCID.BlueSlime;
		//Banner = NPC.type;
		//BannerItem = ModContent.ItemType<Items.Banners.CoconutSlimeBanner>();
	}

	public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) => bestiaryEntry.AddInfo(this, "Ocean");

	public override void HitEffect(NPC.HitInfo hit)
	{
		for (int k = 0; k < 12; k++)
		{
			Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.DynastyWood, 2.5f * hit.HitDirection, -2.5f, 0, Color.White, 0.7f);
			Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.DynastyWood, 2.5f * hit.HitDirection, -2.5f, 0, default, .34f);
		}

		if (NPC.life <= 0 && Main.netMode != NetmodeID.Server)
		{
			for (int k = 0; k < 6; k++)
			{
				//Plantera leaf gore (no internal ID)
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * 0.5f, 386, Main.rand.NextFloat(.3f, .8f));
			}

			Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Coconut1").Type, 1f);
			Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Coconut2").Type, 1f);
		}
	}
	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
		npcLoot.AddCommon(ItemID.Gel, 1, 1, 3);
		npcLoot.AddCommon<CoconutMilk>(); 
		npcLoot.AddCommon(ItemID.SlimeStaff, 10000);
	}
}