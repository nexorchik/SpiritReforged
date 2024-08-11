using System.IO;
using Terraria.GameContent.Bestiary;

namespace SpiritReforged.Content.Savanna.NPCs.Gar;

[AutoloadCritter]
public class Gar : ModNPC
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

	public bool hasPicked = false;
	int pickedType;
	public override void AI()
	{
		if (!hasPicked)
		{
			NPC.scale = Main.rand.NextFloat(.9f, 1f);
			pickedType = Main.rand.Next(0, 2);
			hasPicked = true;
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
	public override void SendExtraAI(BinaryWriter writer)
	{
		writer.Write(pickedType);
		writer.Write(hasPicked);
	}

	public override void ReceiveExtraAI(BinaryReader reader)
	{
		pickedType = reader.ReadInt32();
		hasPicked = reader.ReadBoolean();
	}
	public override void FindFrame(int frameHeight)
	{
		NPC.frameCounter += 0.22f;
		NPC.frameCounter %= Main.npcFrameCount[NPC.type];
		int frame = (int)NPC.frameCounter;
		NPC.frame.Y = frame * frameHeight;
		NPC.frame.X = 80 * pickedType;
		NPC.frame.Width = 80;
	}
	//TODO: HitEffect Dust should look better + velocity should match hit velocity more
	public override void HitEffect(NPC.HitInfo hit)
	{
		for (int num621 = 0; num621 < 8; num621++)
		{
			int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood);
			Main.dust[dust].noGravity = false;
			Main.dust[dust].velocity *= 1.15f * hit.HitDirection;
			Main.dust[dust].scale = Main.rand.NextFloat(.5f, .7f);

		}
		if (NPC.life <= 0 && Main.netMode != NetmodeID.Server)
		{

			if (pickedType == 0)
			{
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("GarGore1").Type, 1f);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("GarGore2").Type, Main.rand.NextFloat(.5f, .7f));
			}
			else
			{
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("GarGore3").Type, 1f);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("GarGore4").Type, Main.rand.NextFloat(.5f, .7f));
			}
		}
	}
}