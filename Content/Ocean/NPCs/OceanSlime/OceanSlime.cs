using Mono.Cecil;
using SpiritReforged.Common.NPCCommon;
using SpiritReforged.Content.Vanilla.Items.Food;
using System.IO;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;

namespace SpiritReforged.Content.Ocean.NPCs.OceanSlime;
 
public class OceanSlime : ModNPC
{
	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[NPC.type] = 2;
		NPCHelper.ImmuneTo(this, BuffID.Poisoned, BuffID.Venom);
	}

	public override void SetDefaults()
	{
		NPC.width = 22;
		NPC.height = 26;
		NPC.damage = 17;
		NPC.defense = 5;
		NPC.lifeMax = 45;
		NPC.value = 12f;
		NPC.knockBackResist = .45f;
		NPC.aiStyle = 1;

		AIType = NPCID.BlueSlime;
		AnimationType = NPCID.BlueSlime;
		//Banner = NPC.type;
		//BannerItem = ModContent.ItemType<Items.Banners.CoconutSlimeBanner>();
	}
	int damageLevel;
	public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) => bestiaryEntry.AddInfo(this, "Ocean");
	public override void SendExtraAI(BinaryWriter writer) => writer.Write(damageLevel);
	public override void ReceiveExtraAI(BinaryReader reader) => damageLevel = reader.ReadInt32();
	public override void HitEffect(NPC.HitInfo hit)
	{
		SoundEngine.PlaySound(new SoundStyle("SpiritReforged/Assets/SFX/OceanSlime/CoconutSlime_Hit") with { PitchVariance = 0.4f, Pitch = 0.1f, Volume = 1.1f }, NPC.Center);

		for (int k = 0; k < 6; k++)
		{
			Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.DynastyWood, hit.HitDirection, -.5f, 0, Color.White, 0.7f);
			Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.DynastyWood, hit.HitDirection, -.5f, 0, default, .34f);
		}

		if(Main.rand.NextBool(10))
		{
			Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Coconut3").Type, Main.rand.NextFloat(.4f, .7f));
		}

		if (Main.rand.NextBool(16))
		{
			Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, 386, Main.rand.NextFloat(.4f, .7f));
		}

		if (NPC.life <= 0 && Main.netMode != NetmodeID.Server)
		{
			SoundEngine.PlaySound(new SoundStyle("SpiritReforged/Assets/SFX/OceanSlime/CoconutSlime_Death") with { Volume = 1.21f }, NPC.Center);
			SoundEngine.PlaySound(SoundID.NPCDeath1 with { Volume = .1f }, NPC.Center);

			for (int k = 0; k < 6; k++)
			{
				//Plantera leaf gore (no internal ID)
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * 0.5f, 386, Main.rand.NextFloat(.3f, .8f));
			}

			Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Coconut1").Type, 1f);
			Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Coconut2").Type, 1f);
		}
	}
	public override void AI()
	{
		if (NPC.life > NPC.lifeMax / 3 * 2)
		{
			damageLevel = 0;
		}
		else if (NPC.life > NPC.lifeMax / 3)
		{
			damageLevel = 1;
		}
		else if (NPC.life > 0)
		{
			damageLevel = 2; 
		}
	}
	public override void FindFrame(int frameHeight)
	{
		NPC.frame.X = 50 * damageLevel;
		NPC.frame.Width = 50;
	}
	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		drawColor = NPC.GetNPCColorTintedByBuffs(drawColor);
		var effects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
		spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
		return false;
	}
	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
		npcLoot.AddCommon(ItemID.Gel, 1, 1, 3);
		npcLoot.AddCommon<CoconutMilk>(); 
		npcLoot.AddCommon(ItemID.SlimeStaff, 10000);
	}
}