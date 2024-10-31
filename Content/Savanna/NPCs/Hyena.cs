using SpiritReforged.Common.NPCCommon;
using System.Linq;

namespace SpiritReforged.Content.Savanna.NPCs;

[SpawnPack(2, 3)]
public class Hyena : ModNPC
{
	private static readonly int[] endFrames = [2, 5, 5, 5, 4, 13];
	private bool OnTransitionFrame => (int)NPC.frameCounter == endFrames[AIState] - 1;

	private bool runOffScreen;
	private bool cautious;

	private enum State : byte
	{
		TrotStart,
		Trotting,
		TrottingAngry,
		BarkingAngry,
		TrotEnd,
		Laugh
	}

	public int AIState { get => (int)NPC.ai[0]; set => NPC.ai[0] = value; }
	public ref float Counter => ref NPC.ai[1];

	private bool IsAngry => AIState is ((int)State.TrottingAngry) or ((int)State.BarkingAngry);

	public override void SetStaticDefaults() => Main.npcFrameCount[Type] = 13; //Rows

	public override void SetDefaults()
	{
		NPC.Size = new Vector2(40, 40);
		NPC.damage = 10;
		NPC.defense = 0;
		NPC.lifeMax = 100;
		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath1;
		NPC.knockBackResist = .45f;
		AIType = -1;
	}

	public override void AI()
	{
		NPC.TargetClosest(false);
		var target = Main.player[NPC.target];
		const int alertDistance = 16 * 12; //The base distance from the player which Hyena will start running

		switch (AIState)
		{
			case (int)State.TrotStart:
				if (OnTransitionFrame)
					ChangeState(State.Trotting);

				break;

			case (int)State.Trotting:
				Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);

				if (cautious)
				{
					const float walkSpeed = 1.5f;

					if (NPC.Distance(target.Center) > alertDistance + 24)
						NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, Math.Sign(target.Center.X - NPC.Center.X) * walkSpeed, .05f);
					else
					{
						ChangeState(State.TrotEnd);
						cautious = false;
					}

					break;
				}

				if (NPC.collideX && NPC.velocity.X == 0 && Counter % 5 == 0)
					NPC.velocity.Y = -6.5f; //Jump

				const float runSpeed = 4.8f;

				if (!runOffScreen && NPC.Distance(target.Center) > alertDistance + 96 && Counter % 100 == 0)
					ChangeState(State.TrotEnd);
				else
					NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, Math.Sign(NPC.Center.X - target.Center.X) * runSpeed, .08f);

				Separate();
				break;

			case (int)State.TrotEnd: //Doubles as our idle state
				if (Counter == 250)
					ChangeState(State.Laugh, false);
				else if (OnTransitionFrame)
				{
					if (Counter >= 30)
						NPC.direction = NPC.spriteDirection = (target.Center.X < NPC.Center.X) ? -1 : 1;
					if (Counter % 150 == 149 && Main.netMode != NetmodeID.MultiplayerClient && Main.rand.NextBool(3) && NPC.Distance(target.Center) > alertDistance + 24)
					{
						ChangeState(State.TrotStart, sync: true);
						cautious = true; //Hyena will slowly encroach
					}
					else if (NPC.Distance(target.Center) < alertDistance)
						ChangeState(State.TrotStart);
				}

				NPC.velocity.X = 0;

				break;

			case (int)State.Laugh:
				if (OnTransitionFrame)
				{
					ChangeState(State.TrotEnd, false);
					NPC.frameCounter = endFrames[AIState];
				}

				break;
		}

		if (IsAngry) //Accounts for states TrottingAngry and BarkingAngry
		{
			const float runSpeed = 5.5f;
			float distance = MathHelper.Clamp(NPC.Distance(target.Center) / (16 * 5), 0, 1);
			NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, Math.Sign(target.Center.X - NPC.Center.X) * runSpeed, .002f + distance * .08f);

			Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);

			if (NPC.collideX && NPC.velocity.X == 0 && Counter % 30 == 0)
				NPC.velocity.Y = -6.5f; //Jump

			if (AIState == (int)State.BarkingAngry)
			{
				if (OnTransitionFrame)
					ChangeState(State.TrottingAngry, false);
			}
			else if (Main.rand.NextBool(100))
				ChangeState(State.BarkingAngry, false);

			Separate();
		}
		else if (target.statLife < target.statLifeMax2 * .2f)
			ChangeState(State.TrottingAngry); //Begin to chase the player if they are low on health

		if (NPC.velocity.X < 0)
			NPC.direction = NPC.spriteDirection = -1;
		else if (NPC.velocity.X > 0)
			NPC.direction = NPC.spriteDirection = 1;

		Counter++;

		void Separate(int distance = 32)
		{
			var nearest = Main.npc.OrderBy(x => x.Distance(NPC.Center)).Where(x => x.whoAmI != NPC.whoAmI && x.type == Type
				&& x.Distance(NPC.Center) < distance).FirstOrDefault(); //All NPC instances of this type, including this one

			if (nearest != default)
			{
				float update = Math.Sign(NPC.Center.X - nearest.Center.X) * .1f;

				if (Math.Sign(NPC.velocity.X) == Math.Sign(NPC.velocity.X + update)) //Does this require a change in direction?
					NPC.velocity.X += update;
			}
		}
	}

	private void ChangeState(State toState, bool resetCounter = true, bool sync = false)
	{
		if (AIState == (int)toState) //We switched to a new state
			return;

		NPC.frameCounter = 0;
		AIState = (int)toState;

		if (resetCounter)
			Counter = 0;
		if (sync && Main.dedServ)
			NPC.netUpdate = true;
	}

	public override bool CanHitPlayer(Player target, ref int cooldownSlot) => IsAngry;

	public override bool CanHitNPC(NPC target) => IsAngry;

	public override void HitEffect(NPC.HitInfo hit)
	{
		bool dead = NPC.life <= 0;

		for (int i = 0; i < (dead ? 20 : 3); i++)
		{
			Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Blood, Scale: Main.rand.NextFloat(.8f, 2f))
				.velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(2f);
		}

		if (!Main.dedServ && dead)
			for (int i = 1; i < 4; i++)
				Gore.NewGore(NPC.GetSource_Death(), Main.rand.NextVector2FromRectangle(NPC.getRect()), NPC.velocity * Main.rand.NextFloat(.3f), Mod.Find<ModGore>("Hyena" + i).Type);

		const int detectDistance = 16 * 25;
		var pack = Main.npc.Where(x => x.type == Type && (x.whoAmI == NPC.whoAmI || x.Distance(NPC.Center) < detectDistance)); //All NPC instances of this type, including this one

		if (hit.Damage >= NPC.lifeMax / 2) //Scare nearby hyena
		{
			foreach (var npc in pack)
			{
				(npc.ModNPC as Hyena).ChangeState(State.Trotting);
				(npc.ModNPC as Hyena).runOffScreen = true;
			}
		}
		else //Anger nearby Hyena
		{
			foreach (var npc in pack)
				(npc.ModNPC as Hyena).ChangeState(State.TrottingAngry);
		}
	}

	public override void FindFrame(int frameHeight)
	{
		NPC.frame.Width = 72; //frameHeight = 48
		NPC.frame.X = NPC.frame.Width * AIState;

		NPC.frameCounter += cautious ? .1f : .2f;

		if (AIState is (int)State.Trotting or (int)State.TrottingAngry) //Trotting states loop automatically
			NPC.frameCounter %= endFrames[AIState];
		else if (NPC.frameCounter >= endFrames[AIState])
			NPC.frameCounter--;

		if (!NPC.collideY && NPC.velocity.Y < 0) //Jump frame
		{
			NPC.frameCounter = 0; //Set frameCounter so we transition smoothly from the jump frame
			(NPC.frame.X, NPC.frame.Y) = (1, (int)NPC.frameCounter * frameHeight);
		}
		else
			NPC.frame.Y = (int)NPC.frameCounter * frameHeight;
	}

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		var texture = TextureAssets.Npc[Type].Value;
		var source = NPC.frame with { Width = NPC.frame.Width - 2, Height = NPC.frame.Height - 2 }; //Remove padding
		var position = NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY - (source.Height - NPC.height) / 2 + 4);
		
		var effects = (NPC.spriteDirection == 1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
		var color = NPC.GetAlpha(NPC.GetNPCColorTintedByBuffs(drawColor));

		Main.EntitySpriteDraw(texture, position, source, color, NPC.rotation, source.Size() / 2, NPC.scale, effects);

		return false;
	}

	public override float SpawnChance(NPCSpawnInfo spawnInfo)
	{
		if (spawnInfo.Player.InModBiome<Biome.SavannaBiome>() && !spawnInfo.Water)
			return .2f;

		return 0;
	}
}
