using SpiritReforged.Content.Vanilla.Items.Food;
using Terraria.GameContent.Bestiary;

namespace SpiritReforged.Content.Savanna.NPCs.Gar;

[AutoloadCritter]
public class GoldGar : ModNPC
{
	private ref float YMovement => ref NPC.ai[0]; // Y Movement (adapted from vanilla)
	private ref float Proximity => ref NPC.ai[1]; // Player proximity 
	private ref float Resting => ref NPC.ai[2]; // Resting check;
	private ref float RestTimer => ref NPC.ai[3]; // Loop through resting phase
	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[NPC.type] = 12;
		Main.npcCatchable[NPC.type] = true;
		NPCID.Sets.CountsAsCritter[Type] = true;
	}

	public override void SetDefaults()
	{
		NPC.width = 40;
		NPC.height = 22;
		NPC.damage = 0;
		NPC.defense = 0;
		NPC.lifeMax = 5;
		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath1;
		NPC.knockBackResist = .35f;
		NPC.noGravity = true;
		NPC.npcSlots = 0;
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
		RestTimer++;
		if (NPC.wet) //swimming AI (adapted from vanilla)
		{
			if (NPC.rotation != 0f)
				NPC.rotation *= .9f;

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

			//Predation: seeks out Killifish to kill
			foreach (var otherNPC in Main.ActiveNPCs)
			{
				if (otherNPC.type == ModContent.NPCType<Killifish.Killifish>())
				{
					if (NPC.DistanceSQ(otherNPC.Center) < 100 * 65 && otherNPC.wet)
					{
						Vector2 vel = NPC.DirectionTo(otherNPC.Center) * 3f;
						NPC.velocity = vel;
						NPC.rotation = MathHelper.WrapAngle((float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + (NPC.velocity.X < 0 ? MathHelper.Pi : 0));
						NPC.friendly = false;
						NPC.damage = 1;
						if (NPC.velocity.X <= 0)
						{
							NPC.spriteDirection = -1;
							NPC.direction = -1;
							NPC.netUpdate = true;
						}
						else if (NPC.velocity.X > 0)
						{
							NPC.spriteDirection = 1;
							NPC.direction = 1;
							NPC.netUpdate = true;
						}

						Resting = 0;
						break;
					}
					else
					{
						//reset friendliness otherwise
						NPC.damage = 0;
					}
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

			// movement
			if (Resting != 1)
			{
				NPC.velocity.X += NPC.direction * (Main.dayTime ? .06f : .1f);

				if (NPC.velocity.X < (Main.dayTime ? -.8f : 1.1f) || NPC.velocity.X > (Main.dayTime ? .8f : 1.1f))
					NPC.velocity.X *= 0.95f;
			}

			// fish goes up and down, and goes the other way upon reaching a limit
			if (YMovement == -1f)
			{
				NPC.velocity.Y -= 0.01f;
				if (NPC.velocity.Y < -0.3f)
					YMovement = 1f;
			}
			else
			{
				NPC.velocity.Y += 0.01f;
				if (NPC.velocity.Y > 0.3f)
					YMovement = -1f;
			}

			// don't swim too close to bottom tiles
			if (Main.tile[tileX, tileY - 1].LiquidAmount > 128)
			{
				if (Main.tile[tileX, tileY + 1].HasTile)
					YMovement = -1f;

				else if (Main.tile[tileX, tileY + 2].HasTile)
					YMovement = -1f;
			}

			// limits on y velocity
			if (NPC.velocity.Y > 0.4f || NPC.velocity.Y < -0.4f)
				NPC.velocity.Y *= 0.95f;

			// Gar Resting AI
			if (Main.dayTime)
			{
				if (RestTimer > 60 * 20 && RestTimer < 60 * 30)
				{
					if (Main.rand.NextBool(3))
					{
						Resting = 1;
						NPC.netUpdate = true;
					}
					else
						Resting = 0;
				}

				if (RestTimer > 60 * 60)
				{
					RestTimer = 0;
					Resting = 0;
				}
			}
			else
				Resting = 0;

			if (Resting == 1)
			{
				if (Main.rand.NextBool(40))
				{
					float bubbleX = NPC.position.X + (NPC.width / 2) + (NPC.direction == 1 ? NPC.width / 2 + 20 : -NPC.width / 2 - 20);
					float bubbleY = NPC.position.Y + NPC.height / 2 - 4;
					Dust.NewDust(new Vector2(bubbleX, bubbleY), 0, 0, DustID.BreatheBubble, .1f * NPC.direction, Main.rand.NextFloat(-1.14f, -1.48f), 0, new Color(255, 255, 255, 200), Main.rand.NextFloat(.65f, .85f));
				}

				if (NPC.velocity.X != 0)
					NPC.velocity.X *= 0.5f;
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
			// no running away
			Proximity = 0;
			Resting = 0;
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
				NPC.velocity.Y = 10f;

			// falling rotation
			NPC.rotation = NPC.velocity.Y* 0.1f;
			if (NPC.rotation < -0.4f)
				NPC.rotation = -0.4f;

			if (NPC.rotation > 0.4f)
				NPC.rotation = 0.4f;

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
	public override void ModifyNPCLoot(NPCLoot npcLoot) => npcLoot.AddCommon<RawFish>(2);
}