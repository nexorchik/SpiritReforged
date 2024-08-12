using SpiritReforged.Content.Vanilla.Items.Food;
using Terraria.GameContent.Bestiary;

namespace SpiritReforged.Content.Savanna.NPCs.Gar;

[AutoloadCritter]
public class GoldGar : ModNPC
{
	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[NPC.type] = 12;
		Main.npcCatchable[NPC.type] = true;
		NPCID.Sets.CountsAsCritter[Type] = true;
	}

	public override void SetDefaults()
	{
		NPC.width = 80;
		NPC.height = 28;
		NPC.damage = 0;
		NPC.defense = 0;
		NPC.lifeMax = 5;
		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath1;
		NPC.knockBackResist = .35f;
		NPC.aiStyle = 16;
		NPC.noGravity = true;
		NPC.npcSlots = 0;
		AIType = NPCID.Goldfish;
		NPC.dontCountMe = true;
	}

	public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
	{
		bestiaryEntry.UIInfoProvider = new CritterUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[Type]);
		bestiaryEntry.AddInfo(this, "Ocean");
	}

	public override void AI()
	{
		Lighting.AddLight((int)(NPC.Center.X / 16f), (int)(NPC.Center.Y / 16f), .1f, .1f, .1f);

		if (Main.rand.NextBool(30))
		{
			int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GoldCoin);
			Main.dust[d].velocity *= 0f;
			Main.dust[d].fadeIn += 0.5f;
		}

		Player target = Main.player[NPC.target];
		if (NPC.DistanceSQ(target.Center) < 65 * 65 && target.wet && NPC.wet)
		{
			Vector2 vel = NPC.DirectionFrom(target.Center) * 4.5f;
			NPC.velocity = vel;
			NPC.rotation = NPC.velocity.X * .06f;
			if (target.position.X > NPC.position.X)
			{
				NPC.spriteDirection = -1;
				NPC.direction = -1;
				NPC.netUpdate = true;
			}
			else if (target.position.X < NPC.position.X)
			{
				NPC.spriteDirection = 1;
				NPC.direction = 1;
				NPC.netUpdate = true;
			}
		}
	}

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		drawColor = NPC.GetNPCColorTintedByBuffs(drawColor);
		var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
		spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
		return false;
	}
	public override void FindFrame(int frameHeight)
	{
		NPC.frameCounter += 0.22f;
		NPC.frameCounter %= Main.npcFrameCount[NPC.type];
		int frame = (int)NPC.frameCounter;
		NPC.frame.Y = frame * frameHeight;
	}
	public override void HitEffect(NPC.HitInfo hit)
	{
		for (int num621 = 0; num621 < 13; num621++)
		{
			Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Sunflower, 2f * hit.HitDirection, -2f, 0, default, Main.rand.NextFloat(0.75f, 0.95f));
		}

		if (NPC.life <= 0 && Main.netMode != NetmodeID.Server)
		{
			Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("GarGore5").Type, 1f);
			Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("GarGore6").Type, Main.rand.NextFloat(.5f, .7f));
		}
	}
	public override void ModifyNPCLoot(NPCLoot npcLoot) => npcLoot.AddCommon<RawFish>();
}