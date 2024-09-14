namespace SpiritReforged.Content.Savanna.NPCs;

public class Ostrich : ModNPC
{
	private static readonly int[] endFrames = [8, 3, 9, 6, 6];
	private bool OnTransitionFrame => (int)NPC.frameCounter == endFrames[AIState] - 1;
	private float frameRate = .2f;

	private enum State : byte
	{
		Running,
		Stopped,
		MunchStart,
		Munching,
		MunchEnd
	}

	public int AIState { get => (int)NPC.ai[0]; set => NPC.ai[0] = value; }
	public ref float Counter => ref NPC.ai[1];

	public override void SetStaticDefaults() => Main.npcFrameCount[Type] = 9; //Rows

	public override void SetDefaults()
	{
		NPC.Size = new Vector2(40, 60);
		NPC.damage = 0;
		NPC.defense = 0;
		NPC.lifeMax = 100;
		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath1;
		NPC.knockBackResist = .45f;
		AIType = -1;
	}

	public override void AI()
	{
		static bool NotClient() => Main.netMode != NetmodeID.MultiplayerClient; //Needs checked often

		NPC.TargetClosest(false);
		var target = Main.player[NPC.target];

		switch (AIState)
		{
			case (int)State.Running:
				const float runSpeed = 4f;
				frameRate = MathHelper.Clamp(Math.Abs(NPC.velocity.X) / runSpeed, .5f, 1f) * .25f;

				Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);

				if (NPC.collideX && NPC.velocity.X == 0 && Counter % 5 == 0)
					NPC.velocity.Y = -6.5f; //Jump

				if (NPC.Distance(target.Center) < 20 * 20) //Prioritize running from the player
					NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, Math.Sign(NPC.Center.X - target.Center.X) * runSpeed, .1f);
				else if (Counter % 80 == 0 && NotClient() && Main.rand.NextBool(2) || Counter > 500 || NPC.velocity.X == 0)
				{
					if (NPC.velocity.X == 0)
					{
						if (NotClient())
						{
							NPC.velocity.X = Main.rand.NextFloat(1.25f, 1.5f) * (Main.rand.NextBool() ? -1 : 1);
							NPC.netUpdate = true;
						}
					}
					else
						ChangeState(State.Stopped);
				}

				break;

			case (int)State.Stopped:
				frameRate = .1f;

				if (NPC.Distance(target.Center) < 8 * 16)
					ChangeState(State.Running);
				else if (Counter % 60 == 0 && NotClient())
				{
					if (Main.rand.NextBool(3))
						ChangeState(State.Running, true);
					if (Main.rand.NextBool(4))
						ChangeState(State.MunchStart, true);
					//Have a chance to start running, munching, or just remain idle
				}

				NPC.velocity.X = 0;

				break;

			case (int)State.MunchStart:
				if (OnTransitionFrame)
					ChangeState(State.Munching);

				break;

			case (int)State.Munching:
				if (NPC.Distance(target.Center) < 8 * 16)
				{
					ChangeState(State.MunchEnd);
					frameRate = .25f; //Stop in a hurry
				}
				else if (OnTransitionFrame && Counter % 30 == 0 && Main.rand.NextBool(3))
				{
					if (NotClient() && Main.rand.NextBool(4))
					{
						ChangeState(State.MunchEnd, true);
						frameRate = .1f;
					}
					else
						NPC.frameCounter = 0; //Randomly restart the animation
				}

				break;

			case (int)State.MunchEnd:
				if (OnTransitionFrame)
				{
					ChangeState(State.Stopped);
					NPC.frameCounter = endFrames[AIState] - 1; //Skip the animation in this context
				}

				break;
		}

		if (NPC.velocity.X < 0)
			NPC.direction = NPC.spriteDirection = -1;
		else if (NPC.velocity.X > 0)
			NPC.direction = NPC.spriteDirection = 1;

		Counter++;
	}

	private void ChangeState(State toState, bool sync = false)
	{
		if (AIState == (int)toState) //We switched to a new state
			return;

		NPC.frameCounter = 0;
		frameRate = .2f;
		Counter = 0;
		AIState = (int)toState;

		if (sync && Main.dedServ)
			NPC.netUpdate = true;
	}

	public override void HitEffect(NPC.HitInfo hit)
	{
		bool dead = NPC.life <= 0;

		for (int i = 0; i < (dead ? 30 : 4); i++)
		{
			Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Blood, Scale: Main.rand.NextFloat(.8f, 2f))
				.velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(2f);
		}

		if (!Main.dedServ && dead)
			for (int i = 1; i < 5; i++)
				Gore.NewGore(NPC.GetSource_Death(), Main.rand.NextVector2FromRectangle(NPC.getRect()), NPC.velocity * Main.rand.NextFloat(.3f), Mod.Find<ModGore>("Ostrich" + i).Type);

		ChangeState(State.Running);
		NPC.velocity.X = Math.Sign(NPC.Center.X - Main.player[NPC.target].Center.X) * 4f;
	}

	public override void FindFrame(int frameHeight)
	{
		NPC.frame.Width = 96; //frameHeight = 90
		NPC.frame.X = NPC.frame.Width * AIState;

		NPC.frameCounter += frameRate;

		if (AIState is (int)State.Running) //Running is the only state that loops automatically
			NPC.frameCounter %= endFrames[AIState];
		else if (NPC.frameCounter >= endFrames[AIState])
			NPC.frameCounter--;

		if (!NPC.collideY && NPC.velocity.Y < 0) //Jump frame
		{
			NPC.frameCounter = 3; //Set frameCounter so we transition smoothly from the jump frame
			(NPC.frame.X, NPC.frame.Y) = (0, (int)NPC.frameCounter * frameHeight);
		}
		else
			NPC.frame.Y = (int)NPC.frameCounter * frameHeight;
	}

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		var texture = TextureAssets.Npc[Type].Value;
		var source = NPC.frame with { Width = NPC.frame.Width - 2, Height = NPC.frame.Height - 2 }; //Remove padding
		var position = NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY - (source.Height - NPC.height) / 2 + 4);
		var extraOffset = new Vector2(16 * NPC.spriteDirection, 0);

		var effects = (NPC.spriteDirection == 1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
		var color = NPC.GetAlpha(NPC.GetNPCColorTintedByBuffs(drawColor));

		Main.EntitySpriteDraw(texture, position + extraOffset, source, color, NPC.rotation, source.Size() / 2, NPC.scale, effects);

		return false;
	}
}
