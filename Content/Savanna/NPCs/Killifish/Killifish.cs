using SpiritReforged.Content.Vanilla.Items.Food;
using System.IO;
using Terraria.GameContent.Bestiary;

namespace SpiritReforged.Content.Savanna.NPCs.Killifish;

//[AutoloadCritter]
//[AutoloadBanner]
public class Killifish : ModNPC
{
	private ref float YMovement => ref NPC.ai[0]; // Y Movement (adapted from vanilla)
	private ref float Proximity => ref NPC.ai[1]; // Player proximity 
	private ref float Turning => ref NPC.ai[2]; // Random turning
	private ref float TurningTime => ref NPC.ai[3]; // slowdown before turning

	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[NPC.type] = 9;
		Main.npcCatchable[NPC.type] = true;
		NPCID.Sets.CountsAsCritter[Type] = true;
	}

	public override void SetDefaults()
	{
		NPC.width = 46;
		NPC.height = 28;
		NPC.damage = 0;
		NPC.defense = 0;
		NPC.lifeMax = 5;
		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath1;
		NPC.knockBackResist = .35f;
		NPC.aiStyle = -1;
		NPC.noGravity = true;
		NPC.npcSlots = 0;
		NPC.dontCountMe = true;
		NPC.friendly = true;
		NPC.dontTakeDamage = false;
	}

	public override void SetBestiary(BestiaryDatabase dataNPC, BestiaryEntry bestiaryEntry)
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
			NPC.scale = Main.rand.NextFloat(.7f, 1f);
			pickedType = Main.rand.Next(0, 2);
			hasPicked = true;
		}

		Player target = Main.player[NPC.target];
		if (NPC.wet) //swimming AI (adapted from vanilla)
		{
			if (NPC.rotation != 0f)
			{
				NPC.rotation *= .9f;
			}

			if (NPC.direction == 0)
			{
				NPC.TargetClosest();
			}

			int tileX = (int)NPC.Center.X / 16;
			int tileY = (int)(NPC.Bottom.Y / 16f);

			// what to do if sloped tiles
			if (Main.tile[tileX, tileY].TopSlope)
			{
				if (Main.tile[tileX, tileY].LeftSlope)
				{
					NPC.direction = -1;
					NPC.velocity.X = Math.Abs(NPC.velocity.X) * -1f;
				}
				else
				{
					NPC.direction = 1;
					NPC.velocity.X = Math.Abs(NPC.velocity.X);
				}
			}
			else if (Main.tile[tileX, tileY + 1].TopSlope)
			{
				if (Main.tile[tileX, tileY + 1].LeftSlope)
				{
					NPC.direction = -1;
					NPC.velocity.X = Math.Abs(NPC.velocity.X) * -1f;
				}
				else
				{
					NPC.direction = 1;
					NPC.velocity.X = Math.Abs(NPC.velocity.X);
				}
			}
			if (Turning == 0)
			{
				NPC.velocity.X += NPC.direction * .12f;

				if (NPC.velocity.X < -0.35f || NPC.velocity.X > 0.35f)
				{
					NPC.velocity.X *= 0.9f;
				}
			}
			else
			{
				NPC.velocity.X *= .96f;
			}
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				if (Turning == 0)
				{
					if (Main.rand.NextBool(260))
					{
						Turning = 1;
					}
				}
			}
			if (Turning == 1)
			{
				TurningTime++;
				if (TurningTime >= 25)
				{
					NPC.velocity.X *= -3.25f;
					NPC.velocity.Y += Main.rand.NextFloat(0.5f, -0.5f);
					NPC.direction *= -1;
					NPC.rotation = NPC.velocity.X * .12f;
					NPC.netUpdate = true;
					Turning = 0;
					TurningTime = 0;
				}
			}
			// switch directions if colliding
			if (NPC.collideX)
			{
				NPC.velocity.X *= -1f;
				NPC.direction *= -1;
				NPC.netUpdate = true;
			}

			// I don't know how often this happens, but if fish bonks head or hits floor, ease it down/up, respectively
			if (NPC.collideY)
			{
				NPC.netUpdate = true;

				if (NPC.velocity.Y > 0f)
				{
					NPC.velocity.Y = Math.Abs(NPC.velocity.Y) * -1f;
					NPC.directionY = -1;
					YMovement = -1f;
				}
				else if (NPC.velocity.Y < 0f)
				{
					NPC.velocity.Y = Math.Abs(NPC.velocity.Y);
					NPC.directionY = 1;
					YMovement = 1f;
				}
			}

			// fish goes up and down, and goes the other way upon reaching a limit
			if (YMovement == -1f)
			{
				NPC.velocity.Y -= 0.01f;
				if (NPC.velocity.Y < -0.3f)
				{
					YMovement = 1f;
				}
			}
			else
			{
				NPC.velocity.Y += 0.01f;
				if (NPC.velocity.Y > 0.3f)
				{
					YMovement = -1f;
				}
			}

			// don't swim too close to bottom tiles
			if (Main.tile[tileX, tileY - 1].LiquidAmount > 128)
			{
				if (Main.tile[tileX, tileY + 1].HasTile)
				{
					YMovement = -1f;
				}
				else if (Main.tile[tileX, tileY + 2].HasTile)
				{
					YMovement = -1f;
				}
			}

			// limits on y velocity
			if (NPC.velocity.Y > 0.4f || NPC.velocity.Y < -0.4f)
			{
				NPC.velocity.Y *= 0.95f;
			}

			// Run away from any non killifish npc
			foreach (var otherNPC in Main.ActiveNPCs)
			{
				if (otherNPC.type != ModContent.NPCType<Killifish>())
				{
					if (NPC.DistanceSQ(otherNPC.Center) < 60 * 65 && otherNPC.wet)
					{
						Vector2 vel = NPC.DirectionFrom(otherNPC.Center) * 6.2f;
						NPC.velocity = vel;
						NPC.rotation = MathHelper.WrapAngle((float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + (NPC.velocity.X < 0 ? MathHelper.Pi : 0));
						if (otherNPC.position.X > NPC.position.X)
						{
							NPC.spriteDirection = -1;
							NPC.direction = -1;
							NPC.netUpdate = true;
						}
						else if (otherNPC.position.X <= NPC.position.X)
						{
							NPC.spriteDirection = 1;
							NPC.direction = 1;
							NPC.netUpdate = true;
						}

						Turning = 0;
					}
				}
			}
			// check for proximity
			if (NPC.DistanceSQ(target.Center) < 40 * 65 && target.wet)
			{
				Proximity = 1;
			}
			else
			{
				Proximity = 0;
			}

			if (Proximity == 1) //Swimming away from player
			{
				Vector2 vel = NPC.DirectionFrom(target.Center) * 2.5f;
				NPC.velocity = vel;
				NPC.rotation = NPC.velocity.X * .04f;
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
		else // flopping around
		{
			// falling rotation
			NPC.rotation = NPC.velocity.Y * NPC.direction * 0.1f;
			if (NPC.rotation < -0.2f)
			{
				NPC.rotation = -0.2f;
			}

			if (NPC.rotation > 0.2f)
			{
				NPC.rotation = 0.2f;
			}

			// no running away
			Proximity = 0;
			// floppa velocity
			if (NPC.velocity.Y == 0f) 
			{
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					NPC.velocity.Y = Main.rand.Next(-50, -20) * 0.1f;
					NPC.velocity.X = Main.rand.Next(-20, 20) * 0.1f;
					NPC.netUpdate = true;
				}
			}

			// fall
			NPC.velocity.Y += 0.3f;
			if (NPC.velocity.Y > 10f)
			{
				NPC.velocity.Y = 10f;
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
	float frameTimer;
	public override void FindFrame(int frameHeight)
	{
		NPC.frameCounter += .25f;
		NPC.frameCounter %= Main.npcFrameCount[NPC.type];
		int frame = (int)NPC.frameCounter;
		NPC.frame.Y = frame * frameHeight;
		NPC.frame.X = 46 * pickedType;
		NPC.frame.Width = 46;
	}
	public override void HitEffect(NPC.HitInfo hit)
	{
		for (int num621 = 0; num621 < 13; num621++)
		{
			Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, 2f * hit.HitDirection, -2f, 0, default, Main.rand.NextFloat(0.75f, 0.95f));
		}

		if (NPC.life <= 0 && Main.netMode != NetmodeID.Server)
		{

			if (pickedType == 0)
			{
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("KillifishGore1").Type, 1f);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("KillifishGore2").Type, Main.rand.NextFloat(.5f, .7f));
			}
			else
			{
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("KillifishGore3").Type, 1f);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("KillifishGore4").Type, Main.rand.NextFloat(.5f, .7f));
			}
		}
	}
	public override void ModifyNPCLoot(NPCLoot npcLoot) => npcLoot.AddCommon<RawFish>();
}