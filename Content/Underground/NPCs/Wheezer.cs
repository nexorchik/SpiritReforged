using SpiritReforged.Common.NPCCommon;
using System.IO;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader.Utilities;

namespace SpiritReforged.Content.Underground.NPCs;

[AutoloadBanner]
public class Wheezer : ModNPC
{
	private enum State : byte
	{
		Idle,
		Sleep,
		Hiccup,
		Wake,
		Walk,
		Wheeze
	}

	private static readonly int[] endFrames = [2, 5, 2, 2, 9, 13];

	public ref float Animation => ref NPC.ai[0];
	public ref float LocalCounter => ref NPC.localAI[0];

	private bool OnTransitionFrame => NPC.frameCounter == LastFrame - 1;
	private int LastFrame => endFrames[(int)Animation % endFrames.Length];

	/// <summary> Determines the colouration of this NPC. </summary>
	private byte _style;
	/// <summary> The cooldown between attacks. </summary>
	private int _cooldown;
	/// <summary> Whether this NPC is experiencing an expert-exclusive death. </summary>
	private bool _explosiveDeath;
	/// <summary> Tracks for how long this NPC has stayed in the same spot. </summary>
	private int _idleTimer;

	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[Type] = 13;
		NPCHelper.ImmuneTo(this, BuffID.Poisoned);
	}

	public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) => bestiaryEntry.AddInfo(this, "Caverns");

	public override void SetDefaults()
	{
		NPC.Size = new(36);
		NPC.damage = 18;
		NPC.defense = 9;
		NPC.lifeMax = 50;
		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath53;
		NPC.value = 120;
		NPC.knockBackResist = .35f;
		NPC.aiStyle = -1;
	}

	public override void OnSpawn(IEntitySource source)
	{
		_style = (byte)Main.rand.Next(4);
		NPC.netUpdate = true;
	}

	public override void AI()
	{
		if (_explosiveDeath)
		{
			NPC.velocity.X *= .9f;
			ChangeState(State.Wheeze);

			if (NPC.frameCounter > 3.8f)
			{
				if (Main.netMode != NetmodeID.MultiplayerClient)
					Projectile.NewProjectile(Entity.GetSource_FromAI(), NPC.Center, Vector2.Zero, ProjectileID.GasTrap, 10, 1);

				NPC.active = false;
			}

			return;
		}

		NPC.TargetClosest((State)Animation is State.Walk or State.Idle);
		var target = Main.player[NPC.target];
		bool canHit = Collision.CanHit(NPC, target);

		if (TrySleeping(canHit))
		{
		}
		else
		{
			WalkingBehaviour(canHit);
		}

		NPC.spriteDirection = NPC.direction;
		LocalCounter = ++LocalCounter;
		_cooldown = Math.Max(_cooldown - 1, 0);
	}

	private void WalkingBehaviour(bool canHit)
	{
		const int attackDistance = 90;
		var target = Main.player[NPC.target];

		if (target.DistanceSQ(NPC.Center) < attackDistance * attackDistance && canHit && _cooldown == 0)
			ChangeState(State.Wheeze);

		if ((State)Animation is State.Wheeze)
		{
			NPC.velocity.X *= .9f;

			if (Main.netMode != NetmodeID.MultiplayerClient && (int)NPC.frameCounter > 4 && LocalCounter % 10 == 0)
				Projectile.NewProjectile(Entity.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(target.Center) * 3f, ProjectileID.GasTrap, 10, 1);

			if (OnTransitionFrame)
			{
				_cooldown = 360;
				ChangeState(State.Walk);
			}
		}
		else
		{
			if (target.Center.Y < NPC.Center.Y - 16 * 4)
			{
				NPC.velocity.X *= .92f;
				_idleTimer++;

				if (Math.Abs(NPC.velocity.X) < .5f)
				{
					ChangeState(State.Idle);
					NPC.frameCounter = 0;
				}

				return;
			}
			else
				NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, NPC.direction, .1f);

			ChangeState(State.Walk);
			Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);

			if ((NPC.collideX || FoundGap()) && NPC.velocity.Y == 0)
				NPC.velocity.Y = -5f; //Jump

			if (NPC.collideX)
				_idleTimer++;
			else
				_idleTimer = 0;
		}

		bool FoundGap()
		{
			var ahead = Framing.GetTileSafely(NPC.Bottom + new Vector2(16 * NPC.direction, 0));
			return !Collision.SolidCollision(NPC.position + new Vector2(16 * NPC.direction, 16), NPC.width, NPC.height) && !Main.tileSolidTop[ahead.TileType];
		}
	}

	private bool TrySleeping(bool canHit)
	{
		const int idleDistance = 300;
		var target = Main.player[NPC.target];

		if ((State)Animation is State.Hiccup)
		{
			if (OnTransitionFrame)
			{
				ChangeState(State.Sleep);
				NPC.frameCounter = LastFrame;
			}
		}
		else
		{
			if ((target.DistanceSQ(NPC.Center) > idleDistance * idleDistance || _idleTimer > 120) && !canHit)
				ChangeState(State.Sleep);
			else if ((State)Animation is State.Sleep)
				ChangeState(State.Wake);

			if ((State)Animation is State.Wake && OnTransitionFrame)
				ChangeState(State.Walk);

			if ((State)Animation is State.Sleep)
			{
				NPC.velocity.X *= .92f;

				if (LocalCounter % 60 == 0 && Main.rand.NextBool(5))
					ChangeState(State.Hiccup);
			}
		}

		return (State)Animation is State.Sleep or State.Hiccup or State.Wake;
	}

	private void ChangeState(State to)
	{
		if ((State)Animation == to)
			return;

		Animation = (int)to;
		NPC.frameCounter = 0;
	}

	public override void HitEffect(NPC.HitInfo hit)
	{
		if (Main.dedServ)
			return;

		for (int k = 0; k < 11; k++)
			Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, .61f);

		if (NPC.life <= 0 && !Main.expertMode)
		{
			string name = _style switch
			{
				1 => "Purple",
				2 => "Red",
				3 => "Teal",
				_ => "Albino"
			};

			for (int i = 1; i < 5; i++)
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Wheezer" + name + i).Type);
		}
	}

	public override bool CheckDead()
	{
		if (Main.expertMode)
		{
			NPC.life = 1;
			NPC.dontTakeDamage = true;
			_explosiveDeath = true;

			NPC.netUpdate = true;
			return false;
		}

		return true;
	}

	//No contact damage
	public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
	public override bool CanHitNPC(NPC target) => false;

	public override void FindFrame(int frameHeight)
	{
		if (NPC.IsABestiaryIconDummy)
			ChangeState(State.Walk);

		bool canLoop = (State)Animation is State.Walk;

		NPC.frame.Width = 80;
		NPC.frame.X = NPC.frame.Width * (int)Animation + NPC.frame.Width * endFrames.Length * _style;

		NPC.frameCounter += 0.15f;
		NPC.frameCounter = canLoop ? NPC.frameCounter % LastFrame : Math.Min(NPC.frameCounter, LastFrame - 1);
		int frame = (int)NPC.frameCounter;

		NPC.frame.Y = frame * frameHeight;

		if (canLoop && NPC.velocity.Y != 0) //Jump
		{
			NPC.frame.X = 480 * _style;
			NPC.frame.Y = frameHeight;
		}
	}

	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
		npcLoot.Add(ItemDropRule.Common(ItemID.DepthMeter, 80));
		npcLoot.Add(ItemDropRule.Common(ItemID.Compass, 80));
		npcLoot.Add(ItemDropRule.Common(ItemID.Rally, 20));
	}

	public override float SpawnChance(NPCSpawnInfo spawnInfo)
	{
		if (spawnInfo.PlayerSafe || spawnInfo.Player.ZoneSnow)
			return 0f;

		return SpawnCondition.Cavern.Chance * (Main.hardMode ? .03f : .17f);
	}

	public override void SendExtraAI(BinaryWriter writer)
	{
		writer.Write(_style);
		writer.Write(_explosiveDeath);
	}

	public override void ReceiveExtraAI(BinaryReader reader)
	{
		_style = reader.ReadByte();
		_explosiveDeath = reader.ReadBoolean();
	}

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		var texture = TextureAssets.Npc[Type].Value;
		var origin = new Vector2(NPC.frame.Width / 2, NPC.frame.Height - NPC.height / 2 - 4);
		var effects = (NPC.spriteDirection == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
		var pos = NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY);

		Main.EntitySpriteDraw(texture, pos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, origin, NPC.scale, effects);
		return false;
	}
}