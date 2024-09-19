using Mono.Cecil;
using System.IO;
using Terraria;
using Terraria.Audio;

namespace SpiritReforged.Content.Savanna.NPCs.Termite;

[AutoloadCritter]
public class Termite : ModNPC
{
	public bool hasGivenStats = false;

	public int termiteLifespan;
	public int termiteTimeLeft;
	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[NPC.type] = 3;
		Main.npcCatchable[NPC.type] = true;
		NPCID.Sets.CountsAsCritter[Type] = true;
	}

	public override void SetDefaults()
	{
		NPC.width = 16;
		NPC.height = 12;
		NPC.damage = 0;
		NPC.defense = 0;
		NPC.lifeMax = 5;
		NPC.dontCountMe = true;
		NPC.HitSound = SoundID.NPCHit1;
		NPC.knockBackResist = .45f;
		NPC.aiStyle = 66;
		NPC.npcSlots = 0;
		NPC.noGravity = false;
		AIType = NPCID.Grubby;
		NPC.dontTakeDamageFromHostiles = false;
	}

	public override void HitEffect(NPC.HitInfo hit)
	{
		if (NPC.life <= 0 && Main.netMode != NetmodeID.Server)
		{
			Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("TermiteGore").Type, NPC.scale);

			SoundEngine.PlaySound(new SoundStyle("SpiritReforged/Assets/SFX/NPCDeath/BugDeath"), NPC.Center);

			for (int k = 0; k < 5; k++)
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Scarecrow, 1.05f * hit.HitDirection, -1.95f, 0, new Color(), 0.6f);
		}
	}

	public override void AI()
	{
		if (!hasGivenStats)
		{
			NPC.scale = Main.rand.NextFloat(.6f, 1f);
			termiteLifespan = Main.rand.Next(340, 680);
			termiteTimeLeft = termiteLifespan;
			hasGivenStats = true;
		}

		Point pos = NPC.Center.ToTileCoordinates();
		Tile tile = Main.tile[pos.X, pos.Y];

		if (tile.TileType is TileID.Trees || tile.TileType is TileID.PalmTree)
		{
			if (Main.rand.NextBool(300))
				WorldGen.KillTile(pos.X, pos.Y, true);
		}

		termiteTimeLeft--;

		if (termiteTimeLeft == termiteLifespan / 4)
			NPC.velocity.Y -= 2f * (NPC.scale * 2f);

		if(termiteTimeLeft <= termiteLifespan / 4)
			NPC.rotation = MathHelper.WrapAngle((float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + (NPC.velocity.X < 0 ? MathHelper.Pi : 0));

		if (termiteTimeLeft < termiteLifespan / 4 && (NPC.collideY || NPC.collideX))
		{
			SoundEngine.PlaySound(SoundID.Dig with { Volume = .05f }, NPC.Center);
			SoundEngine.PlaySound(new SoundStyle("SpiritReforged/Assets/SFX/Ambient/BugChitter") with { Volume = 1.14f, PitchVariance = 0.4f }, NPC.Center);

			for (int i = 0; i < 3; i++)
			{
				Vector2 dustPosition = NPC.Center + new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f));
				int newDust = Dust.NewDust(dustPosition, NPC.width / 2, NPC.height / 2, DustID.Dirt, NPC.velocity.X, NPC.velocity.Y, 0, default, 1f);
				Main.dust[newDust].noGravity = true;
				Main.dust[newDust].velocity *= 0.8f;
				Main.dust[newDust].velocity += NPC.velocity * 0.2f;
			}

			NPC.active = false;
			NPC.netUpdate = true;
		}

		NPC.spriteDirection = NPC.direction;
	}
	
	public override void SendExtraAI(BinaryWriter writer)
	{
		writer.Write(hasGivenStats);
		writer.Write(termiteLifespan);
	}
	
	public override void ReceiveExtraAI(BinaryReader reader)
	{
		termiteLifespan = reader.ReadInt32();
		hasGivenStats = reader.ReadBoolean();
	}
	
	public override void FindFrame(int frameHeight)
	{
		if (NPC.velocity != Vector2.Zero || NPC.IsABestiaryIconDummy)
		{
			NPC.frameCounter += 0.12f;
			NPC.frameCounter %= Main.npcFrameCount[NPC.type];
			int frame = (int)NPC.frameCounter;
			NPC.frame.Y = frame * frameHeight;
		}
	}
}
